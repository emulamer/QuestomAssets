using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{


    public class RectTransform : Transform
    {
        public RectTransform(AssetsFile assetsFile) : base(assetsFile, AssetsConstants.ClassID.RectTransformClassID)
        {
        }

        public RectTransform(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        protected override void Parse(AssetsReader reader)
        {
            base.Parse(reader);
            AnchorMin = new Vector2F(reader);
            AnchorMax = new Vector2F(reader);
            AnchoredPosition = new Vector2F(reader);
            SizeDelta = new Vector2F(reader);
            Pivot = new Vector2F(reader);
        }

        protected override void WriteObject(AssetsWriter writer)
        {
            base.WriteBase(writer);
            AnchorMin.Write(writer);
            AnchorMax.Write(writer);
            AnchoredPosition.Write(writer);
            SizeDelta.Write(writer);
            Pivot.Write(writer);
        }

        public Vector2F AnchorMin { get; set; }
        public Vector2F AnchorMax { get; set; }
        public Vector2F AnchoredPosition { get; set; }
        public Vector2F SizeDelta { get; set; }
        public Vector2F Pivot { get; set; }
    }
}
