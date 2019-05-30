using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatmapAssetMaker.AssetsChanger;
using System.IO;

namespace BeatmapAssetMaker.BeatSaber
{
    public sealed class AssetsMainLevelPackCollection : AssetsMonoBehaviourObject
    {
        public AssetsMainLevelPackCollection(AssetsObjectInfo objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        public AssetsMainLevelPackCollection(AssetsObjectInfo objectInfo) : base(objectInfo)
        { }

        public AssetsMainLevelPackCollection(AssetsMetadata metadata) : base(metadata, AssetsConstants.ScriptHash.MainLevelsCollectionHash, AssetsConstants.ScriptPtr.MainLevelsCollectionScriptPtr)
        { }

        public void UpdateTypes(AssetsMetadata metadata)
        {
            base.UpdateType(metadata, AssetsConstants.ScriptHash.MainLevelsCollectionHash, AssetsConstants.ScriptPtr.MainLevelsCollectionScriptPtr);
        }

        public List<AssetsPtr> BeatmapLevelPacks { get; set; } = new List<AssetsPtr>();
        public List<AssetsPtr> PreviewBeatmapLevelPacks { get; set; } = new List<AssetsPtr>();

        protected override void Parse(AssetsReader reader)
        {
            base.Parse(reader);
            BeatmapLevelPacks = reader.ReadArrayOf(x => new AssetsPtr(x));
            PreviewBeatmapLevelPacks = reader.ReadArrayOf(x => new AssetsPtr(x));
        }

        public override void Write(AssetsWriter writer)
        {
            base.WriteBase(writer);
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
      
        public override byte[] ScriptParametersData
        {
            get
            {
                throw new InvalidOperationException("Cannot access parameters data from this object.");
            }
            set
            {
                throw new InvalidOperationException("Cannot access parameters data from this object.");
            }
        }
    }
}
