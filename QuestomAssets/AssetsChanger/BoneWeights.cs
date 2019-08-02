using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class BoneWeights
    {
        public Single[] Weights { get; set; } = new float[4];
        public int[] BoneIndexes { get; set; } = new int[4];

        public BoneWeights()
        {
        }

        public BoneWeights(AssetsReader reader)
        {
            Parse(reader);
        }

        public void Parse(AssetsReader reader)
        {
            Weights[0] = reader.ReadSingle();
            Weights[1] = reader.ReadSingle();
            Weights[2] = reader.ReadSingle();
            Weights[3] = reader.ReadSingle();
            BoneIndexes[0] = reader.ReadInt32();
            BoneIndexes[1] = reader.ReadInt32();
            BoneIndexes[2] = reader.ReadInt32();
            BoneIndexes[3] = reader.ReadInt32();
        }

        public void Write(AssetsWriter writer)
        {
            writer.Write(Weights[0]);
            writer.Write(Weights[1]);
            writer.Write(Weights[2]);
            writer.Write(Weights[3]);
            writer.Write(BoneIndexes[0]);
            writer.Write(BoneIndexes[1]);
            writer.Write(BoneIndexes[2]);
            writer.Write(BoneIndexes[3]);
        }
    }
}
