﻿using System;
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
        public BeatmapLevelCollectionObject(AssetsFile assetsFile) : base(assetsFile, assetsFile.Manager.GetScriptObject("BeatmapLevelCollectionSO"))
        {
            BeatmapLevels = new List<ISmartPtr<BeatmapLevelDataObject>>();
        }

        public BeatmapLevelCollectionObject(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo)
        {
            BeatmapLevels = new List<ISmartPtr<BeatmapLevelDataObject>>();
            Parse(reader);
        }

        public List<ISmartPtr<BeatmapLevelDataObject>> BeatmapLevels { get; } 

        public override void Parse(AssetsReader reader)
        {
            base.ParseBase(reader);
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                BeatmapLevels.Add(SmartPtr<BeatmapLevelDataObject>.Read(ObjectInfo.ParentFile, this, reader));
            }
        }

        protected override void WriteObject(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.Write(BeatmapLevels.Count());
            foreach (var bml in BeatmapLevels)
            {
                bml.Write(writer);
            }
        }

        [System.ComponentModel.Browsable(false)]
        [Newtonsoft.Json.JsonIgnore]
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
