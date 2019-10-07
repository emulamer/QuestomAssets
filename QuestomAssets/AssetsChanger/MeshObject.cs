using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class MeshObject : AssetsObject, IHaveName
    {
        public MeshObject(AssetsFile assetsFile) : base(assetsFile, AssetsConstants.ClassID.MeshAssetClassID)
        {
        }

        public MeshObject(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        public override void Parse(AssetsReader reader)
        {
            base.ParseBase(reader);
            int startPosition = reader.Position;
            Name = reader.ReadString();
            //int readLen = ObjectInfo.DataSize - (reader.Position - startPosition);
            //MeshData = reader.ReadBytes(readLen);
            SubMeshes = reader.ReadArrayOf(r => new Submesh(r));
            BlendShapeData = new BlendShapeData(reader);
            BindPose = reader.ReadArrayOf(r => reader.ReadSingle());
            BoneNameHashes = reader.ReadArrayOf(r => r.ReadUInt32());
            RootBoneNameHash = reader.ReadUInt32();
            MeshCompression = reader.ReadByte();
            IsReadable = reader.ReadBoolean();
            KeepVerticies = reader.ReadBoolean();
            KeepIndicies = reader.ReadBoolean();
            reader.AlignTo(4);
            IndexFormat = reader.ReadInt32();
            IndexBuffer = reader.ReadArray();
            reader.AlignTo(4);
            VertexData = new VertexData(reader);
            CompressedMesh = new CompressedMesh(reader);
            LocalAABB = new AABB(reader);
            MeshUsageFlags = reader.ReadInt32();
            BakedConvexCollisionMesh = reader.ReadArray();
            reader.AlignTo(4);
            BakedTriangleCollisionMesh = reader.ReadArray();
            reader.AlignTo(4);
            MeshMetrics1 = reader.ReadSingle();
            MeshMetrics2 = reader.ReadSingle();
            StreamData = new StreamingInfo(reader);
        }

        protected override void WriteObject(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.Write(Name);
            //writer.Write(MeshData);
            writer.WriteArrayOf(SubMeshes, (o, w) => o.Write(w));
            BlendShapeData.Write(writer);
            writer.WriteArrayOf(BindPose, (o, w) => w.Write(o));
            writer.WriteArrayOf(BoneNameHashes, (o, w) => w.Write(o));
            writer.Write(RootBoneNameHash);
            writer.Write(MeshCompression);
            writer.Write(IsReadable);
            writer.Write(KeepVerticies);
            writer.Write(KeepIndicies);
            writer.AlignTo(4);
            writer.Write(IndexFormat);
            writer.WriteArray(IndexBuffer);
            writer.AlignTo(4);
            VertexData.Write(writer);
            CompressedMesh.Write(writer);
            LocalAABB.Write(writer);
            writer.Write(MeshUsageFlags);
            writer.WriteArray(BakedConvexCollisionMesh);
            writer.AlignTo(4);
            writer.WriteArray(BakedTriangleCollisionMesh);
            writer.AlignTo(4);
            writer.Write(MeshMetrics1);
            writer.Write(MeshMetrics2);
            StreamData.Write(writer);
        }

        public string Name { get; set; }
        
        public List<Submesh> SubMeshes { get; set; }
        public BlendShapeData BlendShapeData { get; set; }
        public List<Single> BindPose { get; set; }
        public List<UInt32> BoneNameHashes { get; set; }
        public UInt32 RootBoneNameHash { get; set; }
        public byte MeshCompression { get; set; }
        public bool IsReadable { get; set; }
        public bool KeepVerticies { get; set; }
        public bool KeepIndicies { get; set; }
        public int IndexFormat { get; set; }
        public byte[] IndexBuffer { get; set; }
        public VertexData VertexData { get; set; }
        public CompressedMesh CompressedMesh { get; set; }
        public AABB LocalAABB { get; set; }
        public int MeshUsageFlags { get; set; }
        public byte[] BakedConvexCollisionMesh { get; set; }
        public byte[] BakedTriangleCollisionMesh { get; set; }
        public Single MeshMetrics1 { get; set; }
        public Single MeshMetrics2 { get; set; }
        public StreamingInfo StreamData { get; set; }
        
      //  public byte[] MeshData { get; set; }

        [System.ComponentModel.Browsable(false)]
        [Newtonsoft.Json.JsonIgnore]
        public override byte[] Data { get => throw new InvalidOperationException("Data cannot be accessed from this class!"); set => throw new InvalidOperationException("Data cannot be accessed from this class!"); }
    }
}
