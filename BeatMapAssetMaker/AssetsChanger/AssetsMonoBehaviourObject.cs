using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BeatmapAssetMaker.AssetsChanger
{
    public class AssetsMonoBehaviourObject : AssetsObject
    {
        private bool _changes = false;

        
        private AssetsPtr _gameObjectPtr;
        private int _enabled;
        private string _name;
        private AssetsPtr _monoscriptTypePointer;
        private byte[] _scriptParametersData;


        public AssetsPtr GameObjectPtr
        {
            get
            {
                return _gameObjectPtr;
            }
            set
            {
                if (_gameObjectPtr != value)
                {
                    _changes = true;
                }
                _gameObjectPtr = value;
            }
        }
        
        public int Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                if (_enabled != value)
                {
                    _changes = true;
                }
                _enabled = value;
            }
        }
        
        public AssetsPtr MonoscriptTypePtr { get
            {
                return _monoscriptTypePointer;
            }
            set
            { 
                if (_monoscriptTypePointer != value)
                {
                    _changes = true;
                }
                _monoscriptTypePointer = value;
            }
        }
        
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _changes = true;
                }
                _name = value;
            }
        }

        public virtual byte[] ScriptParametersData
        {
            get
            {
                return _scriptParametersData;
            }
            set
            {
                if (_scriptParametersData != value || (value != null && value.SequenceEqual(_scriptParametersData)))
                {
                    _changes = true;
                }
                _scriptParametersData = value;
            }
        }

        public AssetsMonoBehaviourObject(AssetsObjectInfo objectInfo) : base(objectInfo)
        { _enabled = 1; }
        
        public AssetsMonoBehaviourObject(AssetsObjectInfo objectInfo, AssetsReader reader) : base(objectInfo, reader)
        { }

        private void ParseProperties(AssetsReader reader)
        {
            _gameObjectPtr = new AssetsPtr(reader);
            _enabled = reader.ReadInt32();
            _monoscriptTypePointer = new AssetsPtr(reader);
            _name = reader.ReadString();
            reader.AlignToObjectData(4);
        }

        protected override void Parse(AssetsReader reader)
        {
            var startPos = reader.Position;
            ParseProperties(reader);
            _changes = false;
            var readLength = ObjectInfo.DataSize-(reader.Position - startPos);
            _scriptParametersData = reader.ReadBytes(readLength);

        }

        private byte[] _data;

        private void SerializeToData()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (AssetsWriter bw = new AssetsWriter(ms))
                {
                    _gameObjectPtr.Write(bw);
                    bw.Write(_enabled);
                    _monoscriptTypePointer.Write(bw);
                    bw.Write(_name);
                    bw.AlignTo(4);
                    bw.Write(_scriptParametersData);
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
                    ParseProperties(reader);
                    _scriptParametersData = reader.ReadBytes((int)(ms.Length - ms.Position));
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
