using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace QuestomAssets.Models
{

    ///////////////////////////TODO HOOK UP PROPERTYCHANGED
    public class BeatSaberText
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void PropChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
