using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BeatmapAssetMaker.AssetsChanger;
using System.Drawing;
using System.Drawing.Imaging;
using Newtonsoft.Json.Linq;

namespace BeatmapAssetMaker
{
    class Program
    {

        // static void WriteLevelCollectionAsser(string name, string assetFileName, byte[] beatmapDataSOPtr)
        static UPtr BeatmapLevelSOType = new UPtr() { FileID = 1, PathID = 644 };
        static UPtr BeatmapDataSOType = new UPtr() { FileID = 1, PathID = 1552 };
        static UPtr BeatmapLevelCollectionSOType = new UPtr() { FileID = 1, PathID = 762 };
        static UPtr ReplaceLevelCollection = new UPtr() { FileID = 0, PathID = 240 };



        const int BEATMAPDATA_SCRIPT_ID = 0x0E;
        const int BEATMAPLEVEL_SCRIPT_ID = 0x0F;
        const int LEVELCOLLECTION_SCRIPT_ID = 0x10;

        
        

        const int ASSET_PATHID_START = 261;

        static void WriteMBHeader(AlignedStream s, string name, UPtr scriptType)
        {
            //empty gameobject pointer
            new UPtr().Write(s);
            //enabled int
            s.Write((int)1);
            //pointer to the right monoscript type
            scriptType.Write(s);
            s.Write(name);
        }
        static void WriteBeatmapLevelSOAsset(string assetName, string outputFilename, BeatmapLevelDataSO levelData)
        {
            using (FileStream f = File.Open(outputFilename, FileMode.Create))
            {
                using (MemoryStream ms = new MemoryStream(MakeBeatmapLevelSOAsset(assetName, levelData)))
                {
                    AlignedStream fs = new AlignedStream(f);
                    WriteMBHeader(fs, assetName, BeatmapLevelSOType);
                    ms.CopyTo(f);
                }
            }
        }

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
        static void WriteLevelCollectionAsset(string assetName, string outputFilename, BeatmapLevelCollectionSO levelCollection)
        {
            using (FileStream f = File.Open(outputFilename, FileMode.Create))
            {
                using (MemoryStream ms = new MemoryStream(MakeLevelCollectionAsset(assetName, levelCollection)))
                {
                    AlignedStream fs = new AlignedStream(f);
                    WriteMBHeader(fs, assetName, BeatmapLevelCollectionSOType);
                    ms.CopyTo(f);
                }
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

        static void WriteBeatmapAsset(string assetName, string outputFilename, BeatmapSaveData beatmapSaveData)
        {
            using (FileStream f = File.Open(outputFilename, FileMode.Create))
            {
                using (MemoryStream ms = new MemoryStream(MakeBeatmapAsset(assetName, beatmapSaveData)))
                {
                    AlignedStream fs = new AlignedStream(f);
                    WriteMBHeader(fs, assetName, BeatmapDataSOType);
                    ms.CopyTo(f);
                }
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


                //seems to be trailed with a zero *shrug*
                fs.AlignTo(4);
                return f.ToArray();
            }
        }

        //public static Bitmap ConvertTo24bpp(Bitmap img)
        //{
        //    var bmp = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        //    using (var gr = Graphics.FromImage(bmp))
        //        gr.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
        //    return bmp;
        //}

        static string workdir = @"C:\Users\VR\Desktop\platform-tools_r28.0.3-windows\7638-7516\assets\";
        static void MakeAssets(string inputPath, string outputPath)
        {
            int pathid = ASSET_PATHID_START;
            var sng = CustomSongLoader.LoadFromPath(inputPath);
            (sng._difficultyBeatmapSets.Where(x => x._beatmapCharacteristicName != Characteristic.Standard).ToList()).ForEach(y => sng._difficultyBeatmapSets.Remove(y));
            sng._songName = "Aeat Aaber";

            sng._environmentSceneInfo = new UPtr() { FileID = 20, PathID = 1 };
            sng._audioClip = new UPtr() { FileID = 0, PathID = 39 };
            //pathid++;
            sng._coverImageTexture2D = new UPtr() { FileID = 0, PathID = 19 };
            //pathid++;
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
                    string fName = Path.Combine(outputPath, $"{pathid}_{BEATMAPDATA_SCRIPT_ID}_{bmAssetName}.asset");
                    WriteBeatmapAsset(bmAssetName, fName, g._beatmapSaveData);
                    g._beatmapDataPtr = new UPtr() { FileID = 0, PathID = pathid };
                    pathid++;
                }
            }
            string levelAssetName = $"{sng._levelID}Level";
            string levelAssetFile = Path.Combine(outputPath, $"{pathid}_{BEATMAPLEVEL_SCRIPT_ID}_{levelAssetName}.asset");
            
            WriteBeatmapLevelSOAsset(levelAssetName, levelAssetFile, sng);
            var lc = new BeatmapLevelCollectionSO();
            lc._beatmapLevels.Add(new UPtr() { FileID = 0, PathID = 90 });
            lc._beatmapLevels.Add(new UPtr() { FileID = 0, PathID = 151 });
            lc._beatmapLevels.Add(new UPtr() { FileID = 0, PathID = 162 });
            lc._beatmapLevels.Add(new UPtr() { FileID = 0, PathID = 207 });
            lc._beatmapLevels.Add(new UPtr() { FileID = 0, PathID = pathid});
            WriteLevelCollectionAsset("OstVol2LevelCollection", Path.Combine(outputPath, $"240_{LEVELCOLLECTION_SCRIPT_ID}_OstVol2LevelCollection.asset"), lc);
        }
        public static void RGBtoBGR(Bitmap bmp)
        {
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                           ImageLockMode.ReadWrite, bmp.PixelFormat);

