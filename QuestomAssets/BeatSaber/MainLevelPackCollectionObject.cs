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

        //public MainLevelPackCollectionObject(IObjectInfo<AssetsObject> objectInfo) : base(objectInfo)
        //{ }

        public MainLevelPackCollectionObject(AssetsFile assetsFile) : base(assetsFile, assetsFile.Manager.GetScriptObject("BeatmapLevelPackCollectionSO"))
        { }

        //public void UpdateTypes(AssetsMetadata metadata)
        //{
        //    base.UpdateType(metadata, BSConst.ScriptHash.MainLevelsCollectionHash, BSConst.ScriptPtr.MainLevelsCollectionScriptPtr);
        //}

        public List<ISmartPtr<BeatmapLevelPackObject>> BeatmapLevelPacks { get; set; } = new List<ISmartPtr<BeatmapLevelPackObject>>();
        public List<ISmartPtr<MonoBehaviourObject>> PreviewBeatmapLevelPacks { get; set; } = new List<ISmartPtr<MonoBehaviourObject>>();

        protected override void Parse(AssetsReader reader)
        {
            //new PPtr(x)
            //to SmartPtr<BeatmapLevelPackObject>.Read(ObjectInfo.ParentFile,x)
            base.Parse(reader);
            BeatmapLevelPacks = reader.ReadArrayOf(x => SmartPtr<BeatmapLevelPackObject>.Read(ObjectInfo.ParentFile, this, x) ).Cast<ISmartPtr<BeatmapLevelPackObject>>().ToList();
            PreviewBeatmapLevelPacks = reader.ReadArrayOf(x => SmartPtr<MonoBehaviourObject>.Read(ObjectInfo.ParentFile, this, x)).Cast<ISmartPtr<MonoBehaviourObject>>().ToList();
        }

        protected override void WriteObject(AssetsWriter writer)
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
