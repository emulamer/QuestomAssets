using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BeatmapAssetMaker.AssetsChanger
{
    public class AssetsType
    {
        

        public Int32 ClassID { get; set; }
        public SByte Unknown1 { get; set; }
        public Int16 Unknown2 { get; set; }
        public bool IsScriptType
        {
            get { return ClassID == AssetsConstants.MonoBehaviourScriptType; }
        }
        private Guid _scriptHash;
        public Guid ScriptHash
        {
            get
            {
                if (!IsScriptType)
                {
                    return Guid.Empty;
                }
                return _scriptHash;
            }
            set
            {
                if (!IsScriptType)
                {
                    throw new InvalidOperationException("ScriptHash can only be assigned if the type is a MonoBehaviour.");
                }
                _scriptHash = value;
            }
        }

        public Guid TypeHash { get; set; }

        public AssetsType(AssetsReader reader, bool hasTypeTrees = false)
        {
            if (hasTypeTrees == true)
                throw new NotImplementedException("Type Trees aren't supported!");

            Parse(reader);
        }

        public void Write(AssetsWriter writer)
        {
            writer.Write(ClassID);
            writer.Write(Unknown1);
            writer.Write(Unknown2);
            if (IsScriptType)
            {
                writer.Write(ScriptHash);                
            }
            writer.Write(TypeHash);
                
        }       

        private void Parse(AssetsReader reader)
        {
            ClassID = reader.ReadInt32();
            Unknown1 = reader.ReadSByte();
            Unknown2 = reader.ReadInt16();
            if (IsScriptType)
            {
                ScriptHash = reader.ReadGuid();
            }
            TypeHash = reader.ReadGuid();
            //type trees would go here
        }

       

    }

    

    
}
