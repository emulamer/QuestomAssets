using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using QuestomAssets.AssetsChanger;

namespace QuestomAssets.Utils
{
    public class FilePatch
    {
        public string Filename { get; set; }
        public UInt32 ExpectedFileSize { get; set; }
        public List<Patch> Patches { get; set; }
    }
    public class Patch
    {
        public string Name { get; set; }
        public UInt32 Address { get; set; }
        public List<byte> ExpectedData { get; set; }
        public List<byte> PatchData { get; set; }
    }
    public class Patcher
    {
        
        public static bool Patch(IFileProvider apk, FilePatch patch)
        {
            string binaryFile = patch.Filename;
            if (!apk.FileExists(binaryFile))
            {
                Console.WriteLine("Binary file to patch doesn't exist in the APK!");
                return false;
            }
            byte[] binaryBytes = apk.Read(binaryFile);
            if (binaryBytes.Length != patch.ExpectedFileSize)
            {
                Console.WriteLine("Binary file to patch is the wrong length!");
                return false;
            }
            List<Patch> toApply = new List<Patch>();
            Console.WriteLine("Verifying patches binary...");
            using (MemoryStream msBinary = new MemoryStream(binaryBytes))
            {
                //verify each of the patches can be applied or already are applied
                foreach (Patch p in patch.Patches)
                {
                    msBinary.Seek(p.Address, SeekOrigin.Begin);
                    byte[] readVals = new byte[p.ExpectedData.Count];
                    msBinary.Read(readVals, 0, p.ExpectedData.Count);

                    if (!readVals.SequenceEqual(p.ExpectedData))
                    {
                        msBinary.Seek(p.Address, SeekOrigin.Begin);
                        readVals = new byte[p.PatchData.Count];
                        msBinary.Read(readVals, 0, p.PatchData.Count);
                        if (readVals.SequenceEqual(p.PatchData))
                        {
                            Console.WriteLine($"Patch {p.Name} already appears to be applied.");
                            continue;
                        }
                        else
                        {
                            Console.WriteLine($"Patch {p.Name} can't be applied to this binary, the code at the patch location doesn't match what was expected.  Aborting any patching...");
                            //if one patch can't be applied, abort the whole thing
                            return false;
                        }
                    }
                }
                foreach (Patch p in toApply)
                {
                    msBinary.Seek(p.Address, SeekOrigin.Begin);
                    msBinary.Write(p.PatchData.ToArray(), 0, p.PatchData.Count);
                }
                msBinary.Seek(0, SeekOrigin.Begin);
                
                apk.Write(binaryFile, msBinary.ToArray(), true, true);
            }
            Console.WriteLine("Done patching binary!");
            return true;
        }

    }
}
