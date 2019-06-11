using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public abstract class Component : AssetsObject
    {
        protected Component(AssetsFile assetsFile, int classID) : base(assetsFile, classID)
        {
        }

        protected Component(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        protected Component(AssetsFile assetsFile, Guid typeHash)
        {
            ObjectInfo = ObjectInfo<AssetsObject>.FromTypeHash(assetsFile, typeHash, this);
        }

        protected Component(IObjectInfo<AssetsObject> objectInfo) : base(objectInfo)
        {
        }

        protected override void Parse(AssetsReader reader)
        {
            base.Parse(reader);
            try
            {
                GameObject = SmartPtr<GameObject>.Read(ObjectInfo.ParentFile, this, reader);
            }
            catch (Exception ex)
            {
                Log.LogErr("Component failed to load its GameObject... allowing it to continue because this happens with bundles?");
                GameObject = null;
            }
        }

        protected override void WriteBase(AssetsWriter writer)
        {
            base.WriteBase(writer);
            this.GameObject.Write(writer);
        }

        [JsonIgnore]
        public ISmartPtr<GameObject> GameObject { get; set; } = null;

    }
}
