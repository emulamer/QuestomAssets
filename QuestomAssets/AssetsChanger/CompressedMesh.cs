using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class CompressedMesh
    {
        public CompressedMesh()
        {

        }

        public CompressedMesh(AssetsReader reader)
        {
            Parse(reader);
        }

        public void Parse(AssetsReader reader)
        {
            Verticies = new PackedSingleVector(reader);
            UV = new PackedSingleVector(reader);
            Normals = new PackedSingleVector(reader);
            Tangents = new PackedSingleVector(reader);
            Weights = new PackedIntVector(reader);
            NormalSigns = new PackedIntVector(reader);
            TangentSigns = new PackedIntVector(reader);
            FloatColors = new PackedSingleVector(reader);
            BoneIndicies = new PackedIntVector(reader);
            Triangles = new PackedIntVector(reader);
            UVInfo = reader.ReadUInt32();
        }

        public void Write(AssetsWriter writer)
        {
            Verticies.Write(writer);
            UV.Write(writer);
            Normals.Write(writer);
            Tangents.Write(writer);
            Weights.Write(writer);
            NormalSigns.Write(writer);
            TangentSigns.Write(writer);
            FloatColors.Write(writer);
            BoneIndicies.Write(writer);
            Triangles.Write(writer);
            writer.Write(UVInfo);
        }

        public PackedSingleVector Verticies { get; set; }
        public PackedSingleVector UV { get; set; }
        public PackedSingleVector Normals { get; set; }
        public PackedSingleVector Tangents { get; set; }
        public PackedIntVector Weights { get; set; }
        public PackedIntVector NormalSigns { get; set; }
        public PackedIntVector TangentSigns { get; set; }
        public PackedSingleVector FloatColors { get; set; }
        public PackedIntVector BoneIndicies { get; set; }
        public PackedIntVector Triangles { get; set; }
        public UInt32 UVInfo { get; set; }

    }
}
