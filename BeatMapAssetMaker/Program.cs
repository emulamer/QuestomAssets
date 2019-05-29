using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BeatmapAssetMaker.AssetsChanger;
using System.Drawing;
using System.Drawing.Imaging;
using Newtonsoft.Json.Linq;
using BeatmapAssetMaker.BeatSaber;


namespace BeatmapAssetMaker
{
    class Program
    {

        // static void WriteLevelCollectionAsser(string name, string assetFileName, byte[] beatmapDataSOPtr)

        static UPtr ReplaceLevelCollection = new UPtr() { FileID = 0, PathID = 240 };

        static bool IMPORT_COVERS = false;

        const int BEATMAPDATA_SCRIPT_ID = 0x0E;
        const int BEATMAPLEVEL_SCRIPT_ID = 0x0F;
        const int LEVELCOLLECTION_SCRIPT_ID = 0x10;

        
        

        static byte[] MakeBeatmapLevelSOAsset(string assetName, BeatmapLevelDataSO levelData)
        {
            using (MemoryStream f = new MemoryStream())
            {                
                var fs = new AlignedStream(f);
                
                levelData.Write(fs);
                fs.AlignTo(4);
                return f.ToArray();
            }
        }

        static byte[] MakeLevelCollectionAsset(string assetName, BeatmapLevelCollectionSO levelCollection)
        {
            using (MemoryStream f = new MemoryStream())
            {
                AlignedStream fs = new AlignedStream(f);
                levelCollection.Write(fs);
                return f.ToArray();
            }
        }

        static byte[] MakeBeatmapAsset(string assetName, BeatmapSaveData beatmapSaveData)
        {
            using (MemoryStream f = new MemoryStream())
            { 
                AlignedStream fs = new AlignedStream(f);
                //WriteMBHeader(fs, assetName, BeatmapDataSOType);
                //json data string length (0)
                fs.Write((int)0);
                //not really sure if the signature has to be 128 bytes, tossing in all zeroes just in case

                //signature length (128)
                fs.Write((int)128);

                ///signature (all zeroes)
                fs.Write(new byte[128], 0, 128);

                //_projectedData goes next
                byte[] projectedData = beatmapSaveData.SerializeToBinary(false);
                fs.Write(projectedData);
                fs.AlignTo(4);
                return f.ToArray();
            }
        }


        public static byte[] LoadToRGB(Image image)
        {
            
            
            byte[] imageBytes;
            using (MemoryStream msCover = new MemoryStream())
            {
                image.Save(msCover, System.Drawing.Imaging.ImageFormat.Bmp);

                imageBytes = new byte[msCover.Length - 54];
                byte[] msBytes = msCover.ToArray();
                Array.Copy(msBytes, 54, imageBytes, 0, imageBytes.Length);
            }
            for (int i = 0; i < imageBytes.Length-2; i += 3)
            {
                byte hold = imageBytes[i];
                imageBytes[i] = imageBytes[i + 2];
                imageBytes[i + 2] = hold;
            }
            return imageBytes;
        }

