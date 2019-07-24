using QuestomAssets.AssetsChanger;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.BeatSaber
{
    public sealed class BeatmapCharacteristicCollectionObject : MonoBehaviourObject
    {
        public BeatmapCharacteristicCollectionObject(AssetsFile assetsFile) : base(assetsFile, assetsFile.Manager.GetScriptObject("BeatmapLevelCollectionSO"))
        {
            BeatmapCharacteristics = new List<ISmartPtr<BeatmapCharacteristicObject>>();
        }

        public BeatmapCharacteristicCollectionObject(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo)
        {
            BeatmapCharacteristics = new List<ISmartPtr<BeatmapCharacteristicObject>>();
            Parse(reader);
        }

        public List<ISmartPtr<BeatmapCharacteristicObject>> BeatmapCharacteristics { get; set; }

        public override void Parse(AssetsReader reader)
        {
            base.ParseBase(reader);
            BeatmapCharacteristics = reader.ReadArrayOf((o) => (ISmartPtr<BeatmapCharacteristicObject>)SmartPtr<BeatmapCharacteristicObject>.Read(ObjectInfo.ParentFile, this, o));
        }

        protected override void WriteObject(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.WriteArrayOf(BeatmapCharacteristics, (o, w) =>
            o.Write(w));
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
