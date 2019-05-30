using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BeatmapAssetMaker.AssetsChanger
{
    public class AssetsMetadata
    {
        public AssetsMetadata(AssetsReader reader)
        {
            Types = new List<AssetsType>();
            ObjectInfos = new List<AssetsObjectInfo>();
            Adds = new List<AssetsPtr>();
            ExternalFiles = new List<AssetsExternalFile>();
            Parse(reader);
        }
        public string Version { get; set; }
        public Int32 Platform { get; set; }
        public bool HasTypeTrees { get; set; }
        //public Int32 NumberOfTypes { get; set; }
        public List<AssetsType> Types { get; set; }
        //public Int32 NumberOfObjectInfos { get; set; }
        public List<AssetsObjectInfo> ObjectInfos { get; set; }
        //don't know what this is
        //public Int32 NumberOfAdds { get; set; }
        public List<AssetsPtr> Adds { get; set; }

        //public Int32 NumberOfExternalFiles { get; set; }
        public List<AssetsExternalFile> ExternalFiles { get; set; }

        public void Parse(AssetsReader reader)
        {
            Version = reader.ReadCStr();
            Platform = reader.ReadInt32();
            HasTypeTrees = reader.ReadBoolean();
            int numTypes = reader.ReadInt32();
            for (int i = 0;i < numTypes; i++)
            {
                Types.Add(new AssetsType(reader, HasTypeTrees));
            }
            //reader.AlignToMetadata(4);
            int numObj = reader.ReadInt32();
            for (int i = 0; i < numObj; i++)
            {
                ObjectInfos.Add(new AssetsObjectInfo(reader));
            }
            //reader.AlignToMetadata(4);
            int numAdds = reader.ReadInt32();
            for (int i = 0; i < numAdds; i++)
            {
                Adds.Add(new AssetsPtr(reader));
            }
           // reader.AlignToMetadata(4);
            int numExt = reader.ReadInt32();
            for (int i = 0; i < numExt; i++)
            {
                ExternalFiles.Add(new AssetsExternalFile(reader));
            }
        }

        public void Write(AssetsWriter writer)
        {
            writer.WriteCString(Version);
            writer.Write(Platform);
            writer.Write(HasTypeTrees);
            writer.Write(Types.Count());
            Types.ForEach(x => x.Write(writer));
            writer.Write(ObjectInfos.Count());
            writer.AlignTo(4);
            ObjectInfos.ForEach(x => x.Write(writer));
            writer.Write(Adds.Count());
            writer.AlignTo(4);
            Adds.ForEach(x => x.Write(writer));
            writer.Write(ExternalFiles.Count());
            ExternalFiles.ForEach(x => x.Write(writer));
        }
        public int GetTypeIndexFromClassID(int classID)
        {
            var type = Types.FirstOrDefault(x => x.ClassID == classID);
            if (type == null)
                throw new ArgumentException("ClassID was not found in metadata.");

            return Types.IndexOf(type);
        }

        public int GetTypeIndexFromScriptHash(Guid hash)
        {
            var type = Types.FirstOrDefault(x => x.ScriptHash == hash);
            if (type == null)
                throw new ArgumentException("Script hash was not found in metadata.");
            return Types.IndexOf(type);
        }

        public int GetClassIDFromTypeIndex(int typeIndex)
        {
            if (typeIndex < 1 || typeIndex > Types.Count() - 1)
                throw new ArgumentException("There is no type at this index.");
            return Types[typeIndex].ClassID;
        }
    }
}

