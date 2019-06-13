using QuestomAssets.AssetsChanger;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.BeatSaber
{
    public class AlwaysOwnedContentModel : MonoBehaviourObject, INeedAssetsMetadata
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

        public List<ISmartPtr<BeatmapLevelPackObject>> AlwaysOwnedPacks { get; set; } = new List<ISmartPtr<BeatmapLevelPackObject>>();

        public List<ISmartPtr<BeatmapLevelDataObject>> AlwaysOwnedBeatmapLevels { get; set; } = new List<ISmartPtr<BeatmapLevelDataObject>>();
    }
}
