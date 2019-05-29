using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatmapAssetMaker.AssetsChanger;
using System.IO;

namespace BeatmapAssetMaker.BeatSaber
{
    public class AssetsMainLevelPackCollection : AssetsMonoBehaviourObject
    {
        public AssetsMainLevelPackCollection(AssetsObjectInfo objectInfo, AssetsReader reader) : base(objectInfo, reader)
        { }

        public AssetsMainLevelPackCollection(AssetsObjectInfo objectInfo) : base(objectInfo)
        { }

        public List<AssetsPtr> BeatmapLevelPacks { get; set; } = new List<AssetsPtr>();
        public List<AssetsPtr> PreviewBeatmapLevelPacks { get; set; } = new List<AssetsPtr>();

        private byte[] SerializeData()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (AssetsWriter writer= new AssetsWriter(ms))
                {
                    writer.Write(BeatmapLevelPacks.Count());
                    foreach (var ptr in BeatmapLevelPacks)
                    {
                        ptr.Write(writer);
                    }
                    writer.Write(PreviewBeatmapLevelPacks.Count());
                    foreach (var ptr in PreviewBeatmapLevelPacks)
                    {
                        ptr.Write(writer);
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
                        BeatmapLevelPacks.Add(new AssetsPtr(reader));
                    }

                    count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        PreviewBeatmapLevelPacks.Add(new AssetsPtr(reader));
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
