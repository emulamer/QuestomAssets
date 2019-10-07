using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class RectF
    {
        public RectF()
        {

        }
        public RectF(AssetsReader reader)
        {
            Parse(reader);
        }

        public Single X { get; set; }
        public Single Y { get; set; }
        public Single Width { get; set; }
        public Single Height { get; set; }

        public void Parse(AssetsReader reader)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Width = reader.ReadSingle();
            Height = reader.ReadSingle();
        }

        public void Write(AssetsWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Width);
            writer.Write(Height);
        }
    }
}
