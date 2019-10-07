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
        public bool IsEnabled { get; set; }
        public byte CastShadows { get; set; }
        public byte ReceiveShadows { get; set; }
        public byte DynamicOcclude { get; set; }
        public byte MotionVectors { get; set; }
        public byte LightProbeUsage { get; set; }
        public byte ReflectionProbeUsage { get; set; }
        public UInt32 RenderingLayerMask { get; set; }
        public int RendererPriority { get; set; }
        public UInt16 LightmapIndex { get; set; }
        public UInt16 LightmapIndexDynamic { get; set; }
        public Vector4F LightmapTilingOffset { get; set; }
        public Vector4F LightmapTilingOffsetDynamic { get; set; }
        //not sure if this is exclusively materials or not, but since I don't have the class I guess it doesn't much matter
        public List<ISmartPtr<MaterialObject>> Materials { get; set; }
        public StaticBatchInfo StaticBatchInfo { get; set; }
        public SmartPtr<Transform> StaticBatchRoot { get; set; }
        public SmartPtr<Transform> ProbeAnchor { get; set; }
        public SmartPtr<GameObject> LightProbeVolumeOverride { get; set; }
        public int SortingLayerID { get; set; }
        public Int16 SortingLayer { get; set; }
        public Int16 SortingOrder { get; set; }
        public SmartPtr<MeshObject> AdditionalVertexStreams { get; set; }

        public override void Parse(AssetsReader reader)
        {
            base.ParseBase(reader);
            IsEnabled = reader.ReadBoolean();
            CastShadows = reader.ReadByte();
            ReceiveShadows = reader.ReadByte();
            DynamicOcclude = reader.ReadByte();
            MotionVectors = reader.ReadByte();
            LightProbeUsage = reader.ReadByte();
            ReflectionProbeUsage = reader.ReadByte();
            reader.AlignTo(4);
            RenderingLayerMask = reader.ReadUInt32();
            if (ObjectInfo.ParentFile.Metadata.VersionGte("2018.3"))
            {
                RendererPriority = reader.ReadInt32();
            }
            LightmapIndex = reader.ReadUInt16();
            LightmapIndexDynamic = reader.ReadUInt16();
            LightmapTilingOffset = new Vector4F(reader);
            LightmapTilingOffsetDynamic = new Vector4F(reader);
            Materials = reader.ReadArrayOf(r => (ISmartPtr < MaterialObject > )SmartPtr<MaterialObject>.Read(ObjectInfo.ParentFile, this, r));
            StaticBatchInfo = new StaticBatchInfo(reader);
            StaticBatchRoot = SmartPtr<Transform>.Read(ObjectInfo.ParentFile, this, reader);
            ProbeAnchor = SmartPtr<Transform>.Read(ObjectInfo.ParentFile, this, reader);
            LightProbeVolumeOverride = SmartPtr<GameObject>.Read(ObjectInfo.ParentFile, this, reader);
            SortingLayerID = reader.ReadInt32();
            SortingLayer = reader.ReadInt16();
            SortingOrder = reader.ReadInt16();
            AdditionalVertexStreams = SmartPtr<MeshObject>.Read(ObjectInfo.ParentFile, this, reader);
        }

        protected override void WriteObject(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.Write(IsEnabled);
            writer.Write(CastShadows);
            writer.Write(ReceiveShadows);
            writer.Write(DynamicOcclude);
            writer.Write(MotionVectors);
            writer.Write(LightProbeUsage);
            writer.Write(ReflectionProbeUsage);
            writer.AlignTo(4);
            writer.Write(RenderingLayerMask);
            if (ObjectInfo.ParentFile.Metadata.VersionGte("2018.3"))
            {
                writer.Write(RendererPriority);
            }
            writer.Write(LightmapIndex);
            writer.Write(LightmapIndexDynamic);
            LightmapTilingOffset.Write(writer);
            LightmapTilingOffsetDynamic.Write(writer);
            writer.WriteArrayOf(Materials, (o, w) => o.Write(w));
            StaticBatchInfo.Write(writer);
            StaticBatchRoot.Write(writer);
            ProbeAnchor.Write(writer);
            LightProbeVolumeOverride.Write(writer);
            writer.Write(SortingLayerID);
            writer.Write(SortingLayer);
            writer.Write(SortingOrder);
            AdditionalVertexStreams.Write(writer);
        }

        
    }
}
