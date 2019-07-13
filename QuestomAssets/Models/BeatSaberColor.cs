using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace QuestomAssets.Models
{
    ///////////////////////////TODO HOOK UP PROPERTYCHANGED
    public class BeatSaberColor : INotifyPropertyChanged
    {
        private byte _r;
        [JsonProperty("R")]
        public byte R
        {
            get => _r;
            set
            {
                if (_r != value)
                    PropChanged(nameof(R));
                _r = value;
            }
        }
        private byte _g;
        [JsonProperty("G")]
        public byte G
        {
            get => _g;
            set
            {
                if (_g != value)
                    PropChanged(nameof(G));
                _g = value;
            }
        }
        private byte _b;
        [JsonProperty("B")]
        public byte B
        {
            get => _b;
            set
            {
                if (_b != value)
                    PropChanged(nameof(B));
                _b = value;
            }
        }
        private byte _a;
        [JsonProperty("A")]
        public byte A
        {
            get => _a;
            set
            {
                if (_a != value)
                    PropChanged(nameof(A));
                _a = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void PropChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
