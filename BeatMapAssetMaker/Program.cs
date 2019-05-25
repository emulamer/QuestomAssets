using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace ConsoleApp1
{
    class Program
    {


        static void WriteAsset(string name, string assetFileName, byte[] beatmapDataSOPtr,  BeatmapSaveData beatmapSaveData)
        {
            using (FileStream fs = File.OpenWrite(assetFileName))
            {
                byte[] gameObjPtr = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                byte[] enabled = { 0x01, 0x00, 0x00, 0x00 };

                //this has to be the pointer (or whatever unity calls it) to the BeapMapDataSO
                //beatMapDataSOPtr = { 0x01, 0x00, 0x00, 0x00, 0x10, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                fs.Write(gameObjPtr, 0, gameObjPtr.Length);
                fs.Write(enabled, 0, enabled.Length);
                fs.Write(beatmapDataSOPtr, 0, beatmapDataSOPtr.Length);
                byte[] namelen = BitConverter.GetBytes(name.Length);
                fs.Write(namelen, 0, namelen.Length);
                
                byte[] nameBytes = System.Text.UTF8Encoding.UTF8.GetBytes(name);
                fs.Write(nameBytes, 0, nameBytes.Length);
                //dunno if it's aligned to something or what
                if (name.Length % 2 != 0)
                {
                    fs.WriteByte(0);
                }
                //json data string length (0)
                fs.Write(new byte[] { 0x0, 0x0, 0x0, 0x0 }, 0, 4);

                //not really sure if the signature has to be 128 bytes, tossing in all zeroes just in case

                //signature length (128)
                fs.Write(new byte[] { 0x80, 0x0, 0x0, 0x0 }, 0, 4);

                ///signature (all zeroes)
                fs.Write(new byte[128], 0, 128);

                //_projectedData goes next
                byte[] projectedData = beatmapSaveData.SerializeToBinary();

                fs.Write(BitConverter.GetBytes(projectedData.Length), 0, 4);
                fs.Write(projectedData, 0, projectedData.Length);
                //seems to be trailed with a zero *shrug*
                fs.WriteByte(0);

            }
        }
        static void Main(string[] args)
        {
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

                WriteAsset(assetname, assetfile, beatmapSOPtr, bmd);
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
