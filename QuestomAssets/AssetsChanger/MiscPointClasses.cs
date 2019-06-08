using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class Vector3F
    {
        public Single X { get; set; }
        public Single Y { get; set; }
        public Single Z { get; set; }

        public Vector3F()
        { }

        public Vector3F(AssetsReader reader)
        {
            Parse(reader);
        }

        private void Parse(AssetsReader reader)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
        }

        public void Write(AssetsWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
        }
    }

    public class QuaternionF
    {

        public Single X { get; set; }
        public Single Y { get; set; }
        public Single Z { get; set; }
        public Single W { get; set; }

        public QuaternionF()
        { }

        public QuaternionF(AssetsReader reader)
        {
            Parse(reader);
        }

        private void Parse(AssetsReader reader)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
            W = reader.ReadSingle();
        }

        public void Write(AssetsWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
            writer.Write(W);
        }
    }
}