        /// <summary>
        /// Makes an asseet 
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="outputPath"></param>
        /// <returns>the UPtr to the BeatSaberLevel</returns>
        static AssetsPtr MakeAssets(string inputPath, AssetsFile assetsFile, string outputAudioPath)
        {

            var sng = CustomSongLoader.LoadFromPath(inputPath);

            if (assetsFile.Objects.Where(x=>x is AssetsMonoBehaviourObject).Any(x=>((AssetsMonoBehaviourObject)x).Name == sng._levelID+"Level"))
            {
                Console.WriteLine($"{inputPath} seems to have a duplicate level ID of {sng._levelID} as another song that's already loaded.  Skipping it.");
                return null;
            }
                                 
            AssetsTexture2D coverAsset = null;
            string audioClipFile = Path.Combine(inputPath, sng._songFilename);
            string outputAudioClipFile = Path.Combine(outputAudioPath, sng._levelID + ".ogg");
            if (IMPORT_COVERS)
            {
                if (!string.IsNullOrWhiteSpace(sng._coverImageFilename) && File.Exists(Path.Combine(inputPath, sng._coverImageFilename)))
                {
                    try
                    {
                        string coverFile = Path.Combine(inputPath, sng._coverImageFilename);
                        Image coverImage = Image.FromFile(coverFile);
                        var imageBytes = LoadToRGB(coverImage);

                        coverAsset = new AssetsTexture2D(new AssetsObjectInfo()
                        {
                            TypeIndex = assetsFile.GetTypeIndexFromClassID(AssetsConstants.Texture2DClassID)
                        })
                        {
                            Name = sng._levelID + "Cover",
                            ForcedFallbackFormat = 4,
                            DownscaleFallback = false,
                            Width = coverImage.Width,
                            Height = coverImage.Height,
                            CompleteImageSize = imageBytes.Length,
                            TextureFormat = 3,
                            MipCount = 1,
                            IsReadable = false,
                            StreamingMipmaps = false,
                            StreamingMipmapsPriority = 0,
                            ImageCount = 1,
                            TextureDimension = 2,
                            TextureSettings = new AssetsGLTextureSettings()
                            {
                                FilterMode = 2,
                                Aniso = 1,
                                MipBias = -1,
                                WrapU = 1,
                                WrapV = 1,
                                WrapW = 0
                            },
                            LightmapFormat = 6,
                            ColorSpace = 1,
                            ImageData = imageBytes,
                            StreamData = new AssetsStreamingInfo()
                            {
                                offset = 0,
                                size = 0,
                                path = ""
                            }
                        };
                        assetsFile.AddObject(coverAsset, true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error loading cover art asset: {0} {1}", ex.Message, ex.StackTrace);
                    }
                }
                else
                {
                    Console.WriteLine("No cover art found.");
                }
            }

            var v = new NVorbis.VorbisReader(audioClipFile);
            var audioClip = new AssetsAudioClip(new AssetsObjectInfo()
            {
                //todo: make these hash lookups and not by index
                TypeIndex = assetsFile.GetTypeIndexFromClassID(AssetsConstants.AudioClipClassID)
            })
            {
                Name = sng._levelID,
                LoadType = 1,
                IsTrackerFormat = false,
                Ambisonic = false,
                SubsoundIndex = 0,
                PreloadAudioData = false,
                LoadInBackground = true,
                Legacy3D = true,
                CompressionFormat = 1,
                BitsPerSample = 16,
                Channels = v.Channels,
                Frequency = v.SampleRate,
                Length = (Single)v.TotalTime.TotalSeconds,
                Resource = new AssetsStreamedResource(Path.GetFileName(outputAudioClipFile), 0, Convert.ToUInt64(new FileInfo(audioClipFile).Length))
            };
            assetsFile.AddObject(audioClip, true);
            //todo: move elsewhere
            File.Copy(audioClipFile, outputAudioClipFile, true);

            sng._environmentSceneInfo = new UPtr() { FileID = 20, PathID = 1 };
            sng._audioClip = audioClip.ObjectInfo.LocalPtrTo.ToUPtr();

            //temporarily, use beatsaber's cover art if one couldn't be loaded
            if (coverAsset == null)
                sng._coverImageTexture2D = new UPtr() { FileID = 0, PathID = 19 };
            else
                sng._coverImageTexture2D = coverAsset.ObjectInfo.LocalPtrTo.ToUPtr();

            foreach (var s in sng._difficultyBeatmapSets)
            {
                switch (s._beatmapCharacteristicName)
                {
                    case Characteristic.OneSaber:
                        s._beatmapCharacteristic = new UPtr() { FileID = 19, PathID = 1 };
                        break;
                    case Characteristic.NoArrows:
                        s._beatmapCharacteristic = new UPtr() { FileID = 6, PathID = 1 };
                        break;
                    case Characteristic.Standard:
                        s._beatmapCharacteristic = new UPtr() { FileID = 22, PathID = 1 };
                        break;
                }

                foreach (var g in s._difficultyBeatmaps)
                {
                    string bmAssetName = sng._levelID + ((s._beatmapCharacteristicName == Characteristic.Standard) ? "" : s._beatmapCharacteristicName.ToString()) + g._difficulty.ToString() + "BeatmapData";

                    byte[] assetData = MakeBeatmapAsset(bmAssetName, g._beatmapSaveData);
                    AssetsMonoBehaviourObject bmAsset = new AssetsMonoBehaviourObject(new AssetsObjectInfo()
                    {
                        TypeIndex = AssetsConstants.BeatmapDataSOTypeIndex
                    })
                    {
                        MonoscriptTypePtr = AssetsConstants.BeatmapDataTypePtr,
                        Name = bmAssetName,
                        ScriptParametersData = assetData,
                        GameObjectPtr = new AssetsPtr()
                    };
                    assetsFile.AddObject(bmAsset, true);
                    g._beatmapDataPtr = new UPtr() { FileID = 0, PathID = bmAsset.ObjectInfo.ObjectID };
                }
            }

            string levelAssetName = $"{sng._levelID}Level";


            byte[] bmLevelData = MakeBeatmapLevelSOAsset(levelAssetName, sng);
            AssetsMonoBehaviourObject bmLevelAsset = new AssetsMonoBehaviourObject(new AssetsObjectInfo()
            {
                //todo: make these hash lookups and not by index
                TypeIndex = AssetsConstants.BeatmapLevelTypeIndex
            })
            {
                MonoscriptTypePtr = AssetsConstants.BeatmapLevelTypePtr,
                Name = levelAssetName,
                ScriptParametersData = bmLevelData,
                GameObjectPtr = new AssetsPtr()
            };
            assetsFile.AddObject(bmLevelAsset, true);

            return bmLevelAsset.ObjectInfo.LocalPtrTo;

        }
        private static AssetsPtr LoadPackCover(AssetsFile assetsFile)
        {
            //var extrasSprite = assetsFile.Objects.First(x => x.ObjectInfo.ObjectID == 45);
            //I don't want to write all the code for the Sprint class, so I'm encoding the binary and swapping specific bits around
            string spriteData = "CwAAAEV4dHJhc0NvdmVyAAAAAAAAAAAAAACARAAAgEQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIBEAAAAPwAAAD8BAAAAAAAAAKrborQQ7eZHhB371sswB+MgA0UBAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADgAAAAAAAAAAAAAAAAAAAAAAAAABAAAAAAAAAAYAAAAAAAAAAAAAAAAAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAQACAAIAAQADAAQAAAAOAAAAAAAAAwAAAAAAAAAAAAAAAAEAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAAAAAAAAvwAAAD8AAAAAAAAAPwAAAD8AAAAAAAAAvwAAAL8AAAAAAAAAPwAAAL8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIBEAACARAAAAAAAAAAAAACAvwAAgL8AAAAAAACARAAAAEQAAIBEAAAARAAAgD8AAAAAAAAAAA==";
            byte[] spriteBytes = Convert.FromBase64String(spriteData);

            var imageBytes = BeatmapAssetMaker.Resource1.CustomSongsCover;

            var packCover = new AssetsTexture2D(new AssetsObjectInfo()
            {
                TypeIndex = assetsFile.GetTypeIndexFromClassID(AssetsConstants.Texture2DClassID)
            })
            {
                Name = "CustomSongsCover",
                ForcedFallbackFormat = 4,
                DownscaleFallback = false,
                Width = 1024,
                Height = 1024,
                CompleteImageSize = imageBytes.Length,
                TextureFormat = 34,
                MipCount = 11,
                IsReadable = false,
                StreamingMipmaps = false,
                StreamingMipmapsPriority = 0,
                ImageCount = 1,
                TextureDimension = 2,
                TextureSettings = new AssetsGLTextureSettings()
                {
                    FilterMode = 2,
                    Aniso = 1,
                    MipBias = -1,
                    WrapU = 1,
                    WrapV = 1,
                    WrapW = 0
                },
                LightmapFormat = 6,
                ColorSpace = 1,
                ImageData = imageBytes,
                StreamData = new AssetsStreamingInfo()
                {
                    offset = 0,
                    size = 0,
                    path = ""
                }
            };
            assetsFile.AddObject(packCover, true);
            byte[] finalBytes;
            using (MemoryStream ms = new MemoryStream(spriteBytes))
            {
                ms.Seek(116, SeekOrigin.Begin);
                using (AssetsWriter writer = new AssetsWriter(ms))
                    packCover.ObjectInfo.LocalPtrTo.Write(writer);

                finalBytes = ms.ToArray();
            }
            var nameBytes = System.Text.UTF8Encoding.UTF8.GetBytes("Custom");
            Array.Copy(nameBytes, 0, finalBytes, 4, nameBytes.Length);
            AssetsObject coverAsset = new AssetsObject(new AssetsObjectInfo()
            {
                TypeIndex = assetsFile.GetTypeIndexFromClassID(AssetsConstants.SpriteClassID)
            })
            {
                Data = finalBytes
            };

            assetsFile.AddObject(coverAsset, true);
            return coverAsset.ObjectInfo.LocalPtrTo; 
        }
        static void Main(string[] args)
        {
            
            //string playlist = @"C:\Program Files (x86)\Steam\steamapps\common\Beat Saber\Playlists\SongBrowserPluginFavorites.json";
            //string customSongsFolder2 = @"C:\Program Files (x86)\Steam\steamapps\common\Beat Saber\CustomSongs";
            //string copyToFolder = @"C:\Users\VR\Desktop\platform-tools_r28.0.3-windows\dist\ToConvert";
            //dynamic favs = JObject.Parse(File.ReadAllText(playlist));

            //JArray songs = favs["songs"] as JArray;
            //var songsToCopy = (from x in songs select x["key"].Value<string>()).ToList();
            //foreach (var key in songsToCopy)
            //{
            //    if (string.IsNullOrWhiteSpace(key))
            //        continue;
            //    string songPath = Path.Combine(customSongsFolder2, key) +"\\";
            //    if (!Directory.Exists(songPath))
            //    {
            //        Console.WriteLine($"Missing custom song: {songPath}");
            //        continue;
            //    }
            //    try
            //    {
            //        new Microsoft.VisualBasic.Devices.Computer().FileSystem.CopyDirectory(songPath, copyToFolder);
            //    } catch
            //    {
            //        Console.WriteLine($"Error copying {songPath}");
            //    }
            //}
            //return;

            
            try
            {
                if (args != null && args.Length == 2 && args[0].ToLower() == "--patch")
                {
                    if (!File.Exists(args[1]))
                    {
                        Console.WriteLine("Binary file to patch doesn't exist!");
                        Environment.ExitCode = -1;
                        return;
                    }
                    if (new FileInfo(args[1]).Length != 27041992)
                    {
                        Console.WriteLine("Binary file to patch is the wrong length!");
                        Environment.ExitCode = -1;
                        return;
                    }
                    Console.WriteLine("Patching binary...");
                    using (var fs = File.Open(args[1], FileMode.Open))
                    {
                        fs.Seek(0x0109D074, SeekOrigin.Begin);
                        byte[] readVals = new byte[4];
                        fs.Read(readVals, 0, 4);
                        if (readVals[0] != 0x8B || readVals[1] != 0xD8 || readVals[2] != 0xFE || readVals[3] != 0xEB)
                        {
                            if (readVals[0] == 0x01 && readVals[1] == 0x00 && readVals[2] == 0xA0 && readVals[3] == 0xE3)
                            {
                                Console.WriteLine("It already appears to be patched.");
                            }
                            else
                            {
                                Console.WriteLine("Can't patch this binary, the code at the patch location doesn't look familiar...");
                            }
                            Environment.ExitCode = -1;
                            return;
                        }
                        fs.Seek(0x0109D074, SeekOrigin.Begin);
                        fs.Write(new byte[] { 0x01, 0x00, 0xA0, 0xE3 }, 0, 4);
                    }
                    Console.WriteLine("Done!");
                    Environment.ExitCode = 0;
                    return;
                }
                if (args == null || args.Length < 3)
                {
                    Console.WriteLine("Usage: BeatmapAssetMaker assetsFolder outputAssetsFolder customSongsFolder [covers] [nomoxie]");
                    Console.WriteLine("\tassetsFolder: the name/path to where the assets files are. ");
                    Console.WriteLine("\toutputAssetsFolder: the output folder for the assets ");
                    Console.WriteLine("\tcustomSongsFolder: the folder that contains folders of custom songs in the new native beatsaber format, or a folder with a single song that contains an Info.dat file");
                    Console.WriteLine("\tIf you want covers (they will run slow) then add \"covers\" as the last parameter.  If you don't want the dog pack cover, add \"nomoxie\"");
                    Console.WriteLine("");
                    Console.WriteLine("OR");
                    Console.WriteLine("BeatmapAssetMaker --patch pathToBeatSaberBinary (i.e. libil2spp.so)");
                    return;
                }

                string inputAssetsFile = args[0];
                string outputAssetsFolder = args[1];
                string customSongsFolder = args[2];
                bool moxie = true;
                if (args.Length > 3)
                {
                    for (int i = 3; i < args.Length; i++)
                    {
                        if (args[i].ToLower() == "covers")
                            IMPORT_COVERS = true;
                        if (args[i].ToLower() == "nomoxie")
                            moxie = false;
                    }
                }

                //string inputAssetsFile = @"C:\Users\VR\Desktop\platform-tools_r28.0.3-windows\aaoriginalbase\assets\bin\Data\sharedassets17.assets"; ;
                //string outputAssetsFile = @"C:\Users\VR\Desktop\platform-tools_r28.0.3-windows\dist\assets\sharedassets17.assets";
                //string customSongsFolder = @"C:\Users\VR\Desktop\platform-tools_r28.0.3-windows\dist\ToConvert";

                if (!Directory.Exists(outputAssetsFolder))
                {
                    Console.WriteLine("outputAssetsFile directory doesn't exist!");
                    Environment.ExitCode = -1;
                    return;
                }

                List<string> customSongsFolders = new List<string>();

                if (File.Exists(Path.Combine(customSongsFolder, "Info.dat")))
                {
                    //do one
                    Console.WriteLine("Found Info.dat in customSongsFolder, injecting one custom song.");
                    customSongsFolders.Add(customSongsFolder);
                }
                else
                {
                    //do many
                    List<string> foundSongs = Directory.EnumerateDirectories(customSongsFolder).Where(y => File.Exists(Path.Combine(y, "Info.dat"))).ToList();
                    Console.WriteLine($"Found {foundSongs.Count()} custom songs to inject:");
                    customSongsFolders.AddRange(foundSongs);
                }
                if (customSongsFolders.Count < 1)
                {
                    Console.WriteLine("No custom songs found!");
                    Environment.ExitCode = -1;
                    return;
                }



                string fileName17 = Path.Combine(inputAssetsFile, "sharedassets17.assets");
                if (!File.Exists(fileName17))
                    fileName17 += ".split0";

                if (!File.Exists(fileName17))
                {
                    Console.WriteLine("Couldn't find sharedassets17.assets(.split0) in the assetsFolder.");
                    Environment.ExitCode = -1;
                    return;
                }

                string fileName19 = Path.Combine(inputAssetsFile, "sharedassets19.assets");
                if (!File.Exists(fileName19))
                    fileName19 += ".split0";

                if (!File.Exists(fileName19))
                {
                    Console.WriteLine("Couldn't find sharedassets19.assets(.split0) in the assetsFolder.");
                    Environment.ExitCode = -1;
                    return;
                }

                Dictionary<Guid, Type> scriptHashToTypes = new Dictionary<Guid, Type>();
                scriptHashToTypes.Add(AssetsConstants.BeatmapLevelPackScriptHash, typeof(BeatSaber.AssetsBeatmapLevelPackObject));
                scriptHashToTypes.Add(AssetsConstants.BeatmapLevelCollectionScriptHash, typeof(BeatSaber.AssetsBeatmapLevelCollectionObject));
                scriptHashToTypes.Add(new Guid("8398a1c6-7d3b-cc41-e8d7-83cd6a11bfd4"), typeof(BeatSaber.AssetsMainLevelPackCollection));
                AssetsFile f = new AssetsFile(fileName17, scriptHashToTypes);

                var levelCollection = f.Objects.FirstOrDefault(x => x is AssetsBeatmapLevelCollectionObject && ((AssetsBeatmapLevelCollectionObject)x).Name == "CustomSongsLevelPackCollection") as AssetsBeatmapLevelCollectionObject;

                if (levelCollection == null)
                {
                    levelCollection = new BeatSaber.AssetsBeatmapLevelCollectionObject(new AssetsObjectInfo()
                    {

                        TypeIndex = f.GetTypeIndexFromScriptHash(AssetsConstants.BeatmapLevelCollectionScriptHash)
                    })
                    {
                        MonoscriptTypePtr = AssetsConstants.BeatmapLevelCollectionTypePtr,
                        Enabled = 1,
                        Name = "CustomSongsLevelPackCollection"
                    };
                    f.AddObject(levelCollection, true);
                }

                foreach (var customSongFolder in customSongsFolders)
                {
                    try
                    {
                        var levelPtr = MakeAssets(customSongFolder, f, outputAssetsFolder);
                        if (levelPtr == null)
                            continue;
                        levelCollection.BeatmapLevels.Add(levelPtr);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error injecting {0}: {1}, {2}", customSongFolder, ex.Message, ex.StackTrace);
                        Environment.ExitCode = -1;
                        return;
                    }
                }
                var levelPack = f.Objects.FirstOrDefault(x => x is AssetsBeatmapLevelPackObject && ((AssetsBeatmapLevelPackObject)x).Name == "CustomSongsLevelPack") as AssetsBeatmapLevelPackObject;
                if (levelPack == null) {
                    levelPack = new BeatSaber.AssetsBeatmapLevelPackObject(new AssetsObjectInfo()
                    {
                        TypeIndex = f.GetTypeIndexFromScriptHash(AssetsConstants.BeatmapLevelPackScriptHash)
                    })
                    {
                        MonoscriptTypePtr = AssetsConstants.BeatmapLevelPackScriptPtr,
                        CoverImage = moxie?LoadPackCover(f):new AssetsPtr(0, 45),
                        Enabled = 1,
                        GameObjectPtr = new AssetsPtr(),
                        IsPackAlwaysOwned = true,
                        PackID = "CustomSongs",
                        Name = "CustomSongsLevelPack",
                        PackName = "Custom Songs",
                        BeatmapLevelCollection = levelCollection.ObjectInfo.LocalPtrTo
                    };
                    f.AddObject(levelPack, true);
                }
                levelPack.BeatmapLevelCollection = levelCollection.ObjectInfo.LocalPtrTo;
                AssetsFile file19 = new AssetsFile(fileName19, scriptHashToTypes);


                var extFile = file19.Metadata.ExternalFiles.First(x => x.FileName == "sharedassets17.assets");
                var fileIndex = file19.Metadata.ExternalFiles.IndexOf(extFile) + 1;
                var mainLevelPack = file19.Objects.First(x => x is BeatSaber.AssetsMainLevelPackCollection) as BeatSaber.AssetsMainLevelPackCollection;
                mainLevelPack.BeatmapLevelPacks.Add(new AssetsPtr(fileIndex, levelPack.ObjectInfo.ObjectID));

                try
                {
                    f.Write(Path.Combine(outputAssetsFolder, "sharedassets17.assets"));
                    file19.Write(Path.Combine(outputAssetsFolder, "sharedassets19.assets"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error writing output files: {0} {1}", ex.Message, ex.StackTrace);
                    Environment.ExitCode = -1;
                    return;
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went horribly wrong: {0} {1}", ex.Message, ex.StackTrace);
                Environment.ExitCode = -1;
                return;
            }
            Environment.ExitCode = 0;
            return;
        }
    }
}
