using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BeatmapAssetMaker.AssetsChanger
{
    public class AssetsFile
    {

        public AssetsFileHeader Header { get; set; }

        public AssetsMetadata Metadata { get; set; }

        public List<AssetsObject> Objects { get; set; }

        private Dictionary<Guid, Type> _scriptHashToTypes = new Dictionary<Guid, Type>();

        public AssetsFile(Stream assetsFileStream, Dictionary<Guid, Type> scriptHashToTypes)
        {
            _scriptHashToTypes = scriptHashToTypes;
            Objects = new List<AssetsObject>();

            assetsFileStream.Seek(0, SeekOrigin.Begin);

            using (AssetsReader reader = new AssetsReader(assetsFileStream, false))
            {
                Header = new AssetsFileHeader(reader);
            }
            using (AssetsReader reader = new AssetsReader(assetsFileStream, false))
            {
                Metadata = new AssetsMetadata(reader);
            }
            assetsFileStream.Seek(Header.ObjectDataOffset, SeekOrigin.Begin);
            using (AssetsReader reader = new AssetsReader(assetsFileStream))
            {
                foreach (var objectInfo in Metadata.ObjectInfos)
                {
                    AssetsObject assetsObject = ReadObject(reader, objectInfo);
                    Objects.Add(assetsObject);
                }
            }
        }

        private AssetsObject ReadObject(AssetsReader reader, ObjectInfo objectInfo)
        {
            AssetsObject assetsObject;
            var objectType = Metadata.Types[objectInfo.TypeIndex];
            switch (objectType.ClassID)
            {
                case AssetsConstants.ClassID.MonoBehaviourScriptType:
                    if (_scriptHashToTypes.ContainsKey(objectType.ScriptHash))
                    {
                        Type assetObjectType = _scriptHashToTypes[objectType.ScriptHash];
                        if (!assetObjectType.IsSubclassOf(typeof(MonoBehaviourObject)))
                        {
                            throw new ArgumentException("Types provided in scriptHashToTypes must be a subclass of AssetsMonoBehaviourObject.");
                        }
                        assetsObject = (AssetsObject)Activator.CreateInstance(assetObjectType, objectInfo, reader);
                    }
                    else
                    {
                        assetsObject = new MonoBehaviourObject(objectInfo, reader);
                    }
                    break;
                case AssetsConstants.ClassID.AudioClipClassID:
                    assetsObject = new AudioClipObject(objectInfo, reader);
                    break;
                case AssetsConstants.ClassID.Texture2DClassID:
                    assetsObject = new Texture2DObject(objectInfo, reader);
                    break;
                default:
                    assetsObject = new AssetsObject(objectInfo, reader);
                    break;
            }
            // is this needed?
            //reader.AlignToObjectData(8);
            return assetsObject;
        }



        public void Write(Stream outputStream)
        {
            MemoryStream objectsMS = new MemoryStream();
            MemoryStream metaMS = new MemoryStream();
            using (AssetsWriter writer = new AssetsWriter(objectsMS))
            {
                int ctr = 0;
                foreach (var obj in Objects)
                {
                    ctr++;
                    obj.ObjectInfo.DataOffset = (int)objectsMS.Position;
                    obj.Write(writer);
                    writer.Flush();
                    var origSize = obj.ObjectInfo.DataSize;
                    obj.ObjectInfo.DataSize = (int)(objectsMS.Position - obj.ObjectInfo.DataOffset);
                    writer.AlignTo(8);
                }
            }
            using (AssetsWriter writer = new AssetsWriter(metaMS))
            {
                Metadata.Write(writer);
            }

            Header.FileSize = Header.HeaderSize + (int)objectsMS.Length + (int)metaMS.Length;
            Header.ObjectDataOffset = Header.HeaderSize + (int)metaMS.Length;

            int objectPad = 4;
            int diff = (int)(Header.HeaderSize + metaMS.Length) % objectPad;
            if (diff > 0 && diff < objectPad)
            {
                Header.ObjectDataOffset += diff;
                Header.FileSize += diff;
            }

            Header.MetadataSize = (int)metaMS.Length;
            objectsMS.Seek(0, SeekOrigin.Begin);
            metaMS.Seek(0, SeekOrigin.Begin);

            using (AssetsWriter writer = new AssetsWriter(outputStream))
            {
                Header.Write(writer);
            }
            metaMS.CopyTo(outputStream);

            if (diff > 0 && diff < objectPad)
            {
                outputStream.Write(new byte[diff], 0, diff);
            }

            objectsMS.CopyTo(outputStream);
        }

        public long GetNextObjectID()
        {
            return Metadata.ObjectInfos.Max(x => x.ObjectID) + 1;
        }

        public void AddObject(AssetsObject assetsObject, bool assignNextObjectID = true)
        {
            if (assetsObject.ObjectInfo == null)
                throw new ArgumentException("ObjectInfo must be set!");

            if (assignNextObjectID)
            {
                assetsObject.ObjectInfo.ObjectID = GetNextObjectID();
            }
            if (assetsObject.ObjectInfo.ObjectID < 1)
                throw new ArgumentException("ObjectInfo.ObjectID must be > 0.");
            if (Metadata.ObjectInfos.Exists(x => x.ObjectID == assetsObject.ObjectInfo.ObjectID))
                throw new ArgumentException("ObjectInfo.ObjectID already exists in this file.");

            Metadata.ObjectInfos.Add(assetsObject.ObjectInfo);
            Objects.Add(assetsObject);
        }
    }
}
