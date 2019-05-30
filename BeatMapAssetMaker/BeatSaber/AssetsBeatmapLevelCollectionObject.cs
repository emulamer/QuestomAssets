using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatmapAssetMaker.AssetsChanger;
using Newtonsoft.Json;

namespace BeatmapAssetMaker.BeatSaber
{
    public sealed class AssetsBeatmapLevelCollectionObject : AssetsMonoBehaviourObject, INeedAssetsMetadata
    {
        public AssetsBeatmapLevelCollectionObject(AssetsMetadata metadata) : base(metadata, AssetsConstants.ScriptHash.BeatmapLevelCollectionScriptHash, AssetsConstants.ScriptPtr.BeatmapLevelCollectionScriptPtr)
        { }

        public AssetsBeatmapLevelCollectionObject(AssetsObjectInfo objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        public void UpdateTypes(AssetsMetadata metadata)
        {
            base.UpdateType(metadata, AssetsConstants.ScriptHash.BeatmapLevelCollectionScriptHash, AssetsConstants.ScriptPtr.BeatmapLevelCollectionScriptPtr);
        }

        public List<AssetsPtr> BeatmapLevels { get; set; } = new List<AssetsPtr>();

        protected override void Parse(AssetsReader reader)
        {
            base.Parse(reader);
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                BeatmapLevels.Add(new AssetsPtr(reader));
            }
        }

        public override void Write(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.Write(BeatmapLevels.Count());
            foreach (var bml in BeatmapLevels)
            {
                bml.Write(writer);
            }
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
