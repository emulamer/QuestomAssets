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
            Weights = new PackedSingleVector(reader);
            NormalSigns = new PackedSingleVector(reader);
            TangentSigns = new PackedSingleVector(reader);
            FloatColors = new PackedSingleVector(reader);
            BoneIndicies = new PackedSingleVector(reader);
            Triangles = new PackedSingleVector(reader);
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
        }

        public PackedSingleVector Verticies { get; set; }
        public PackedSingleVector UV { get; set; }
        public PackedSingleVector Normals { get; set; }
        public PackedSingleVector Tangents { get; set; }
        public PackedSingleVector Weights { get; set; }
        public PackedSingleVector NormalSigns { get; set; }
        public PackedSingleVector TangentSigns { get; set; }
        public PackedSingleVector FloatColors { get; set; }
        public PackedSingleVector BoneIndicies { get; set; }
        public PackedSingleVector Triangles { get; set; }

    }
}
