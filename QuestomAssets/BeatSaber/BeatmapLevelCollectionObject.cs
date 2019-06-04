using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestomAssets.AssetsChanger;
using Newtonsoft.Json;

namespace QuestomAssets.BeatSaber
{
    public sealed class BeatmapLevelCollectionObject : MonoBehaviourObject, INeedAssetsMetadata
    {
        public BeatmapLevelCollectionObject(AssetsMetadata metadata) : base(metadata, BSConst.ScriptHash.BeatmapLevelCollectionScriptHash, BSConst.ScriptPtr.BeatmapLevelCollectionScriptPtr)
        { }

        public BeatmapLevelCollectionObject(ObjectInfo objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        public void UpdateTypes(AssetsMetadata metadata)
        {
            base.UpdateType(metadata, BSConst.ScriptHash.BeatmapLevelCollectionScriptHash, BSConst.ScriptPtr.BeatmapLevelCollectionScriptPtr);
        }

        public List<PPtr> BeatmapLevels { get; set; } = new List<PPtr>();

        protected override void Parse(AssetsReader reader)
        {
            base.Parse(reader);
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                BeatmapLevels.Add(new PPtr(reader));
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
