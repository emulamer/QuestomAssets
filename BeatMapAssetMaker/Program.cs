using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using BeatmapAssetMaker.AssetsChanger;
using BeatmapAssetMaker.BeatSaber;
using System.Diagnostics;
using Emulamer.Utils;

namespace BeatmapAssetMaker
{
    class Program
    {
        static bool IMPORT_COVERS = false;

        const string ASSETS_ROOT_PATH = "assets/bin/Data/";


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

        static bool Patch(Apkifier apk)
        {
            string binaryFile = "lib/armeabi-v7a/libil2cpp.so";
            if (!apk.FileExists(binaryFile))
            {
                Console.WriteLine("Binary file to patch doesn't exist in the APK!");
                return false;
            }
            byte[] binaryBytes = apk.Read(binaryFile);
            if (binaryBytes.Length != 27041992)
            {
                Console.WriteLine("Binary file to patch is the wrong length!");
                return false;
            }
            Console.WriteLine("Patching binary...");
            using (MemoryStream msBinary = new MemoryStream(binaryBytes))
            {
                msBinary.Seek(0x0109D074, SeekOrigin.Begin);
                byte[] readVals = new byte[4];
                msBinary.Read(readVals, 0, 4);
                if (readVals[0] != 0x8B || readVals[1] != 0xD8 || readVals[2] != 0xFE || readVals[3] != 0xEB)
                {
                    if (readVals[0] == 0x01 && readVals[1] == 0x00 && readVals[2] == 0xA0 && readVals[3] == 0xE3)
                    {
                        Console.WriteLine("It already appears to be patched.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Can't patch this binary, the code at the patch location doesn't look familiar...");
                        return false;
                    }
                }
                msBinary.Seek(0x0109D074, SeekOrigin.Begin);
                msBinary.Write(new byte[] { 0x01, 0x00, 0xA0, 0xE3 }, 0, 4);
                msBinary.Seek(0, SeekOrigin.Begin);
                byte[] binaryOutData = msBinary.ToArray();
                apk.Write(msBinary, binaryFile, true, true);
            }
            Console.WriteLine("Done patching binary!");
            return true;
        }

        static MemoryStream ReadCombinedAssets(Apkifier apk, string assetsFilePath)
        {
            List<string> assetFiles = new List<string>();
            if (assetsFilePath.ToLower().EndsWith("split0"))
            {
                assetFiles.AddRange(apk.FindFiles(assetsFilePath.Replace(".split0", ".split*"))
                    .OrderBy(x => Convert.ToInt32(x.Split(new string[] { ".split" }, StringSplitOptions.None).Last())));
            }
            else
            {
                assetFiles.Add(assetsFilePath);
            }
            MemoryStream msFullFile = new MemoryStream();
            foreach (string assetsFile in assetFiles)
            {
                byte[] fileBytes = apk.Read(assetsFile);
                msFullFile.Write(fileBytes, 0, fileBytes.Length);
            }

            return msFullFile;
        }

