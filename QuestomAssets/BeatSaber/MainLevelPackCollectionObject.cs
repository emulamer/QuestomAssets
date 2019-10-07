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

        public MainLevelPackCollectionObject(AssetsFile assetsFile) : base(assetsFile, assetsFile.Manager.GetScriptObject("BeatmapLevelPackCollectionSO"))
        { }

        public List<ISmartPtr<BeatmapLevelPackObject>> BeatmapLevelPacks { get; set; } = new List<ISmartPtr<BeatmapLevelPackObject>>();
        public List<ISmartPtr<MonoBehaviourObject>> PreviewBeatmapLevelPacks { get; set; } = new List<ISmartPtr<MonoBehaviourObject>>();

        public override void Parse(AssetsReader reader)
        {
            base.ParseBase(reader);
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

        [System.ComponentModel.Browsable(false)]
        [Newtonsoft.Json.JsonIgnore]
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
