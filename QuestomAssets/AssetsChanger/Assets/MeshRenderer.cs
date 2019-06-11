using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class MeshRenderer : Component
    {
        public MeshRenderer(AssetsFile assetsFile) : base(assetsFile, AssetsConstants.ClassID.MeshFilterClassID)
        {
        }

        public MeshRenderer(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        protected override void Parse(AssetsReader reader)
        {
            base.Parse(reader);
            var readLength = ObjectInfo.DataSize - (reader.Position - ObjectInfo.DataOffset);
            MeshRendererData = reader.ReadBytes(readLength);
        }

        public override void Write(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.Write(MeshRendererData);
        }

        public byte[] MeshRendererData { get; set; }
    }
}