        static void Main(string[] args)
        {

            bool covers = true;
            bool patch = true;



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

                if (args == null || args.Length < 2)
                {
                    Console.WriteLine("Usage: BeatmapAssetMaker apkFile customSongsFolder [nocovers] [nopatch]");
                    Console.WriteLine("\tapkFile: the path to the APK.");
                    Console.WriteLine("\tcustomSongsFolder: the folder that contains folders of custom songs in the new native beatsaber format, or a folder with a single song that contains an Info.dat file");
                    Console.WriteLine("\tIf you want to skip importing covers then add \"nocovers\" at the end.");
                    Console.WriteLine("\tIf you want to skip patching the binary to allow custom songs, add \"nopatch\" at the end.");
                    return;
                }

                string apkFile = args[0];
                string customSongsFolder = args[1];
                bool moxie = true;
                if (args.Length > 2)
                {
                    for (int i = 2; i < args.Length; i++)
                    {
                        if (args[i].ToLower() == "nocovers")
                            covers = false;
                        if (args[i].ToLower() == "nopatch")
                            patch = false;
                    }
                }

                if (!File.Exists(apkFile))
                {
                    Console.WriteLine("apkFile doesn't exist!");
                    Environment.ExitCode = -1;
                    return;
                }
                if (!Directory.Exists(customSongsFolder))
                {
                    Console.WriteLine("customSongsFolder doesn't exist!");
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
                List<Tuple<string, string>> fileFromTo = new List<Tuple<string, string>>();
                Console.Write("Opening APK...");
                using (Apkifier apk = new Apkifier(apkFile))
                {
                    Console.WriteLine("..opened!");                   

                    string fileName17 = ASSETS_ROOT_PATH + "sharedassets17.assets";
                    if (!apk.FileExists(fileName17))
                        fileName17 += ".split0";

                    if (!apk.FileExists(fileName17))
                    {
                        Console.WriteLine("Couldn't find sharedassets17.assets(.split0) in the APK.");
                        Environment.ExitCode = -1;
                        return;
                    }

                    string fileName19 = ASSETS_ROOT_PATH + "sharedassets19.assets";
                    if (!apk.FileExists(fileName19))
                        fileName19 += ".split0";

                    if (!apk.FileExists(fileName19))
                    {
                        Console.WriteLine("Couldn't find sharedassets19.assets(.split0) in the APK.");
                        Environment.ExitCode = -1;
                        return;
                    }

                    Dictionary<Guid, Type> scriptHashToTypes = new Dictionary<Guid, Type>();
                    scriptHashToTypes.Add(AssetsConstants.ScriptHash.BeatmapLevelPackScriptHash, typeof(BeatSaber.BeatmapLevelPackObject));
                    scriptHashToTypes.Add(AssetsConstants.ScriptHash.BeatmapLevelCollectionScriptHash, typeof(BeatSaber.BeatmapLevelCollectionObject));
                    scriptHashToTypes.Add(AssetsConstants.ScriptHash.MainLevelsCollectionHash, typeof(BeatSaber.MainLevelPackCollectionObject));
                    scriptHashToTypes.Add(AssetsConstants.ScriptHash.BeatmapDataHash, typeof(BeatmapDataObject));
                    scriptHashToTypes.Add(AssetsConstants.ScriptHash.BeatmapLevelDataHash, typeof(BeatmapLevelDataObject));

                    AssetsFile assetsFile17;
                    using (MemoryStream msFile17 = ReadCombinedAssets(apk, fileName17))
                    {
                        assetsFile17 = new AssetsFile(msFile17, scriptHashToTypes);
                    }
                      
                    var levelCollection = assetsFile17.Objects.FirstOrDefault(x => x is BeatmapLevelCollectionObject && ((BeatmapLevelCollectionObject)x).Name == "CustomSongsLevelPackCollection") as BeatmapLevelCollectionObject;

                    if (levelCollection == null)
                    {
                        levelCollection = new BeatmapLevelCollectionObject(assetsFile17.Metadata)
                        { Name = "CustomSongsLevelPackCollection" };
                        assetsFile17.AddObject(levelCollection, true);
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
                            var level = CustomLevelLoader.LoadSongFromPathToAsset(customSongFolder, assetsFile17, covers);

                            if (level == null)
                                continue;

                            //copy audio file to the output
                            var oggSrc = Path.Combine(customSongFolder, level.SongFilename);
                            var clip = assetsFile17.Objects.First(x => x.ObjectInfo.ObjectID == level.AudioClip.PathID) as AudioClipObject;
                            fileFromTo.Add(new Tuple<string, string>(oggSrc, ASSETS_ROOT_PATH + clip.Resource.Source));

                            levelCollection.BeatmapLevels.Add(level.ObjectInfo.LocalPtrTo);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error injecting {0}: {1}, {2}", customSongFolder, ex.Message, ex.StackTrace);
                            Environment.ExitCode = -1;
                            return;
                        }
                    }
                    Console.WriteLine($"{songCount.ToString().PadLeft(5)} of {totalSongs}... (+{(int)sw.Elapsed.TotalSeconds} seconds, {String.Format("{0:n0}", GC.GetTotalMemory(false))} bytes of memory used");
                    var levelPack = assetsFile17.Objects.FirstOrDefault(x => x is BeatmapLevelPackObject && ((BeatmapLevelPackObject)x).Name == "CustomSongsLevelPack") as BeatmapLevelPackObject;
                    if (levelPack == null)
                    {
                        levelPack = new BeatSaber.BeatmapLevelPackObject(assetsFile17.Metadata)
                        {
                            CoverImage = moxie ? CustomLevelLoader.LoadPackCover(assetsFile17) : new PPtr(0, 45),
                            Enabled = 1,
                            GameObjectPtr = new PPtr(),
                            IsPackAlwaysOwned = true,
                            PackID = "CustomSongs",
                            Name = "CustomSongsLevelPack",
                            PackName = "Custom Songs",
                            BeatmapLevelCollection = levelCollection.ObjectInfo.LocalPtrTo
                        };
                        assetsFile17.AddObject(levelPack, true);
                    }
                    levelPack.BeatmapLevelCollection = levelCollection.ObjectInfo.LocalPtrTo;
                    AssetsFile assetsFile19;
                    using (MemoryStream msFile19 = ReadCombinedAssets(apk, fileName19))
                    {
                        assetsFile19 = new AssetsFile(msFile19, scriptHashToTypes);
                    }


                    var extFile = assetsFile19.Metadata.ExternalFiles.First(x => x.FileName == "sharedassets17.assets");
                    var fileIndex = assetsFile19.Metadata.ExternalFiles.IndexOf(extFile) + 1;
                    var mainLevelPack = assetsFile19.Objects.First(x => x is BeatSaber.MainLevelPackCollectionObject) as BeatSaber.MainLevelPackCollectionObject;
                    if (!mainLevelPack.BeatmapLevelPacks.Any(x => x.FileID == fileIndex && x.PathID == levelPack.ObjectInfo.ObjectID))
                        mainLevelPack.BeatmapLevelPacks.Add(new PPtr(fileIndex, levelPack.ObjectInfo.ObjectID));

                    if (fileName17.EndsWith(".split0"))
                    {
                        fileName17 = fileName17.Replace(".split0", "");
                    }
                    if (fileName19.EndsWith(".split0"))
                    {
                        fileName19 = fileName19.Replace(".split0", "");
                    }                   

                    try
                    {
                        apk.Delete(fileName17+".split*");
                        apk.Delete(fileName19+".split*");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error deleting old assets files: {0} {1}", ex.Message, ex.StackTrace);
                        Environment.ExitCode = -1;
                        return;
                    }
                    try
                    {

                        using (MemoryStream msOutputBuffer = new MemoryStream())
                        {
                            assetsFile17.Write(msOutputBuffer);
                            msOutputBuffer.Seek(0, SeekOrigin.Begin);
                            apk.Write(msOutputBuffer, fileName17, true, true);
                        }


                        using (MemoryStream msOutputBuffer = new MemoryStream())
                        {
                            assetsFile19.Write(msOutputBuffer);
                            msOutputBuffer.Seek(0, SeekOrigin.Begin);
                            apk.Write(msOutputBuffer, fileName19, true, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error writing output files: {0} {1}", ex.Message, ex.StackTrace);
                        Environment.ExitCode = -1;
                        return;
                    }
                    try
                    {
                        Console.Write("Adding ogg files...");
                        foreach (var fromTo in fileFromTo)
                        {
                            apk.Write(fromTo.Item1, fromTo.Item2, true, false);
                        }
                        Console.WriteLine("..done!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error copying ogg files: {0} {1}", ex.Message, ex.StackTrace);
                        Environment.ExitCode = -1;
                        return;
                    }

                    if (patch)
                    {
                        if (!Patch(apk))
                        {
                            Environment.ExitCode = -1;
                            return;
                        }
                    }
                    Console.Write("Closing and signing apk...");
                }
                Console.WriteLine("..done!");
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
