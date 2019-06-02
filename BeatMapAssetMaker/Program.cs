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
        const string ASSETS_ROOT_PATH = "assets/bin/Data/";

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
        static void Usage()
        {
            Console.WriteLine("Usage: BeatmapAssetMaker [-nocovers] [-nopatch] <apkFile> <customSongsFolder> [pemFile]");
           // Console.WriteLine("or     BeatmapAssetMaker patchasset <apkFile> <apkAssetName> <pathIDToReplace> <newAssetDataFile> [pemFile]");
            Console.WriteLine("\tapkFile: the path to the APK.");
            Console.WriteLine("\tcustomSongsFolder: the folder that contains folders of custom songs in the new native beatsaber format, or a folder with a single song that contains an Info.dat file");
            Console.WriteLine("\tpemFile: the name of the PEM certificate file to load or create.  If not specified, BS.pem will be used as a default.");
            Console.WriteLine("\tIf you want to skip importing covers then add \"nocovers\" at the end.");
            Console.WriteLine("\tIf you want to skip patching the binary to allow custom songs, add \"nopatch\" at the end.");
            return;
        }

        static void Main(string[] args)
        {

            //using (FileStream fs = File.Open(@"C:\Program Files (x86)\Steam\steamapps\common\Beat Saber\CustomSabers\Neo Katanas.saber", FileMode.Open, FileAccess.Read))
            //{
            //    BundleFile u = new BundleFile(fs);

            //    AssetsFile file = new AssetsFile(u.Entries[0].Data.ToStream(), MiscUtils.GetKnownAssetTypes());
            //}
            //return;


            Stopwatch sw = new Stopwatch();

            bool covers = true;
            bool patch = true;

            try
            {
                if (args == null || args.Length < 1)
                {
                    Usage();
                    Environment.ExitCode = -1;
                    return;
                }
                List<string> argList = new List<string>(args);
                foreach (var arg in argList.Where(x=> x.StartsWith(" - ")).ToList())
                {
                    if (arg.ToLower() == "-nocovers")
                    {
                        covers = false;
                    }
                    else if (arg.ToLower() == "-nopatch")
                    {
                        patch = false;
                    }
                    else
                    {
                        Console.WriteLine($"Unknown option: {arg}");
                        Environment.ExitCode = -1;
                        return;
                    }
                    argList.Remove(arg);
                }
                 
                if (argList.Count < 2)
                {
                    Usage();
                    Environment.ExitCode = -1;
                    return;
                }

                string apkFile = argList[0];
                string customSongsFolder = argList[1];
                string pemFile = "BS.PEM";
                if (argList.Count >= 3)
                {
                    pemFile = argList[2];
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
                if (string.IsNullOrEmpty(pemFile))
                {
                    pemFile = "bs.pem";
                }
                if (Path.GetDirectoryName(pemFile) != "" && !Directory.Exists(Path.GetDirectoryName(pemFile)))
                {
                    Console.WriteLine("Directory for pemFile doesn't exist!");
                    Environment.ExitCode = -1;
                    return;
                }

                string pemData = null;

                if (File.Exists(pemFile))
                {
                    Console.WriteLine($"Using existing certificate from {pemFile}");
                    pemData = File.ReadAllText(pemFile);
                }
                else
                {
                    Console.WriteLine($"PEM file not found, one will be saved to {pemFile}");
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
                    Console.WriteLine($"Found {foundSongs.Count()} custom songs to inject");
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
                using (Apkifier apk = new Apkifier(apkFile, true, pemData))
                {
                    Console.WriteLine("..opened!");


                    do
                    {
                        ////test code start
                        string filename11 = "assets/bin/Data/sharedassets11.assets";
                        byte[] file11 = apk.Read(filename11);
                        AssetsFile assetsFile11;
                        using (var ms = file11.ToStream())
                            assetsFile11 = new AssetsFile(ms, MiscUtils.GetKnownAssetTypes());

                        var saberHandle = assetsFile11.Objects.First(x => x.ObjectInfo.ObjectID == 12);
                        var saberBlade = assetsFile11.Objects.First(x => x.ObjectInfo.ObjectID == 13);
                        var saberGlowingEdges = assetsFile11.Objects.First(x => x.ObjectInfo.ObjectID == 14);

                        saberHandle.Data = File.ReadAllBytes(@"C:\Users\VR\Desktop\platform-tools_r28.0.3-windows\dist\sabers\SaberHandle-sharedassets11.assets-12.dat");
                        saberBlade.Data = File.ReadAllBytes(@"C:\Users\VR\Desktop\platform-tools_r28.0.3-windows\dist\sabers\SaberBlade asset-sharedassets11.assets-13.dat");
                        saberGlowingEdges.Data = File.ReadAllBytes(@"C:\Users\VR\Desktop\platform-tools_r28.0.3-windows\dist\sabers\SaberGlowingEdges-sharedassets11.assets-14.dat");

                        using (var ws = apk.GetWriteStream(filename11,true))
                            assetsFile11.Write(ws);






                        break;
                        ////test code end



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

                        Dictionary<Guid, Type> scriptHashToTypes = MiscUtils.GetKnownAssetTypes();

                        sw.Start();
                        Console.Write($"Loading {fileName17}...");
                        AssetsFile assetsFile17 = null;
                        try
                        {
                            using (MemoryStream msFile17 = ReadCombinedAssets(apk, fileName17))
                            {
                                assetsFile17 = new AssetsFile(msFile17, scriptHashToTypes);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: {0} {1}", ex.Message, ex.StackTrace);
                            Environment.ExitCode = -1;
                            return;
                        }
                        Console.WriteLine($"..done! (+{ (int)sw.Elapsed.TotalSeconds} seconds, { String.Format("{0:n0}", GC.GetTotalMemory(false))} bytes of memory used)");

                        var levelCollection = assetsFile17.Objects.FirstOrDefault(x => x is BeatmapLevelCollectionObject && ((BeatmapLevelCollectionObject)x).Name == "CustomSongsLevelPackCollection") as BeatmapLevelCollectionObject;

                        if (levelCollection == null)
                        {
                            levelCollection = new BeatmapLevelCollectionObject(assetsFile17.Metadata)
                            { Name = "CustomSongsLevelPackCollection" };
                            assetsFile17.AddObject(levelCollection, true);
                        }
                        int totalSongs = customSongsFolders.Count;
                        Console.WriteLine($"Found {totalSongs} custom song folders, starting import...");

                        int songCount = 0;

                        foreach (var customSongFolder in customSongsFolders)
                        {
                            songCount++;
                            if (songCount % 20 == 0)
                                Console.WriteLine($"{songCount.ToString().PadLeft(5)} of {totalSongs}... (+{(int)sw.Elapsed.TotalSeconds} seconds, {String.Format("{0:n0}", GC.GetTotalMemory(false))} bytes of memory used)");

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
                        Console.WriteLine($"{songCount.ToString().PadLeft(5)} of {totalSongs}... (+{(int)sw.Elapsed.TotalSeconds} seconds, {String.Format("{0:n0}", GC.GetTotalMemory(false))} bytes of memory used)");
                        var levelPack = assetsFile17.Objects.FirstOrDefault(x => x is BeatmapLevelPackObject && ((BeatmapLevelPackObject)x).Name == "CustomSongsLevelPack") as BeatmapLevelPackObject;
                        if (levelPack == null)
                        {
                            var packCover = CustomLevelLoader.LoadPackCover(assetsFile17);
                            levelPack = new BeatSaber.BeatmapLevelPackObject(assetsFile17.Metadata)
                            {
                                CoverImage = packCover ?? new PPtr(0, 45),
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
                        AssetsFile assetsFile19 = null;
                        Console.WriteLine($"Loading {fileName19}...");
                        try
                        {
                            using (MemoryStream msFile19 = ReadCombinedAssets(apk, fileName19))
                            {
                                assetsFile19 = new AssetsFile(msFile19, scriptHashToTypes);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: {0} {1}", ex.Message, ex.StackTrace);
                            Environment.ExitCode = -1;
                            return;
                        }
                        Console.WriteLine($"..done! (+{ (int)sw.Elapsed.TotalSeconds} seconds, { String.Format("{0:n0}", GC.GetTotalMemory(false))} bytes of memory used)");


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
                        Console.Write("Removing old .assets files...");
                        try
                        {
                            apk.Delete(fileName17 + ".split*");
                            apk.Delete(fileName19 + ".split*");
                            Console.WriteLine($"..done! (+{ (int)sw.Elapsed.TotalSeconds} seconds, { String.Format("{0:n0}", GC.GetTotalMemory(false))} bytes of memory used)");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error deleting old assets files: {0} {1}", ex.Message, ex.StackTrace);
                            Environment.ExitCode = -1;
                            return;
                        }
                        Console.Write("Saving new .assets files...");
                        try
                        {
                            using (var writer = apk.GetWriteStream(fileName17, true, true))
                                assetsFile17.Write(writer);

                            using (var writer = apk.GetWriteStream(fileName19, true, true))
                                assetsFile19.Write(writer);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error writing output files: {0} {1}", ex.Message, ex.StackTrace);
                            Environment.ExitCode = -1;
                            return;
                        }
                        Console.WriteLine($"..done! (+{ (int)sw.Elapsed.TotalSeconds} seconds, { String.Format("{0:n0}", GC.GetTotalMemory(false))} bytes of memory used)");
                        try
                        {
                            Console.Write("Adding ogg files...");
                            foreach (var fromTo in fileFromTo)
                            {
                                apk.Write(fromTo.Item1, fromTo.Item2, true, false);
                            }
                            Console.WriteLine($"..done! (+{ (int)sw.Elapsed.TotalSeconds} seconds, { String.Format("{0:n0}", GC.GetTotalMemory(false))} bytes of memory used)");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error copying ogg files: {0} {1}", ex.Message, ex.StackTrace);
                            Environment.ExitCode = -1;
                            return;
                        }
                    } while (false);
                    if (patch)
                    {
                        if (!Patch(apk))
                        {
                            Environment.ExitCode = -1;
                            return;
                        }
                    }
                    try
                    {
                        if (!File.Exists(pemFile))
                        {
                            Console.WriteLine($"Writing new certificate to {pemFile}");
                            File.WriteAllText(pemFile, apk.PemData);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Warning: could not save certificate to {pemFile}: {0} {1}", ex.Message, ex.StackTrace);
                    }
                    Console.Write("Closing and signing apk...");
                }
                Console.WriteLine($"..done! (+{ (int)sw.Elapsed.TotalSeconds} seconds, { String.Format("{0:n0}", GC.GetTotalMemory(false))} bytes of memory used)");
            }
            catch (System.Security.SecurityException sex)
            {
                Console.WriteLine("There was some sort of certificate problem: {0}, {1}", sex.Message, sex.StackTrace);
                Environment.ExitCode = -1;
                return;
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
