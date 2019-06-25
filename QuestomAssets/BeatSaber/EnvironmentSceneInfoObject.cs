using QuestomAssets.AssetsChanger;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.BeatSaber
{
    public class EnvironmentSceneInfoObject : MonoBehaviourObject, INeedAssetsMetadata
    {
        public EnvironmentSceneInfoObject(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        public EnvironmentSceneInfoObject(AssetsFile assetsFile) : base(assetsFile, assetsFile.Manager.GetScriptObject("SceneInfo"))
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

        protected override void Parse(AssetsReader reader)
        {
            base.Parse(reader);
            SceneName = reader.ReadString();
            DisabledRootObjects = reader.ReadBoolean();
            reader.AlignTo(4);
        }

        protected override void WriteObject(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.Write(SceneName);
            writer.Write(DisabledRootObjects);
            writer.AlignTo(4);
        }

        public string SceneName { get; set; }
        public bool DisabledRootObjects { get; set; }
    }
}
