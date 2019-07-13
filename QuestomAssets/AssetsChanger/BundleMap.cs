using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class BundleMap : Map<string, AssetInfo>
    {
        public BundleMap(AssetsFile assetsFile, AssetsObject owner, AssetsReader reader)
        {
            Parse(assetsFile, owner, reader);
        }

        private void Parse(AssetsFile assetsFile, AssetsObject owner, AssetsReader reader)
        {
            First = reader.ReadString();
            Second = new AssetInfo(assetsFile, owner, reader);
        }

        public void Write(AssetsWriter writer)
        {
            writer.Write(First);
            Second.Write(writer);
        }
    }
}
