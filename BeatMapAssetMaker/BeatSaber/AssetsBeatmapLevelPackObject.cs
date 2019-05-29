using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatmapAssetMaker.AssetsChanger;

namespace BeatmapAssetMaker.BeatSaber
{
    public class AssetsBeatmapLevelPackObject : AssetsMonoBehaviourObject
    {
        public AssetsBeatmapLevelPackObject(AssetsObjectInfo objectInfo, AssetsReader reader) : base(objectInfo, reader)
        { }
        public AssetsBeatmapLevelPackObject(AssetsObjectInfo objectInfo) : base(objectInfo)
        { }

        public string PackID { get; set; }

        public string PackName { get; set; }

        public AssetsPtr CoverImage { get; set; }

        public bool IsPackAlwaysOwned { get; set; }

        public AssetsPtr BeatmapLevelCollection { get; set; }

        private byte[] SerializeData()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (AssetsWriter writer = new AssetsWriter(ms))
                {
                    writer.Write(PackID);
                    writer.AlignTo(4);
                    writer.Write(PackName);
                    writer.AlignTo(4);
                    CoverImage.Write(writer);
                    writer.Write(IsPackAlwaysOwned);
                    writer.AlignTo(4);
                    BeatmapLevelCollection.Write(writer);
                }
                return ms.ToArray();
            }

        }

        private void DeserializeData()
        {
            using (MemoryStream ms = new MemoryStream(base.ScriptParametersData))
            {
                using (AssetsReader reader = new AssetsReader(ms))
                {
                    PackID = reader.ReadString();
                    reader.AlignToObjectData(4);
                    PackName = reader.ReadString();
                    reader.AlignToObjectData(4);
                    CoverImage = new AssetsPtr(reader);
                    IsPackAlwaysOwned = reader.ReadBoolean();
                    reader.AlignTo(4);
                    BeatmapLevelCollection = new AssetsPtr(reader);
                }
            }
        }

        public override byte[] ScriptParametersData
        {
            get
            {
                return SerializeData();
            }
            set
            {
                base.ScriptParametersData = value;
                DeserializeData();
            }
        }
    }
}
