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
        private float _r;
        [JsonProperty("R")]
        public float R
        {
            get => _r;
            set
            {
                if (_r != value)
                    PropChanged(nameof(R));
                _r = value;
            }
        }
        private float _g;
        [JsonProperty("G")]
        public float G
        {
            get => _g;
            set
            {
                if (_g != value)
                    PropChanged(nameof(G));
                _g = value;
            }
        }
        private float _b;
        [JsonProperty("B")]
        public float B
        {
            get => _b;
            set
            {
                if (_b != value)
                    PropChanged(nameof(B));
                _b = value;
            }
        }
        private float _a;
        [JsonProperty("A")]
        public float A
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
