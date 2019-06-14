using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class MeshFilterObject : Component
    {
        public MeshFilterObject(AssetsFile assetsFile) : base(assetsFile, AssetsConstants.ClassID.MeshFilterClassID)
        {
        }

        public MeshFilterObject(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        protected override void Parse(AssetsReader reader)
        {
            base.Parse(reader);
            Mesh = SmartPtr<MeshObject>.Read(ObjectInfo.ParentFile, this, reader);
        }

        protected override void WriteObject(AssetsWriter writer)
        {
            base.WriteBase(writer);
            Mesh.Write(writer);
        }
        
        public ISmartPtr<MeshObject> Mesh { get; set; } = null;

    }
}