            int length = Math.Abs(data.Stride) * bmp.Height;

            unsafe
            {
                byte* rgbValues = (byte*)data.Scan0.ToPointer();

                for (int i = 0; i < length; i += 3)
                {
                    byte dummy = rgbValues[i];
                    rgbValues[i] = rgbValues[i + 2];
                    rgbValues[i + 2] = dummy;
                }
            }

            bmp.UnlockBits(data);
        }


        static void MakeAssets(string inputPath, AssetsFile assetsFile, BeatmapLevelCollectionSO targetLevelCollection, string outputAudioPath)
        {

            var sng = CustomSongLoader.LoadFromPath(inputPath);

            if (assetsFile.Objects.Where(x=>x is AssetsMonoBehaviourObject).Any(x=>((AssetsMonoBehaviourObject)x).Name == sng._levelID+"Level"))
            {
                Console.WriteLine("*************WARNING************");
                Console.WriteLine($"{inputPath} seems to have a duplicate level ID of {sng._levelID} as another song that's already loaded.  Unknown things may happen.  I chose a bad way to generate level ID, sorry.");
            }
                                 
            AssetsTexture2D coverAsset = null;
            string audioClipFile = Path.Combine(inputPath, sng._songFilename);
            string outputAudioClipFile = Path.Combine(outputAudioPath, sng._levelID + ".ogg");
            if (!string.IsNullOrWhiteSpace(sng._coverImageFilename) && File.Exists(sng._coverImageFilename))
            {
                try
                {
                    string coverFile = Path.Combine(inputPath, sng._coverImageFilename);
                    Image coverImage = Image.FromFile(coverFile);
                    byte[] imageBytes;
                    using (MemoryStream msCover = new MemoryStream())
                    {
                        coverImage.Save(msCover, System.Drawing.Imaging.ImageFormat.Bmp);

                        imageBytes = new byte[msCover.Length - 54];
                        byte[] msBytes = msCover.ToArray();
                        Array.Copy(msBytes, 54, imageBytes, 0, imageBytes.Length);
                    }
                    for (int i = 0; i < imageBytes.Length; i += 3)
                    {
                        byte hold = imageBytes[i];
                        imageBytes[i] = imageBytes[i + 2];
                        imageBytes[i + 2] = hold;
                    }

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
            var v = new NVorbis.VorbisReader(audioClipFile);
            var audioClip = new AssetsAudioClip(new AssetsObjectInfo()
            {
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
                        MonoscriptTypePtr = new AssetsPtr(BeatmapDataSOType),
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
                TypeIndex = AssetsConstants.BeatmapLevelTypeIndex
            })
            {
                MonoscriptTypePtr = new AssetsPtr(BeatmapLevelSOType),
                Name = levelAssetName,
                ScriptParametersData = bmLevelData,
                GameObjectPtr = new AssetsPtr()
            };
            assetsFile.AddObject(bmLevelAsset, true);

            targetLevelCollection._beatmapLevels.Add(bmLevelAsset.ObjectInfo.LocalPtrTo.ToUPtr());

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
                //if (args == null || args.Length < 3)
                //{
                //    Console.WriteLine("Usage: BeatmapAssetMaker assetsFile outputAssetsFile customSongsFolder");
                //    Console.WriteLine("\tassetsFile: the name/path to the assets file.  If the assets file ends with .split0, send in the filename without the .split0, e.g. c:\\files\\assets\\Data\\sharedassets17.assets.split0 should be passed as c:\\files\\assets\\Data\\sharedassets17.assets");
                //    Console.WriteLine("\toutputAssetsFile: the filename of the output assets file (also don't include .split0)");
                //    Console.WriteLine("\tcustomSongsFolder: the folder that contains folders of custom songs in the new native beatsaber format, or a folder with a single song that contains an Info.dat file");
                //    return;
                //}

                //string inputAssetsFile = args[0];
                //string outputAssetsFile = args[1];
                //string customSongsFolder = args[2];

                string inputAssetsFile = @"C:\Users\VR\Desktop\platform-tools_r28.0.3-windows\aaoriginalbase\assets\bin\Data\sharedassets17.assets"; ;
                string outputAssetsFile = @"C:\Users\VR\Desktop\platform-tools_r28.0.3-windows\dist\assets\sharedassets17.assets";
                string customSongsFolder = @"C:\Users\VR\Desktop\platform-tools_r28.0.3-windows\dist\ToConvert";

                if (!Directory.Exists(Path.GetDirectoryName(outputAssetsFile)))
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
                    Console.WriteLine($"Found {foundSongs} custom songs to inject:");
                    customSongsFolders.AddRange(foundSongs);
                }
                if (customSongsFolders.Count < 1)
                {
                    Console.WriteLine("No custom songs found!");
                    Environment.ExitCode = -1;
                    return;
                }
                Dictionary<Guid, Type> scriptHashToTypes = new Dictionary<Guid, Type>();

                string fileName = inputAssetsFile;// @"C:\Users\VR\Desktop\platform-tools_r28.0.3-windows\aaoriginalbase\assets\bin\Data\sharedassets17.assets";
                if (!File.Exists(fileName))
                    fileName += ".split0";

                if (!File.Exists(fileName))
                {
                    Console.WriteLine("Couldn't find the assetsFile (or it's .split0)!");
                    Environment.ExitCode = -1;
                    return;
                }
                AssetsFile f = new AssetsFile(fileName, scriptHashToTypes);

                //TODO: load in the existing one and append isntead of overwriting
                var levelCollection = new BeatmapLevelCollectionSO();
                levelCollection._beatmapLevels.Add(new UPtr() { FileID = 0, PathID = 90 });
                levelCollection._beatmapLevels.Add(new UPtr() { FileID = 0, PathID = 151 });
                levelCollection._beatmapLevels.Add(new UPtr() { FileID = 0, PathID = 162 });
                levelCollection._beatmapLevels.Add(new UPtr() { FileID = 0, PathID = 207 });
                levelCollection._beatmapLevels.Add(new UPtr() { FileID = 0, PathID = 229 });
                
                

                foreach (var customSongFolder in customSongsFolders)
                {
                    try
                    {
                        MakeAssets(customSongFolder, f, levelCollection, Path.GetDirectoryName(outputAssetsFile));
                    } catch (Exception ex)
                    {
                        Console.WriteLine("Error injecting {0}: {1}, {2}", customSongFolder, ex.Message, ex.StackTrace);
                        Environment.ExitCode = -1;
                        return;
                    }
                }

                byte[] levelColData = MakeLevelCollectionAsset("OstVol2LevelCollection", levelCollection);
                var ostObj = ((AssetsMonoBehaviourObject)f.Objects.First(x => x is AssetsMonoBehaviourObject && ((AssetsMonoBehaviourObject)x).Name == "OstVol2LevelCollection"));
                ostObj.ScriptParametersData = levelColData;

                try
                {
                    f.Write(outputAssetsFile);
                } catch (Exception ex)
                {
                    Console.WriteLine("Error writing output file: {0} {1}", ex.Message, ex.StackTrace);
                }
            } catch (Exception ex)
            {
                Console.WriteLine("Something went horribly wrong: {0} {1}", ex.Message, ex.StackTrace);
                Environment.ExitCode = -1;
                return;
            }
            return;
            //byte[] b;
            //using (FileStream f = File.OpenRead(@"C:\Users\VR\Desktop\platform-tools_r28.0.3-windows\7638-7516\assets\BeatSaberExpertPlusBeatmapData.asset"))
            //{
            //    var offset = 204;
            //    f.Seek(offset, SeekOrigin.Begin);
            //    var len = f.Length - offset;
            //    b = new byte[len];
            //    f.Read(b, 0, (int)len);
            //}

            //BeatmapSaveData beatmapSaveData = BeatmapSaveData.DeserializeFromFromBinary(b);
            //WriteBeatmapAsset("BeatSaberExpertPlusBeatmapData", @"C:\Users\VR\Desktop\platform-tools_r28.0.3-windows\7638-7516\assets\easy.out", beatmapSaveData);


            MakeAssets(@"C:\Program Files (x86)\Steam\steamapps\common\Beat Saber\Beat Saber_Data\CustomLevels\Jaroslav Beck - Beat Saber (Built in)",
                @"C:\Users\VR\Desktop\platform-tools_r28.0.3-windows\7638-7516\assets");
            
            return;
            if (System.Configuration.ConfigurationManager.AppSettings["BeapMapDataSOUnityPointer"] == null) {
                Console.WriteLine("BeapMapDataSOUnityPointer must be set to the pointer of the BeatMapDataSO object in the .config file.");
                return;
            }
            var splitBytes = System.Configuration.ConfigurationManager.AppSettings["BeapMapDataSOUnityPointer"].Split(' ');
            bool ptrErr = false;
            byte[] beatmapSOPtr = null;
            if (splitBytes.Count() != 12)
            {
                ptrErr = true;
            } else
            {
                try
                {
                    beatmapSOPtr = splitBytes.Select(x => Convert.ToByte(x, 16)).ToArray();
                }
                catch
                {
                    ptrErr = true;
                }
            }
            if (ptrErr)
            { 
                Console.WriteLine("BeapMapDataSOUnityPointer in the .config file must be 12 space-separated hex encoded bytes.");
                return;
            }

    
            if (args == null || args.Length < 3)
            {
                Console.WriteLine("Usage: BeatmapAssetMaker assetname datfile assetfile");
                Console.WriteLine("\tassetname: name of the asset (e.g. EscapeExpertPlusBeatmapData)");
                Console.WriteLine("\tdatfile: filename of the json input dat file of the beatmap (e.g. Easy.dat)");
                Console.WriteLine("\tassetfile: filename that the unity asset will be written to");
                
                return;
            }
            string datfile = args[1];
            string assetfile = args[2];
            string assetname = args[0];

            if (!File.Exists(datfile))
            {
                Console.WriteLine("Input datfile does not exist!");
                return;
            }
            if (!Directory.Exists(Path.GetDirectoryName(assetfile)))
            {
                Console.WriteLine("Output assetfile directory does not exist!");
                return;
            }


            BeatmapSaveData bmd = null;
            string json;
            try
            {

                using (StreamReader sr = new StreamReader(datfile))
                {
                    json = sr.ReadToEnd();
                }
            } catch (Exception ex)
            {
                Console.WriteLine("Error opening datfile: " + ex.Message + ", " + ex.StackTrace.ToString());
                return;
            }
            try
            {                    
                bmd = BeatmapDataLoader.GetBeatmapSaveDataFromJson(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deserializing BeatmapSaveData:" + ex.Message + ", " + ex.StackTrace.ToString());
                return;
            }

            //float timeMod = 1.088F;

            //foreach (var v in bmd.notes)
            //{
            //    v.time = v.time * timeMod;
            //}
            //foreach (var v in bmd.events)
            //{
            //    v.time = v.time * timeMod;
            //}
            //foreach (var v in bmd.obstacles)
            //{
            //    v.time = v.time * timeMod;
            //}
            //this outputs a fake asset for the beatmap
            try
            {

                //WriteBeatmapAsset(assetname, assetfile, beatmapSOPtr, bmd);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating asset: " + ex.Message + ", " + ex.StackTrace.ToString());
                return;
            }

            
            /*
            byte[] b;
            using (FileStream f = File.OpenRead(@"C:\Users\VR\Desktop\platform-tools_r28.0.3-windows\easy.raw.dat"))
            {
                var offset =  196;
                f.Seek(offset, SeekOrigin.Begin);
                var len = f.Length - offset;
                b = new byte[len];
                f.Read(b, 0, (int)len);
            }
            //var bin = BeatmapDataLoader.GetBeatmapDataFromBinary(b, 166, 0, 1);
            BeatmapSaveData beatmapSaveData = BeatmapSaveData.DeserializeFromFromBinary(b);
            WriteAsset("EscapeExpertPlusBeatmapData", @"C:\Users\VR\Desktop\platform-tools_r28.0.3-windows\7638-7516\Good Life\easy.out", beatmapSaveData);
            */

        }
    }
}
