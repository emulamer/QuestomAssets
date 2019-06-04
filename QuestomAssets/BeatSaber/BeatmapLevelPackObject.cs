using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestomAssets.AssetsChanger;

namespace QuestomAssets.BeatSaber
{
    public sealed class BeatmapLevelPackObject : MonoBehaviourObject, INeedAssetsMetadata
    {
        public BeatmapLevelPackObject(AssetsMetadata metadata) : base(metadata, BSConst.ScriptHash.BeatmapLevelPackScriptHash, BSConst.ScriptPtr.BeatmapLevelPackScriptPtr)
        { }

        public BeatmapLevelPackObject(ObjectInfo objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }
        public BeatmapLevelPackObject(ObjectInfo objectInfo) : base(objectInfo)
        { }

        public void UpdateTypes(AssetsMetadata metadata)
        {
            base.UpdateType(metadata, BSConst.ScriptHash.BeatmapLevelPackScriptHash, BSConst.ScriptPtr.BeatmapLevelPackScriptPtr);
        }

        public string PackID { get; set; }

        public string PackName { get; set; }

        public PPtr CoverImage { get; set; }

        public bool IsPackAlwaysOwned { get; set; }

        public PPtr BeatmapLevelCollection { get; set; }

        public override void Write(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.Write(PackID);
            writer.Write(PackName);
            CoverImage.Write(writer);
            writer.Write(IsPackAlwaysOwned);
            BeatmapLevelCollection.Write(writer);
        }

        protected override void Parse(AssetsReader reader)
        {
            base.Parse(reader);
            PackID = reader.ReadString();
            PackName = reader.ReadString();
            CoverImage = new PPtr(reader);
            IsPackAlwaysOwned = reader.ReadBoolean();
            BeatmapLevelCollection = new PPtr(reader);
        }

        public override byte[] ScriptParametersData
        {
            get
            {
                throw new InvalidOperationException("Cannot access parameters data from this object.");
            }
            set
            {
                throw new InvalidOperationException("Cannot access parameters data from this object.");
            }
        }
    }
}
