using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace ConsoleApp1
{
    class Program
    {

        // static void WriteLevelCollectionAsser(string name, string assetFileName, byte[] beatmapDataSOPtr)
        static UPtr BeatmapLevelSOType = new UPtr() { FileID = 1, PathID = 644 };
        static UPtr BeatmapDataSOType = new UPtr() { FileID = 1, PathID = 1552 };
        static UPtr BeatmapLevelCollectionSOType = new UPtr() { FileID = 1, PathID = 762 };
        static UPtr ReplaceLevelCollection = new UPtr() { FileID = 0, PathID = 240 };
        

        const int ASSET_PATHID_START = 4800010;

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
                
                var fs = new AlignedStream(f);
                WriteMBHeader(fs, assetName, BeatmapLevelSOType);
                levelData.Write(fs);
                f.Close();
            }
        }

        static void WriteLevelCollectionAsset(string assetName, string outputFilename, BeatmapLevelCollectionSO levelCollection)
        {
            using (FileStream f = File.Open(outputFilename, FileMode.Create))
            {
                AlignedStream fs = new AlignedStream(f);
                WriteMBHeader(fs, assetName, BeatmapLevelCollectionSOType);
                levelCollection.Write(fs);
                f.Close();
            }
        }

        static void WriteBeatmapAsset(string assetName, string outputFilename, BeatmapSaveData beatmapSaveData)
        {            
            using (FileStream f = File.Open(outputFilename, FileMode.Create))
            {
                AlignedStream fs = new AlignedStream(f);
                WriteMBHeader(fs, assetName, BeatmapDataSOType);
                //json data string length (0)
                fs.Write((int)0);
                //not really sure if the signature has to be 128 bytes, tossing in all zeroes just in case

                //signature length (128)
                fs.Write((int)128);

                ///signature (all zeroes)
                fs.Write(new byte[128], 0, 128);

                //_projectedData goes next
                byte[] projectedData = beatmapSaveData.SerializeToBinary(true);
                fs.Write(projectedData);


                //seems to be trailed with a zero *shrug*
                fs.WriteByte(0);
            }
        }

        static void MakeAssets(string inputPath, string outputPath)
        {
            int pathid = ASSET_PATHID_START;
            var sng = CustomSongLoader.LoadFromPath(inputPath);
            (sng._difficultyBeatmapSets.Where(x => x._beatmapCharacteristicName != Characteristic.Standard).ToList()).ForEach(y => sng._difficultyBeatmapSets.Remove(y));
           // sng._songName = "ABeatSab";

            sng._environmentSceneInfo = new UPtr() { FileID = 20, PathID = 1 };
            sng._audioClip = new UPtr() { FileID = 0, PathID = 39 };
            pathid++;
            sng._coverImageTexture2D = new UPtr() { FileID = 0, PathID = 19 };
            pathid++;
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
                    string fName = Path.Combine(outputPath, $"{pathid}_{bmAssetName}.asset");
                    WriteBeatmapAsset(bmAssetName, fName, g._beatmapSaveData);
                    g._beatmapDataPtr = new UPtr() { FileID = 0, PathID = pathid };
                    pathid++;
                }
            }
            string levelAssetName = $"{sng._levelID}Level";
            string levelAssetFile = Path.Combine(outputPath, $"{pathid}_{levelAssetName}.asset");
            
            WriteBeatmapLevelSOAsset(levelAssetName, levelAssetFile, sng);
            var lc = new BeatmapLevelCollectionSO();
            lc._beatmapLevels.Add(new UPtr() { FileID = 0, PathID = 90 });
            lc._beatmapLevels.Add(new UPtr() { FileID = 0, PathID = 151 });
            lc._beatmapLevels.Add(new UPtr() { FileID = 0, PathID = 162 });
            lc._beatmapLevels.Add(new UPtr() { FileID = 0, PathID = 207 });
            lc._beatmapLevels.Add(new UPtr() { FileID = 0, PathID = pathid});
            WriteLevelCollectionAsset("OstVol2LevelCollection", Path.Combine(outputPath, "240_OstVol2LevelCollection.asset"), lc);
        }
        static void Main(string[] args)
        {

            byte[] b;
            using (FileStream f = File.OpenRead(@"C:\Users\VR\Desktop\platform-tools_r28.0.3-windows\7638-7516\assets\BeatSaberExpertPlusBeatmapData.asset"))
            {
                var offset = 204;
                f.Seek(offset, SeekOrigin.Begin);
                var len = f.Length - offset;
                b = new byte[len];
                f.Read(b, 0, (int)len);
            }

            BeatmapSaveData beatmapSaveData = BeatmapSaveData.DeserializeFromFromBinary(b);
            WriteBeatmapAsset("BeatSaberExpertPlusBeatmapData", @"C:\Users\VR\Desktop\platform-tools_r28.0.3-windows\7638-7516\assets\easy.out", beatmapSaveData);


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
