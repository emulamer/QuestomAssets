using QuestomAssets.AssetsChanger;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.BeatSaber
{
    public sealed class BeatmapCharacteristicObject : MonoBehaviourObject, INeedAssetsMetadata
    {
        public BeatmapCharacteristicObject(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        public BeatmapCharacteristicObject(AssetsFile assetsFile) : base(assetsFile, assetsFile.Manager.GetScriptObject("BeatmapCharacteristicSO"))
        { }

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

        public override void Parse(AssetsReader reader)
        {
            base.ParseBase(reader);
            Icon = SmartPtr<SpriteObject>.Read(ObjectInfo.ParentFile, this, reader);
            HintText = reader.ReadString();
            CharacteristicName = reader.ReadString();
            SerializedName = reader.ReadString();
            CompoundIdPartName = reader.ReadString();
            SortingOrder = reader.ReadInt32();
        }

        protected override void WriteObject(AssetsWriter writer)
        {
            base.WriteBase(writer);
            Icon.Write(writer);
            writer.Write(HintText);
            writer.Write(CharacteristicName);
            writer.Write(SerializedName);
            writer.Write(CompoundIdPartName);
            writer.Write(SortingOrder);
        }

        public SmartPtr<SpriteObject> Icon { get; set; }
        public string HintText { get; set; }
        public string CharacteristicName { get; set; }
        public string SerializedName { get; set; }
        public string CompoundIdPartName { get; set; }
        public int SortingOrder { get; set; }
    }
}
