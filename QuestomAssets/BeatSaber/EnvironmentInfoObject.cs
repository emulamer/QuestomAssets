using QuestomAssets.AssetsChanger;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.BeatSaber
{
    public sealed class EnvironmentInfoObject : MonoBehaviourObject, INeedAssetsMetadata
    {
        public EnvironmentInfoObject(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        public EnvironmentInfoObject(AssetsFile assetsFile) : base(assetsFile, assetsFile.Manager.GetScriptObject("EnvironmentInfoSO"))
        { }

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

        public override void Parse(AssetsReader reader)
        {
            base.ParseBase(reader);
            EnvironmentName = reader.ReadString();
            ColorScheme = SmartPtr<AssetsObject>.Read(ObjectInfo.ParentFile, this, reader);
            SceneInfo = SmartPtr<EnvironmentSceneInfoObject>.Read(ObjectInfo.ParentFile, this, reader);
            SerializedName = reader.ReadString();
            EnvironmentType = SmartPtr<AssetsObject>.Read(ObjectInfo.ParentFile, this, reader);
            reader.AlignTo(4);
        }

        protected override void WriteObject(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.Write(EnvironmentName);
            ColorScheme.Write(writer);
            SceneInfo.Write(writer);
            writer.Write(SerializedName);
            EnvironmentType.Write(writer);
            writer.AlignTo(4);
        }

        public string EnvironmentName { get; set; }

        public ISmartPtr<AssetsObject> ColorScheme { get; set; }

        public ISmartPtr<EnvironmentSceneInfoObject> SceneInfo { get; set; }

        public string SerializedName { get; set; }

        public ISmartPtr<AssetsObject> EnvironmentType { get; set; }
        
    }
}
