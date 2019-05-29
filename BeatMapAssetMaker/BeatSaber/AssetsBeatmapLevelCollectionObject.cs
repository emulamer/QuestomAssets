using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatmapAssetMaker.AssetsChanger;

namespace BeatmapAssetMaker.BeatSaber
{
    public class AssetsBeatmapLevelCollectionObject : AssetsMonoBehaviourObject
    {
        public AssetsBeatmapLevelCollectionObject(AssetsObjectInfo objectInfo, AssetsReader reader) : base(objectInfo, reader)
        { }

        public AssetsBeatmapLevelCollectionObject(AssetsObjectInfo objectInfo) : base(objectInfo)
        { }

        public List<AssetsPtr> BeatmapLevels { get; set; } = new List<AssetsPtr>();

        private byte[] SerializeData()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (AssetsWriter writer = new AssetsWriter(ms))
                {
                    writer.Write(BeatmapLevels.Count());
                    foreach (var bml in BeatmapLevels)
                    {
                        bml.Write(writer);
                    }
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
                    int count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        BeatmapLevels.Add(new AssetsPtr(reader));
                    }
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
