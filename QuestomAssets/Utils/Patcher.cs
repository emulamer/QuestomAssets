using Emulamer.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QuestomAssets.Utils
{
    public class Patcher
    {
        public static bool PatchBeatmapSigCheck(Apkifier apk)
        {
            string binaryFile = "lib/armeabi-v7a/libil2cpp.so";
            if (!apk.FileExists(binaryFile))
            {
                Console.WriteLine("Binary file to patch doesn't exist in the APK!");
                return false;
            }
            byte[] binaryBytes = apk.Read(binaryFile);
            if (binaryBytes.Length != 244756415)
            {
                Console.WriteLine("Binary file to patch is the wrong length!");
                return false;
            }
            Console.WriteLine("Patching binary...");
            using (MemoryStream msBinary = new MemoryStream(binaryBytes))
            {
                msBinary.Seek(0x491578, SeekOrigin.Begin);
                byte[] readVals = new byte[4];
                msBinary.Read(readVals, 0, 4);
                if (readVals[0] != 0x00 || readVals[1] != 0x00 || readVals[2] != 0xA0 || readVals[3] != 0xE3)
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
                msBinary.Seek(0x491578, SeekOrigin.Begin);
                msBinary.Write(new byte[] { 0x01, 0x00, 0xA0, 0xE3 }, 0, 4);
                msBinary.Seek(0, SeekOrigin.Begin);
                byte[] binaryOutData = msBinary.ToArray();
                apk.Write(msBinary, binaryFile, true, true);
            }
            Console.WriteLine("Done patching binary!");
            return true;
        }

        
    }
}
