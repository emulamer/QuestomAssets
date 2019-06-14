using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{

   
    public class Transform : Component
    {
        protected Transform(AssetsFile assetsFile, int classID) : base(assetsFile, classID)
        {
        }

        protected Transform(IObjectInfo<AssetsObject> objectInfo) : base(objectInfo)
        {
        }

        public Transform(AssetsFile assetsFile) : base(assetsFile, AssetsConstants.ClassID.TransformClassID)
        {
        }

        public Transform(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        protected override void Parse(AssetsReader reader)
        {
            base.Parse(reader);
            LocalRotation = new QuaternionF(reader);
            LocalPosition = new Vector3F(reader);
            LocalScale = new Vector3F(reader);
            Children = reader.ReadArrayOf(x => (ISmartPtr<Transform>)SmartPtr<Transform>.Read(ObjectInfo.ParentFile, this, reader));
            Father = SmartPtr<Transform>.Read(ObjectInfo.ParentFile, this, reader);            
        }

        protected override void WriteObject(AssetsWriter writer)
        {
            base.WriteBase(writer);
            LocalRotation.Write(writer);
            LocalPosition.Write(writer);
            LocalScale.Write(writer);
            writer.WriteArrayOf(Children, (x, y) => x.Write(y));
            Father.Write(writer);            
        }

        public Guid ID = Guid.NewGuid();
        public QuaternionF LocalRotation { get; set; }
        public Vector3F LocalPosition { get; set; }
        public Vector3F LocalScale { get; set; }
        public List<ISmartPtr<Transform>> Children { get; set; } = new List<ISmartPtr<Transform>>();
        public ISmartPtr<Transform> Father { get; set; }

    }
}
