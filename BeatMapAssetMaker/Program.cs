using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using BeatmapAssetMaker.AssetsChanger;
using System.Drawing;
using BeatmapAssetMaker.BeatSaber;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace BeatmapAssetMaker
{
    class Program
    {
        static bool IMPORT_COVERS = false;




        /// <summary>
        /// Makes an asseet 
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="outputPath"></param>
        /// <returns>the UPtr to the BeatSaberLevel</returns>
        //static AssetsPtr MakeAssets(string inputPath, AssetsFile assetsFile, string outputAudioPath)
        //{

        //    var sng = BeatmapLevelData.LoadFromPath(inputPath);

        //    if (assetsFile.Objects.Where(x=>x is AssetsMonoBehaviourObject).Any(x=>((AssetsMonoBehaviourObject)x).Name == sng.LevelID+"Level"))
        //    {
        //        Console.WriteLine($"{inputPath} seems to have a duplicate level ID of {sng.LevelID} as another song that's already loaded.  Skipping it.");
        //        return null;
        //    }

        //    AssetsTexture2D coverAsset = null;
        //    string audioClipFile = Path.Combine(inputPath, sng.SongFilename);
        //    string outputAudioClipFile = Path.Combine(outputAudioPath, sng.LevelID + ".ogg");
        //    if (IMPORT_COVERS)
        //    {
        //        if (!string.IsNullOrWhiteSpace(sng.CoverImageFilename) && File.Exists(Path.Combine(inputPath, sng.CoverImageFilename)))
        //        {
        //            try
        //            {
        //                string coverFile = Path.Combine(inputPath, sng.CoverImageFilename);
        //                Bitmap coverImage = (Bitmap)Bitmap.FromFile(coverFile);
        //                var imageBytes = LoadToRGB(coverImage);

        //                coverAsset = new AssetsTexture2D(assetsFile.Metadata)
        //                {
        //                    Name = sng.LevelID + "Cover",
        //                    ForcedFallbackFormat = 4,
        //                    DownscaleFallback = false,
        //                    Width = coverImage.Width,
        //                    Height = coverImage.Height,
        //                    CompleteImageSize = imageBytes.Length,
        //                    TextureFormat = 3,
        //                    MipCount = 1,
        //                    IsReadable = false,
        //                    StreamingMipmaps = false,
        //                    StreamingMipmapsPriority = 0,
        //                    ImageCount = 1,
        //                    TextureDimension = 2,
        //                    TextureSettings = new GLTextureSettings()
        //                    {
        //                        FilterMode = 2,
        //                        Aniso = 1,
        //                        MipBias = -1,
        //                        WrapU = 1,
        //                        WrapV = 1,
        //                        WrapW = 0
        //                    },
        //                    LightmapFormat = 6,
        //                    ColorSpace = 1,
        //                    ImageData = imageBytes,
        //                    StreamData = new StreamingInfo()
        //                    {
        //                        offset = 0,
        //                        size = 0,
        //                        path = ""
        //                    }
        //                };
        //                assetsFile.AddObject(coverAsset, true);
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine("Error loading cover art asset: {0} {1}", ex.Message, ex.StackTrace);
        //            }
        //        }
        //        else
        //        {
        //            Console.WriteLine("No cover art found.");
        //        }
        //    }
        //    int channels;
        //    int frequency;
        //    Single length;
        //    unsafe
        //    {
        //        byte[] oggBytes = File.ReadAllBytes(audioClipFile);
        //        GCHandle pinnedArray = GCHandle.Alloc(oggBytes, GCHandleType.Pinned);
        //        try
        //        {
        //            IntPtr pointer = pinnedArray.AddrOfPinnedObject();
        //            int error;
        //            StbSharp.StbVorbis.stb_vorbis_alloc alloc;
        //            StbSharp.StbVorbis.stb_vorbis v = StbSharp.StbVorbis.stb_vorbis_open_memory((byte*)pointer.ToPointer(), oggBytes.Length, &error, &alloc);
        //            channels = v.channels;
        //            frequency = (int)v.sample_rate;
        //            length = StbSharp.StbVorbis.stb_vorbis_stream_length_in_seconds(v);
        //            StbSharp.StbVorbis.stb_vorbis_close(v);
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Exception parsing ogg file {audioClipFile}: {0} {1}", ex.Message, ex.StackTrace);
        //            return null;
        //        }
        //        finally
        //        {
        //            pinnedArray.Free();
        //        }
        //    }


        //    var audioClip = new AssetsAudioClip(assetsFile.Metadata)
        //    {
        //        Name = sng.LevelID,
        //        LoadType = 1,
        //        IsTrackerFormat = false,
        //        Ambisonic = false,
        //        SubsoundIndex = 0,
        //        PreloadAudioData = false,
        //        LoadInBackground = true,
        //        Legacy3D = true,
        //        CompressionFormat = 1,
        //        BitsPerSample = 16,
        //        Channels = channels,
        //        Frequency = frequency,
        //        Length = (Single)length,
        //        Resource = new StreamedResource(Path.GetFileName(outputAudioClipFile), 0, Convert.ToUInt64(new FileInfo(audioClipFile).Length))
        //    };
        //    assetsFile.AddObject(audioClip, true);
        //    //todo: move elsewhere
        //    File.Copy(audioClipFile, outputAudioClipFile, true);

        //    sng.EnvironmentSceneInfo = AssetsConstants.KnownObjects.DefaultEnvironment;
        //    sng.AudioClip = audioClip.ObjectInfo.LocalPtrTo;

        //    //temporarily, use beatsaber's cover art if one couldn't be loaded
        //    if (coverAsset == null)
        //        sng.CoverImageTexture2D = AssetsConstants.KnownObjects.BeatSaberCoverArt;
        //    else
        //        sng.CoverImageTexture2D = coverAsset.ObjectInfo.LocalPtrTo;

        //    foreach (var s in sng.DifficultyBeatmapSets)
        //    {
        //        switch (s.BeatmapCharacteristicName)
        //        {
        //            case Characteristic.OneSaber:
        //                s.BeatmapCharacteristic = AssetsConstants.KnownObjects.OneSaberCharacteristic;
        //                break;
        //            case Characteristic.NoArrows:
        //                s.BeatmapCharacteristic = AssetsConstants.KnownObjects.NoArrowsCharacteristic;
        //                break;
        //            case Characteristic.Standard:
        //                s.BeatmapCharacteristic = AssetsConstants.KnownObjects.StandardCharacteristic;
        //                break;
        //        }

        //        foreach (var g in s.DifficultyBeatmaps)
        //        {

        //            string bmAssetName = sng.LevelID + ((s.BeatmapCharacteristicName == Characteristic.Standard) ? "" : s.BeatmapCharacteristicName.ToString()) + g.Difficulty.ToString() + "BeatmapData";

        //            byte[] assetData = MakeBeatmapAsset(bmAssetName, g.BeatmapSaveData);

        //            AssetsMonoBehaviourObject bmAsset = new AssetsMonoBehaviourObject(new AssetsObjectInfo()
        //            {
        //                TypeIndex = AssetsConstants.BeatmapDataSOTypeIndex
        //            })
        //            {
        //                MonoscriptTypePtr = AssetsConstants.BeatmapDataTypePtr,
        //                Name = bmAssetName,
        //                ScriptParametersData = assetData,
        //                GameObjectPtr = new AssetsPtr()
        //            };
        //            assetsFile.AddObject(bmAsset, true);
        //            g._beatmapDataPtr = new UPtr() { FileID = 0, PathID = bmAsset.ObjectInfo.ObjectID };
        //        }
        //    }

        //    string levelAssetName = $"{sng._levelID}Level";


        //    byte[] bmLevelData = MakeBeatmapLevelSOAsset(levelAssetName, sng);
        //    AssetsMonoBehaviourObject bmLevelAsset = new AssetsMonoBehaviourObject(new AssetsObjectInfo()
        //    {
        //        //todo: make these hash lookups and not by index
        //        TypeIndex = AssetsConstants.BeatmapLevelTypeIndex
        //    })
        //    {
        //        MonoscriptTypePtr = AssetsConstants.ScriptPtr.BeatmapLevelCo.BeatmapLevelTypePtr,
        //        Name = levelAssetName,
        //        ScriptParametersData = bmLevelData,
        //        GameObjectPtr = new AssetsPtr()
        //    };
        //    assetsFile.AddObject(bmLevelAsset, true);

        //    return bmLevelAsset.ObjectInfo.LocalPtrTo;

        //}

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
                scriptHashToTypes.Add(AssetsConstants.ScriptHash.BeatmapLevelPackScriptHash, typeof(BeatSaber.AssetsBeatmapLevelPackObject));
                scriptHashToTypes.Add(AssetsConstants.ScriptHash.BeatmapLevelCollectionScriptHash, typeof(BeatSaber.AssetsBeatmapLevelCollectionObject));
                scriptHashToTypes.Add(AssetsConstants.ScriptHash.MainLevelsCollectionHash, typeof(BeatSaber.AssetsMainLevelPackCollection));
                scriptHashToTypes.Add(AssetsConstants.ScriptHash.BeatmapDataHash, typeof(BeatmapData));
                scriptHashToTypes.Add(AssetsConstants.ScriptHash.BeatmapLevelDataHash, typeof(BeatmapLevelData));


                AssetsFile assetsFile = new AssetsFile(fileName17, scriptHashToTypes);



                var levelCollection = assetsFile.Objects.FirstOrDefault(x => x is AssetsBeatmapLevelCollectionObject && ((AssetsBeatmapLevelCollectionObject)x).Name == "CustomSongsLevelPackCollection") as AssetsBeatmapLevelCollectionObject;

                if (levelCollection == null)
                {
                    levelCollection = new AssetsBeatmapLevelCollectionObject(assetsFile.Metadata)
                    { Name = "CustomSongsLevelPackCollection" };
                    assetsFile.AddObject(levelCollection, true);
                }
                int totalSongs = customSongsFolders.Count;
                Console.WriteLine($"Found {totalSongs} custom song folders to convert...");
                Stopwatch sw = new Stopwatch();
                sw.Start();
                int songCount = 0;

                foreach (var customSongFolder in customSongsFolders)
                {
                    songCount++;
                    if (songCount % 20 == 0)
                        Console.WriteLine($"{songCount.ToString().PadLeft(5)} of {totalSongs}... (+{(int)sw.Elapsed.TotalSeconds} seconds, {String.Format("{0:n0}", GC.GetTotalMemory(false))} bytes of memory used");

                    try
                    {
                        var level = CustomLevelLoader.LoadSongFromPathToAsset(customSongFolder, assetsFile);

                        if (level == null)
                            continue;

                        //copy audio file to the output
                        var oggSrc = Path.Combine(customSongFolder, level.SongFilename);
                        var clip = assetsFile.Objects.First(x => x.ObjectInfo.ObjectID == level.AudioClip.PathID) as AssetsAudioClip;
                        var oggDst = Path.Combine(outputAssetsFolder, clip.Resource.Source);
                        File.Copy(oggSrc, oggDst, true);

                        levelCollection.BeatmapLevels.Add(level.ObjectInfo.LocalPtrTo);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error injecting {0}: {1}, {2}", customSongFolder, ex.Message, ex.StackTrace);
                        Environment.ExitCode = -1;
                        return;
                    }
                }
                var levelPack = assetsFile.Objects.FirstOrDefault(x => x is AssetsBeatmapLevelPackObject && ((AssetsBeatmapLevelPackObject)x).Name == "CustomSongsLevelPack") as AssetsBeatmapLevelPackObject;
                if (levelPack == null)
                {
                    levelPack = new BeatSaber.AssetsBeatmapLevelPackObject(assetsFile.Metadata)
                    {
                        CoverImage = moxie?CustomLevelLoader.LoadPackCover(assetsFile):new AssetsPtr(0, 45),
                        Enabled = 1,
                        GameObjectPtr = new AssetsPtr(),
                        IsPackAlwaysOwned = true,
                        PackID = "CustomSongs",
                        Name = "CustomSongsLevelPack",
                        PackName = "Custom Songs",
                        BeatmapLevelCollection = levelCollection.ObjectInfo.LocalPtrTo
                    };
                    assetsFile.AddObject(levelPack, true);
                }
                levelPack.BeatmapLevelCollection = levelCollection.ObjectInfo.LocalPtrTo;
                AssetsFile file19 = new AssetsFile(fileName19, scriptHashToTypes);


                var extFile = file19.Metadata.ExternalFiles.First(x => x.FileName == "sharedassets17.assets");
                var fileIndex = file19.Metadata.ExternalFiles.IndexOf(extFile) + 1;
                var mainLevelPack = file19.Objects.First(x => x is BeatSaber.AssetsMainLevelPackCollection) as BeatSaber.AssetsMainLevelPackCollection;
                if (!mainLevelPack.BeatmapLevelPacks.Any(x => x.FileID == fileIndex && x.PathID == levelPack.ObjectInfo.ObjectID))
                    mainLevelPack.BeatmapLevelPacks.Add(new AssetsPtr(fileIndex, levelPack.ObjectInfo.ObjectID));

                try
                {
                    assetsFile.Write(Path.Combine(outputAssetsFolder, "sharedassets17.assets"));
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
