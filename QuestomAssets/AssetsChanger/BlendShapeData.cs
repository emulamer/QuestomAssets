using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class BlendShapeData
    {
        public BlendShapeData()
        { }
        public BlendShapeData(AssetsReader reader)
        {
            Parse(reader);
        }

        public void Parse(AssetsReader reader)
        {
            Verticies = reader.ReadArrayOf((r) => new BlendShapeVertex(r));
            Shapes = reader.ReadArrayOf((r) => new BlendShape(r));
            Channels = reader.ReadArrayOf((r) => new BlendShapeChannel(r));
            FullWeights = reader.ReadArrayOf((r) => r.ReadSingle());
        }

        public void Write(AssetsWriter writer)
        {
            writer.WriteArrayOf(Verticies, (o, w) => o.Write(w));
            writer.WriteArrayOf(Shapes, (o, w) => o.Write(w));
            writer.WriteArrayOf(Channels, (o, w) => o.Write(w));
            writer.WriteArrayOf(FullWeights, (o, w) => w.Write(o));
        }

        public List<BlendShapeVertex> Verticies { get; set; }
        public List<BlendShape> Shapes { get; set; }
        public List<BlendShapeChannel> Channels { get; set; }
        public List<Single> FullWeights { get; set; }


        public class BlendShapeChannel
        {
            public string Name { get; set; }
            public UInt32 NameHash { get; set; }
            public int FrameIndex { get; set; }
            public int FrameCount { get; set; }

            public BlendShapeChannel()
            { }

            public BlendShapeChannel(AssetsReader reader)
            {
                Parse(reader);
            }

            public void Parse(AssetsReader reader)
            {
                Name = reader.ReadString();
                NameHash = reader.ReadUInt32();
                FrameIndex = reader.ReadInt32();
                FrameCount = reader.ReadInt32();
            }

            public void Write(AssetsWriter writer)
            {
                writer.Write(Name);
                writer.Write(NameHash);
                writer.Write(FrameIndex);
                writer.Write(FrameCount);
            }
        }
        public class BlendShape
        {
            public UInt32 FirstVertex { get; set; }
            public UInt32 VertexCount { get; set; }
            public bool HasNormals { get; set; }
            public bool HasTangents { get; set; }

            public BlendShape()
            {
            }

            public BlendShape(AssetsReader reader)
            {
                Parse(reader);
            }

            public void Parse(AssetsReader reader)
            {
                FirstVertex = reader.ReadUInt32();
                VertexCount = reader.ReadUInt32();
                HasNormals = reader.ReadBoolean();
                HasTangents = reader.ReadBoolean();
                reader.AlignTo(4);
            }

            public void Write(AssetsWriter writer)
            {
                writer.Write(FirstVertex);
                writer.Write(VertexCount);
                writer.Write(HasNormals);
                writer.Write(HasTangents);
                writer.AlignTo(4);
            }
        }

        public class BlendShapeVertex
        {
            public Vector3F Vertex { get; set; }
            public Vector3F Normal { get; set; }
            public Vector3F Tangent { get; set; }
            public uint Index { get; set; }

            public BlendShapeVertex()
            { }

            public BlendShapeVertex(AssetsReader reader)
            {
                Parse(reader);
            }

            public void Parse(AssetsReader reader)
            {
                Vertex = new Vector3F(reader);
                Normal = new Vector3F(reader);
                Tangent = new Vector3F(reader);
                Index = reader.ReadUInt32();
            }

            public void Write(AssetsWriter writer)
            {
                Vertex.Write(writer);
                Normal.Write(writer);
                Tangent.Write(writer);
                writer.Write(Index);
            }
        }
    }
}
