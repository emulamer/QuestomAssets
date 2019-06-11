using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class ExternalFile : INotifyPropertyChanged
    {
        public string AssetName { get; set; }
        public Guid ID { get; set; }
        public Int32 Type { get; set; }
        public string FileName { get; set; }

        public ExternalFile()
        { }

        public ExternalFile(AssetsReader reader)
        {
            Parse(reader);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Write(AssetsWriter writer)
        {
            writer.WriteCString(AssetName);
            writer.Write(ID);
            writer.Write(Type);
            writer.WriteCString(FileName);            
        }

        private void Parse(AssetsReader reader)
        {
            AssetName = reader.ReadCStr();
            ID = reader.ReadGuid();
            Type = reader.ReadInt32();
            FileName = reader.ReadCStr();
        }
    }
}
