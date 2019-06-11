using QuestomAssets.AssetsChanger;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.BeatSaber
{
    public class AlwaysOwnedContentModel : MonoBehaviourObject
    {

        public AlwaysOwnedContentModel(AssetsFile assetsFile) : base(assetsFile, assetsFile.Manager.GetScriptObject("AlwaysOwnedContentModelSO"))
        { }

        public AlwaysOwnedContentModel(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        protected override void Parse(AssetsReader reader)
        {
            base.Parse(reader);
            AlwaysOwnedPacks = reader.ReadArrayOf<ISmartPtr<BeatmapLevelPackObject>>(x => SmartPtr<BeatmapLevelPackObject>.Read(ObjectInfo.ParentFile, this, reader));
            AlwaysOwnedBeatmapLevels = reader.ReadArrayOf<ISmartPtr<BeatmapLevelDataObject>>(x => SmartPtr<BeatmapLevelDataObject>.Read(ObjectInfo.ParentFile, this, reader));
        }

        public override void Write(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.WriteArrayOf(AlwaysOwnedPacks, (x, y) => x.Write(y));
            writer.WriteArrayOf(AlwaysOwnedBeatmapLevels, (x, y) => x.Write(y));
        }

        public System.Collections.ObjectModel.ObservableCollection<ISmartPtr<BeatmapLevelPackObject>> AlwaysOwnedPacks { get; set; } = new System.Collections.ObjectModel.ObservableCollection<ISmartPtr<BeatmapLevelPackObject>>();

        public System.Collections.ObjectModel.ObservableCollection<ISmartPtr<BeatmapLevelDataObject>> AlwaysOwnedBeatmapLevels { get; set; } = new System.Collections.ObjectModel.ObservableCollection<ISmartPtr<BeatmapLevelDataObject>>();
    }
}
