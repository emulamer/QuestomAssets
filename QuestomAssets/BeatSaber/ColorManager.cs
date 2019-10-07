using Newtonsoft.Json;
using QuestomAssets.AssetsChanger;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.BeatSaber
{
    public sealed class ColorManager : MonoBehaviourObject, INeedAssetsMetadata
    {
        public ISmartPtr<AssetsObject> PlayerModel { get; set; }
        public ISmartPtr<SimpleColorSO> ColorA { get; set; }
        public ISmartPtr<SimpleColorSO> ColorB { get; set; }

        public ColorManager(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo, reader)
        {
            Parse(reader);
        }

        public ColorManager(AssetsFile assetsFile) : base(assetsFile, assetsFile.Manager.GetScriptObject("ColorManager"))
        { }

        public override void Parse(AssetsReader reader)
        {
            base.ParseBase(reader);
            PlayerModel = SmartPtr<AssetsObject>.Read(ObjectInfo.ParentFile, this, reader);
            ColorA = SmartPtr<SimpleColorSO>.Read(ObjectInfo.ParentFile, this, reader);
            ColorB = SmartPtr<SimpleColorSO>.Read(ObjectInfo.ParentFile, this, reader);
        }

        protected override void WriteObject(AssetsWriter writer)
        {
            WriteBase(writer);
            PlayerModel.Write(writer);
            ColorA.Write(writer);
            ColorB.Write(writer);
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
