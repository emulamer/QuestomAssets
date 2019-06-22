using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace QuestomAssets
{
    public class SaberModel : INotifyPropertyChanged
    {
        private string _saberID;
        //this will map to the Name minus "Saber".  the default is "Basic"
        public string SaberID
        {
            get => _saberID;
            set
            {
                if (_saberID != value)
                    PropChanged(nameof(SaberID));
                _saberID = value;
            }
        }

        private string _customSaberFolder;
        public string CustomSaberFolder
        {
            get => _customSaberFolder;
            set
            {
                if (_customSaberFolder != value)
                    PropChanged(nameof(CustomSaberFolder));
                _customSaberFolder = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void PropChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
