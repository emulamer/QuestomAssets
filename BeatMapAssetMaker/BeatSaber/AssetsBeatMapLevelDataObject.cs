using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatmapAssetMaker.AssetsChanger;

namespace BeatmapAssetMaker.BeatSaber
{
    public class AssetsBeatmapLevelDataObject : AssetsMonoBehaviourObject
    {
        public AssetsBeatmapLevelDataObject(AssetsObjectInfo objectInfo) : base(objectInfo)
        { }

        public AssetsBeatmapLevelDataObject(AssetsObjectInfo objectInfo, AssetsReader reader) : base(objectInfo, reader)
        { }

        private bool _changes;
        private BeatmapLevelDataSO _beatMapLevelData;
        public BeatmapLevelDataSO BeatMapLevelData
        {
            get
            {
                return _beatMapLevelData;
            }
            set
            {
                if (_beatMapLevelData != value)
                    _changes = true;

                _beatMapLevelData = value;
            }
        }

        private void SerializeScriptParameters()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var writer = new AlignedStream(ms);
                _beatMapLevelData.Write(writer);
                ScriptParametersData = ms.ToArray();
            }
        }

        private void DeserializeScriptParameters()
        {
            using (MemoryStream ms = new MemoryStream(ScriptParametersData))
            {
                using (AssetsReader reader = new AssetsReader(ms))
                {
                    _beatMapLevelData = new BeatmapLevelDataSO();

                    _beatMapLevelData._levelID = reader.ReadString();
                    _beatMapLevelData._songName = reader.ReadString();
                    _beatMapLevelData._songAuthorName = reader.ReadString();
                    _beatMapLevelData._levelAuthorName = reader.ReadString();
                    _beatMapLevelData._audioClip = new AssetsPtr(reader).ToUPtr();
                    _beatMapLevelData._beatsPerMinute = reader.ReadSingle();
                    _beatMapLevelData._songTimeOffset = reader.ReadSingle();
                    _beatMapLevelData._shuffle = reader.ReadSingle();
                    _beatMapLevelData._shufflePeriod = reader.ReadSingle();
                    _beatMapLevelData._previewStartTime = reader.ReadSingle();
                    _beatMapLevelData._previewDuration = reader.ReadSingle();
                    _beatMapLevelData._coverImageTexture2D = new AssetsPtr(reader).ToUPtr();
                    _beatMapLevelData._environmentSceneInfo = new AssetsPtr(reader).ToUPtr();
                    int numBeatmaps = reader.ReadInt32();
                    for (int i = 0; i < numBeatmaps; i++)
                    {
                        _beatMapLevelData._difficultyBeatmapSets.Add(DeserializeBeatmapSet(reader));
                    }                    
                }
            }
        }

        private DifficultyBeatmapSetSO DeserializeBeatmapSet(AssetsReader reader)
        {
            DifficultyBeatmapSetSO set = new DifficultyBeatmapSetSO();
            set._beatmapCharacteristic = new AssetsPtr(reader).ToUPtr();
            int numBeatmaps = reader.ReadInt32();
            for (int i = 0; i < numBeatmaps; i++)
            {
                set._difficultyBeatmaps.Add(DeserializeDifficultyBeatmap(reader));
            }
            return set;
        }

        private DifficultyBeatmapSO DeserializeDifficultyBeatmap(AssetsReader reader)
        {
            DifficultyBeatmapSO bm = new DifficultyBeatmapSO();
            bm._difficulty = (Difficulty)reader.ReadInt32();
            bm._difficultyRank = reader.ReadInt32();
            bm._noteJumpMovementSpeed = reader.ReadSingle();
            bm._noteJumpStartBeatOffset = reader.ReadInt32();
            bm._beatmapDataPtr = new AssetsPtr(reader).ToUPtr();
            return bm;
        }

    }
}
