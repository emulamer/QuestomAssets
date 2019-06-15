using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class MeshObject : AssetsObject, IHaveName
    {
        //public object SubMeshes { get; set; }

        //public object Shapes { get; set; }
        //public object BindPose { get; set; }
        //public object BoneNameHashes { get; set; }
        //public object RootBoneNameHash { get; set; }
        //public object MeshCompression { get; set; }
        //public object IsReadable { get; set; }
        //public object KeepVertices { get; set; }
        //public object KeepIndices { get; set; }
        //public object IndexFormat { get; set; }
        //public byte[] IndexBuffer { get; set; }
        //public object VertexData { get; set; }
        //public object CompressedMesh { get; set; }
        //public object LocalAABB { get; set; }
        //public object Center { get; set; }
        //public object Extent { get; set; }
        //public object MeshUsageFlags { get; set; }
        //public object BakedConvexCollisionMesh { get; set; }
        //public object BakedTriangleCollisionMesh { get; set; }
        //public object MeshMetrics0 { get; set; }
        //public object MeshMetrics1 { get; set; }
        //public StreamingInfo StreamData { get; set; }


        public MeshObject(AssetsFile assetsFile) : base(assetsFile, AssetsConstants.ClassID.MeshAssetClassID)
        {
        }

        public MeshObject(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        protected override void Parse(AssetsReader reader)
        {
            base.Parse(reader);
            int startPosition = reader.Position;
            Name = reader.ReadString();
            int readLen = ObjectInfo.DataSize - (reader.Position - startPosition);
            MeshData = reader.ReadBytes(readLen);
        }
        protected override void WriteObject(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.Write(Name);
            writer.Write(MeshData);
        }

        public string Name { get; set; }

        public byte[] MeshData { get; set; }


        public override byte[] Data
        {
            get
            {
                throw new InvalidOperationException("Data cannot be accessed from this class.");
            }
            set
            {
                throw new InvalidOperationException("Data cannot be accessed from this class.");
            }


        }
    }
}
