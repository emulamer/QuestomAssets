using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BeatmapAssetMaker.AssetsChanger
{
    public class AssetsAudioClip : AssetsObject
    {
        private string _name;
        private int _loadType;
        private int _channels;
        private int _frequency;
        private int _bitsPerSample;
        private Single _length;
        private bool _isTrackerFormat;
        private bool _ambisonic;
        private int _subsoundIndex;
        private bool _preloadAudioData;
        private bool _loadInBackground;
        private bool _legacy3D;
        private AssetsStreamedResource _resource;
        private int _compressionFormat;

        private bool _changes;
        private byte[] _data;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                    _changes = true;
                _name = value;
            }
        }

        public int LoadType
        {
            get
            {
                return _loadType;
            }
            set
            {
                if (_loadType != value)
                    _changes = true;
                _loadType = value;
            }
        }

        public int Channels
        {
            get
            {
                return _channels;
            }
            set
            {
                if (_channels != value)
                    _changes = true;
                _channels = value;
            }
        }

        public int Frequency
        {
            get
            {
                return _frequency;
            }
            set
            {
                if (_frequency != value)
                    _changes = true;
                _frequency = value;
            }
        }

        public int BitsPerSample
        {
            get
            {
                return _bitsPerSample;
            }
            set
            {
                if (_bitsPerSample != value)
                    _changes = true;
                _bitsPerSample = value;
            }
        }

        public Single Length
        {
            get
            {
                return _length;
            }
            set
            {
                if (_length != value)
                    _changes = true;
                _length = value;
            }
        }

        public bool IsTrackerFormat
        {
            get
            {
                return _isTrackerFormat;
            }
            set
            {
                if (_isTrackerFormat != value)
                    _changes = true;
                _isTrackerFormat = value;
            }
        }

        public bool Ambisonic
        {
            get
            {
                return _ambisonic;
            }
            set
            {
                if (_ambisonic != value)
                    _changes = true;
                _ambisonic = value;
            }
        }

        public int SubsoundIndex
        {
            get
            {
                return _subsoundIndex;
            }
            set
            {
                if (_subsoundIndex != value)
                    _changes = true;
                _subsoundIndex = value;
            }
        }

        public bool PreloadAudioData
        {
            get
            {
                return _preloadAudioData;
            }
            set
            {
                if (_preloadAudioData != value)
                    _changes = true;
                _preloadAudioData = value;
            }
        }

        public bool LoadInBackground
        {
            get
            {
                return _loadInBackground;
            }
            set
            {
                if (_loadInBackground != value)
                    _changes = true;
                _loadInBackground = value;
            }
        }

        public bool Legacy3D
        {
            get
            {
                return _legacy3D;
            }
            set
            {
                if (_legacy3D != value)
                    _changes = true;
                _legacy3D = value;
            }
        }

        public AssetsStreamedResource Resource
        {
            get
            {
                return _resource;
            }
            set
            {
                if (_resource != value)
                    _changes = true;
                _resource = value;
            }
        }

        public int CompressionFormat
        {
            get
            {
                return _compressionFormat;
            }
            set
            {
                if (_compressionFormat != value)
                    _changes = true;
                _compressionFormat = value;
            }
        }


        public AssetsAudioClip(AssetsObjectInfo objectInfo) : base(objectInfo)
        { }

        public AssetsAudioClip(AssetsObjectInfo objectInfo, AssetsReader reader) : base(objectInfo, reader)
        {
        }

        protected override void Parse(AssetsReader reader)
        {
            _name = reader.ReadString();
            reader.AlignToObjectData(4);
            _loadType = reader.ReadInt32();
            _channels = reader.ReadInt32();
            _frequency = reader.ReadInt32();
            _bitsPerSample = reader.ReadInt32();
            _length = reader.ReadSingle();
            _isTrackerFormat = reader.ReadBoolean();
            _ambisonic = reader.ReadBoolean();
            //it seems after a serias of booleans, it always aligns
            reader.AlignToObjectData(4);
            _subsoundIndex = reader.ReadInt32();
            _preloadAudioData = reader.ReadBoolean();
            _loadInBackground = reader.ReadBoolean();
            _legacy3D = reader.ReadBoolean();
            reader.AlignToObjectData(4);
            _resource = new AssetsStreamedResource(reader);
            _compressionFormat = reader.ReadInt32();
            reader.AlignToObjectData(4);
        }

        private void SerializeToData()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (AssetsWriter writer = new AssetsWriter(ms))
                {
                    writer.Write(_name);
                    writer.AlignTo(4);
                    writer.Write(_loadType);
                    writer.Write(_channels);
                    writer.Write(_frequency);
                    writer.Write(_bitsPerSample);
                    writer.Write(_length);
                    writer.Write(_isTrackerFormat);
                    writer.Write(_ambisonic);
                    writer.AlignTo(4);
                    writer.Write(_subsoundIndex);
                    writer.Write(_preloadAudioData);
                    writer.Write(_loadInBackground);
                    writer.Write(_legacy3D);
                    writer.AlignTo(4);
                    _resource.Write(writer);
                    writer.Write(_compressionFormat);
                    //TODO: I think this might be some kind of alignment thing?  not sure.
                    //writer.Write((byte)0);
                    writer.AlignTo(4);
                }
                _data = ms.ToArray();
            }
        }

        private void DeserializeData(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (AssetsReader reader = new AssetsReader(ms))
                {
                    Parse(reader);
                }
            }
        }

        public override byte[] Data
        {
            get
            {
                if (_changes || _data == null)
                {
                    SerializeToData();
                    _changes = false;
                }
                return _data;
            }
            set
            {
                DeserializeData(value);
                _data = value;
                _changes = false;
            }


        }
    }
}
