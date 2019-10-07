using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class AssetsType
    {
        public Int32 ClassID { get; set; }
        public SByte Unknown1 { get; set; }
        public Int16 Unknown2 { get; set; }
        public bool IsScriptType
        {
            get { return ClassID == AssetsConstants.ClassID.MonoBehaviourScriptType; }
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

        public TypeTree TypeTree { get; set; }

        public AssetsType(AssetsReader reader, bool hasTypeTrees = false)
        {
            Parse(reader, hasTypeTrees);
        }
        internal AssetsType()
        { }

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

            if (TypeTree != null)
                TypeTree.Write(writer);
                
        }       

        private void Parse(AssetsReader reader, bool hasTypeTree)
        {
            ClassID = reader.ReadInt32();
            Unknown1 = reader.ReadSByte();
            Unknown2 = reader.ReadInt16();
            if (IsScriptType)
            {
                ScriptHash = reader.ReadGuid();
            }
            TypeHash = reader.ReadGuid();
            if (hasTypeTree)
            {
                TypeTree = new TypeTree(reader);
            }
        }

        public AssetsType CloneWithoutTypeTree()
        {
            var clone = new AssetsType()
            {
                ClassID = this.ClassID,
                TypeHash = this.TypeHash,
                Unknown1 = this.Unknown1,
                Unknown2 = this.Unknown2
            };
            if (IsScriptType)
                clone.ScriptHash = this.ScriptHash;

            return clone;
        }
    }

    /// <summary>
    /// I'll make it a tree later if I need to
    /// </summary>
    public class TypeTree
    {
        public List<TypeTreeEntry> Entries { get; } = new List<TypeTreeEntry>();

        public TypeTree(AssetsReader reader)
        {
            Parse(reader);
        }

        private void Parse(AssetsReader reader)
        {
            int numTreeNodes = reader.ReadInt32();
            int stringBufferSize = reader.ReadInt32();
            for (int i = 0; i < numTreeNodes; i++)
            {
                Entries.Add(new TypeTreeEntry(reader));
            }
            StringBuffer = new String(reader.ReadChars(stringBufferSize));
        }

        public void Write(AssetsWriter writer)
        {
            writer.Write(Entries.Count());
            writer.Write(StringBuffer.Length);
            foreach (var entry in Entries)
            {
                entry.Write(writer);
            }
            writer.WriteChars(StringBuffer);
        }

        public string StringBuffer { get; set; }
        public class TypeTreeEntry
        {
            public Int16 Version { get; set; }
            public SByte Depth { get; set; }
            public bool IsArray { get; set; }
            public int TypeOffset { get; set; }
            public int NameOffset { get; set; }
            public int Size { get; set; }
            public int Index { get; set; }
            public int Flags { get; set; }
            public TypeTreeEntry(AssetsReader reader)
            {
                Parse(reader);
            }

            private void Parse(AssetsReader reader)
            {
                Version = reader.ReadInt16();
                Depth = reader.ReadSByte();
                IsArray = reader.ReadBoolean();
                TypeOffset = reader.ReadInt32();
                NameOffset = reader.ReadInt32();
                Size = reader.ReadInt32();
                Index = reader.ReadInt32();
                Flags = reader.ReadInt32();
            }

            public void Write(AssetsWriter writer)
            {
                writer.Write(Version);
                writer.Write(Depth);
                writer.Write(IsArray);
                writer.Write(TypeOffset);
                writer.Write(NameOffset);
                writer.Write(Size);
                writer.Write(Index);
                writer.Write(Flags);
            }
        }
    }
    

    

    
}
