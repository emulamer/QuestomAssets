//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;

//namespace QuestomAssets.AssetsChanger
//{
//    public class MeshObject : AssetsObject
//    {
//        public object SubMeshes { get; set; }

//        public object Shapes { get; set; }
//        public object BindPose { get; set; }
//        public object BoneNameHashes { get; set; }
//        public object RootBoneNameHash { get; set; }
//        public object MeshCompression { get; set; }
//        public object IsReadable { get; set; }
//        public object KeepVertices { get; set; }
//        public object KeepIndices { get; set; }
//        public object IndexFormat { get; set; }
//        public byte[] IndexBuffer { get; set; }
//        public object VertexData { get; set; }
//        public object CompressedMesh { get; set; }
//        public object LocalAABB { get; set; }
//        public object Center { get; set; }
//    public object Extent { get; set; }
//public object MeshUsageFlags { get; set; }
//  public object BakedConvexCollisionMesh { get; set; }
//  public object BakedTriangleCollisionMesh { get; set; }
//  public object MeshMetrics0 { get; set; }
//  public object MeshMetrics1 { get; set; }
//  public StreamingInfo StreamData { get; set; }
//        /*
//         * m_Name: "SaberHandle"
//  m_SubMeshes:
//  - serializedVersion: 2
//    firstByte: 0
//    indexCount: 192
//    topology: 0
//    baseVertex: 0
//    firstVertex: 0
//    vertexCount: 66
//    localAABB:
//      m_Center: {x: 0, y: 0.0858435, z: 0}
//      m_Extent: {x: 0.016981, y: 0.0791765, z: 0.016981}
//  m_Shapes:
//    vertices: []
//    shapes: []
//    channels: []
//    fullWeights: 
//  m_BindPose: []
//  m_BoneNameHashes: 
//  m_RootBoneNameHash: 0
//  m_MeshCompression: 0
//  m_IsReadable: 0
//  m_KeepVertices: 1
//  m_KeepIndices: 1
//  m_IndexFormat: 0
//  m_IndexBuffer: byte[384]{....}
//  m_VertexData:
//    serializedVersion: 2
//    m_VertexCount: 66
//    m_Channels:
//    - stream: 0
//      offset: 0
//      format: 0
//      dimension: 3
//    - stream: 0
//      offset: 12
//      format: 0
//      dimension: 3
//    - stream: 0
//      offset: 0
//      format: 0
//      dimension: 0
//    - stream: 0
//      offset: 0
//      format: 0
//      dimension: 0
//    - stream: 0
//      offset: 24
//      format: 0
//      dimension: 2
//    - stream: 0
//      offset: 0
//      format: 0
//      dimension: 0
//    - stream: 0
//      offset: 0
//      format: 0
//      dimension: 0
//    - stream: 0
//      offset: 0
//      format: 0
//      dimension: 0
//    - stream: 0
//      offset: 0
//      format: 0
//      dimension: 0
//    - stream: 0
//      offset: 0
//      format: 0
//      dimension: 0
//    - stream: 0
//      offset: 0
//      format: 0
//      dimension: 0
//    - stream: 0
//      offset: 0
//      format: 0
//      dimension: 0
//    - stream: 0
//      offset: 0
//      format: 0
//      dimension: 0
//    - stream: 0
//      offset: 0
//      format: 0
//      dimension: 0
//    m_DataSize: byte[2112]{....}
//  m_CompressedMesh:
//    m_Vertices:
//      m_NumItems: 0
//      m_Range: 0
//      m_Start: 0
//      m_Data: 
//      m_BitSize: 0
//    m_UV:
//      m_NumItems: 0
//      m_Range: 0
//      m_Start: 0
//      m_Data: 
//      m_BitSize: 0
//    m_Normals:
//      m_NumItems: 0
//      m_Range: 0
//      m_Start: 0
//      m_Data: 
//      m_BitSize: 0
//    m_Tangents:
//      m_NumItems: 0
//      m_Range: 0
//      m_Start: 0
//      m_Data: 
//      m_BitSize: 0
//    m_Weights:
//      m_NumItems: 0
//      m_Data: 
//      m_BitSize: 0
//    m_NormalSigns:
//      m_NumItems: 0
//      m_Data: 
//      m_BitSize: 0
//    m_TangentSigns:
//      m_NumItems: 0
//      m_Data: 
//      m_BitSize: 0
//    m_FloatColors:
//      m_NumItems: 0
//      m_Range: 0
//      m_Start: 0
//      m_Data: 
//      m_BitSize: 0
//    m_BoneIndices:
//      m_NumItems: 0
//      m_Data: 
//      m_BitSize: 0
//    m_Triangles:
//      m_NumItems: 0
//      m_Data: 
//      m_BitSize: 0
//    m_UVInfo: 0
//  m_LocalAABB:
//    m_Center: {x: 0, y: 0.0858435, z: 0}
//    m_Extent: {x: 0.016981, y: 0.0791765, z: 0.016981}
//  m_MeshUsageFlags: 0
//  m_BakedConvexCollisionMesh: 
//  m_BakedTriangleCollisionMesh: 
//  m_MeshMetrics[0]: 1
//  m_MeshMetrics[1]: 1
//  m_StreamData:
//    offset: 0
//    size: 0
//    path: ""

//         */

        
//        public MeshObject(ObjectInfo objectInfo) : base(objectInfo)
//        { }

//        public MeshObject(ObjectInfo objectInfo, AssetsReader reader) : base(objectInfo)
//        {
//            Parse(reader);
            
            
//        }

//        public MeshObject(AssetsMetadata metadata) : base(metadata, AssetsConstants.ClassID.AudioClipClassID)
//        { }

//        protected override void Parse(AssetsReader reader)
//        {
//            base.Parse(reader);
//           //parse here
//        }
//        public override void Write(AssetsWriter writer)
//        {
//            base.WriteBase(writer);
//           //write here
//        }

        

//        public override byte[] Data
//        {
//            get
//            {
//                throw new InvalidOperationException("Data cannot be accessed from this class.");
//            }
//            set
//            {
//                throw new InvalidOperationException("Data cannot be accessed from this class.");
//            }


//        }
//    }
//}
