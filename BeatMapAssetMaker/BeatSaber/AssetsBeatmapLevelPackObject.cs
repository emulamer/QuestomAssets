using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatmapAssetMaker.AssetsChanger;

namespace BeatmapAssetMaker.BeatSaber
{
    public sealed class AssetsBeatmapLevelPackObject : AssetsMonoBehaviourObject, INeedAssetsMetadata
    {
        public AssetsBeatmapLevelPackObject(AssetsMetadata metadata) : base(metadata, AssetsConstants.ScriptHash.BeatmapLevelPackScriptHash, AssetsConstants.ScriptPtr.BeatmapLevelPackScriptPtr)
        { }

        public AssetsBeatmapLevelPackObject(AssetsObjectInfo objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }
        public AssetsBeatmapLevelPackObject(AssetsObjectInfo objectInfo) : base(objectInfo)
        { }

        public void UpdateTypes(AssetsMetadata metadata)
        {
            base.UpdateType(metadata, AssetsConstants.ScriptHash.BeatmapLevelPackScriptHash, AssetsConstants.ScriptPtr.BeatmapLevelPackScriptPtr);
        }

        public string PackID { get; set; }

        public string PackName { get; set; }

        public AssetsPtr CoverImage { get; set; }

        public bool IsPackAlwaysOwned { get; set; }

        public AssetsPtr BeatmapLevelCollection { get; set; }

        public override void Write(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.Write(PackID);
            writer.AlignTo(4);
            writer.Write(PackName);
            writer.AlignTo(4);
            CoverImage.Write(writer);
            writer.Write(IsPackAlwaysOwned);
            writer.AlignTo(4);
            BeatmapLevelCollection.Write(writer);
        }

        protected override void Parse(AssetsReader reader)
        {
            base.Parse(reader);
            PackID = reader.ReadString();
            reader.AlignToObjectData(4);
            PackName = reader.ReadString();
            reader.AlignToObjectData(4);
            CoverImage = new AssetsPtr(reader);
            IsPackAlwaysOwned = reader.ReadBoolean();
            reader.AlignTo(4);
            BeatmapLevelCollection = new AssetsPtr(reader);
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
