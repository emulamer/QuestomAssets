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
            //Parse(reader);
        }
/*
        public void Parse(AssetsReader reader)
        {
            Verticies = reader.ReadArrayOf((r) => new Vector(r));
            Shapes = reader.ReadArrayOf((r) => new Vector(r));
            Channels = reader.ReadArrayOf((r) => new Vector(r));
            FullWeights = reader.ReadArrayOf((r) => new Vector(r));
        }

        public void Write(AssetsWriter writer)
        {
            writer.WriteArrayOf(Verticies, (o, w) => o.Write(w));
            writer.WriteArrayOf(Shapes, (o, w) => o.Write(w));
            writer.WriteArrayOf(Channels, (o, w) => o.Write(w));
            writer.WriteArrayOf(FullWeights, (o, w) => o.Write(w));
        }

        public List<Vector> Verticies { get; set; }
        public List<Vector> Shapes { get; set; }
        public List<Vector> Channels { get; set; }
        public List<Single> FullWeights { get; set; }*/
    }
}
