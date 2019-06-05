using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestomAssets.AssetsChanger;
using System.IO;

namespace QuestomAssets.BeatSaber
{
    public sealed class MainLevelPackCollectionObject : MonoBehaviourObject
    {
        public MainLevelPackCollectionObject(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        public MainLevelPackCollectionObject(IObjectInfo<AssetsObject> objectInfo) : base(objectInfo)
        { }

        public MainLevelPackCollectionObject(AssetsFile assetsFile) : base(assetsFile, BSConst.ScriptHash.MainLevelsCollectionHash, KnownObjects.File17.MainLevelsCollectionScriptPtr)
        { }

        //public void UpdateTypes(AssetsMetadata metadata)
        //{
        //    base.UpdateType(metadata, BSConst.ScriptHash.MainLevelsCollectionHash, BSConst.ScriptPtr.MainLevelsCollectionScriptPtr);
        //}

        public List<PPtr> BeatmapLevelPacks { get; set; } = new List<PPtr>();
        public List<PPtr> PreviewBeatmapLevelPacks { get; set; } = new List<PPtr>();

        protected override void Parse(AssetsReader reader)
        {
            base.Parse(reader);
            BeatmapLevelPacks = reader.ReadArrayOf(x => new PPtr(x));
            PreviewBeatmapLevelPacks = reader.ReadArrayOf(x => new PPtr(x));
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
