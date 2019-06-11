using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class Map : INotifyPropertyChanged
    {
        public Map()
        { }

        public Map(AssetsFile assetsFile, AssetsObject owner, AssetsReader reader)
        {
            Parse(assetsFile, owner, reader);
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        public string First { get; set; }
        public AssetInfo Second { get; set; }
    }
}
