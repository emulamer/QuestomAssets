using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public sealed class MeshRendererObject : Component
    {
        public MeshRendererObject(AssetsFile assetsFile) : base(assetsFile, AssetsConstants.ClassID.MeshFilterClassID)
        {
        }

        public MeshRendererObject(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        public override void Parse(AssetsReader reader)
        {
            base.ParseBase(reader);
            Mesh = SmartPtr<MeshObject>.Read(ObjectInfo.ParentFile, this, reader);
        }

        protected override void WriteObject(AssetsWriter writer)
        {
            base.WriteBase(writer);
            Mesh.Write(writer);
        }

        public ISmartPtr<MeshObject> Mesh { get; set; } = null;
        public bool IsEnabled { get; set; }
        public byte CastShadows { get; set; }
        public byte ReceiveShadows { get; set; }
        public byte DynamicOcclude { get; set; }
        public byte MotionVectors { get; set; }
        public byte LightProbeUsage { get; set; }
        public byte ReflectionProbeUsage { get; set; }
        public UInt32 RenderingLayerMask { get; set; }
        public UInt16 LightmapIndex { get; set; }
        public UInt16 LightmapIndexDynamic { get; set; }
        public Vector4F LightmapTilingOffset { get; set; }
        public Vector4F LightmapTilingOffsetDynamic { get; set; }
        //not sure if this is exclusively materials or not, but since I don't have the class I guess it doesn't much matter
        public List<ISmartPtr<AssetsObject>> Materials { get; set; }
        public StaticBatchInfo StaticBatchInfo { get; set; }
        public SmartPtr<Transform> StaticBatchRoot { get; set; }
        public SmartPtr<Transform> ProbeAnchor { get; set; }
        public SmartPtr<GameObject> LightProbeVolumeOverride { get; set; }
        public int SortingLayerID { get; set; }
        public Int16 SortingLayer { get; set; }
        public Int16 SortingOrder { get; set; }
        public SmartPtr<MeshObject> AdditionalVertexStreams { get; set; }
    }
}
