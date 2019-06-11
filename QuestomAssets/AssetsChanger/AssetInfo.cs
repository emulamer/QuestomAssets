using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class AssetInfo
    {
        public AssetInfo()
        { }

        public AssetInfo( AssetsFile assetsFile, AssetsObject owner, AssetsReader reader)
        {
            Parse(assetsFile, owner, reader);
        }

        private void Parse(AssetsFile assetsFile, AssetsObject owner, AssetsReader reader)
        {
            PreloadIndex = reader.ReadInt32();
            PreloadSize = reader.ReadInt32();
            Asset = SmartPtr<AssetsObject>.Read(assetsFile, owner, reader);
        }

        public void Write(AssetsWriter writer)
        {
            writer.Write(PreloadIndex);
            writer.Write(PreloadSize);
            Asset.WritePtr(writer);
        }

        public Int32 PreloadIndex { get; set; }
        public Int32 PreloadSize { get; set; }
        public ISmartPtr<AssetsObject> Asset { get; set; }
    }
}
