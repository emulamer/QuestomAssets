using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using QuestomAssets.AssetsChanger;

namespace QuestomAssets.BeatSaber
{
    public sealed class BeatmapLevelDataObject : MonoBehaviourObject, INeedAssetsMetadata
    {
        public BeatmapLevelDataObject()
        { }

        public BeatmapLevelDataObject(ObjectInfo objectInfo) : base(objectInfo)
        { }

        public BeatmapLevelDataObject(ObjectInfo objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        public BeatmapLevelDataObject(AssetsMetadata metadata) : base(metadata, AssetsConstants.ScriptHash.BeatmapLevelDataHash, AssetsConstants.ScriptPtr.BeatmapLevelDataScriptPtr)
        { }

        public void UpdateTypes(AssetsMetadata metadata)
        {
            base.UpdateType(metadata, AssetsConstants.ScriptHash.BeatmapLevelDataHash, AssetsConstants.ScriptPtr.BeatmapLevelDataScriptPtr);
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
        private string _levelID;

        [JsonProperty("_levelID")]
        public string LevelID
        {
            get
            {
                if (_levelID == null)
                {
                    _levelID = new string(SongName.Where(c => char.IsLetter(c)).ToArray());
                }
                //this probably isn't safe to use as a songID
                return _levelID;
            }
            set
            {
                _levelID = value;
            }
        }

        [JsonProperty("_songName")]
        public string SongName { get; set; }

        [JsonProperty("_songSubName")]
        public string SongSubName { get; set; }

        [JsonProperty("_songAuthorName")]
        public string SongAuthorName { get; set; }

        [JsonProperty("_levelAuthorName")]
        public string LevelAuthorName { get; set; }

        [JsonProperty("_beatsPerMinute")]
        public Single BeatsPerMinute { get; set; }

        [JsonProperty("_songTimeOffset")]
        public Single SongTimeOffset { get; set; }

        [JsonProperty("_shuffle")]
        public Single Shuffle { get; set; }

        [JsonProperty("_shufflePeriod")]
        public Single ShufflePeriod { get; set; }

        [JsonProperty("_previewStartTime")]
        public Single PreviewStartTime { get; set; }

        [JsonProperty("_previewDuration")]
        public Single PreviewDuration { get; set; }

        [JsonProperty("_difficultyBeatmapSets")]
        public List<DifficultyBeatmapSet> DifficultyBeatmapSets { get; private set; } = new List<DifficultyBeatmapSet>();

        //json CustomSong format properties
        [JsonProperty("_songFilename")]
        public string SongFilename { get; set; }

        [JsonProperty("_coverImageFilename")]
        public string CoverImageFilename { get; set; }

        [JsonProperty("_environmentName")]
        public string EnvironmentName { get; set; }


        //unity asset format properties
        [JsonIgnore]
        public PPtr CoverImageTexture2D { get; set; }

        [JsonIgnore]
        public PPtr EnvironmentSceneInfo { get; set; }

        [JsonIgnore]
        public PPtr AudioClip { get; set; }


        public override void Write(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.Write(LevelID);
            writer.Write(SongName);
            writer.Write(SongSubName);
            writer.Write(SongAuthorName);
            writer.Write(LevelAuthorName);
            AudioClip.Write(writer);
            writer.Write(BeatsPerMinute);
            writer.Write(SongTimeOffset);
            writer.Write(Shuffle);
            writer.Write(ShufflePeriod);
            writer.Write(PreviewStartTime);
            writer.Write(PreviewDuration);
            CoverImageTexture2D.Write(writer);
            EnvironmentSceneInfo.Write(writer);
            writer.Write(DifficultyBeatmapSets.Count);
            foreach (var bms in DifficultyBeatmapSets)
            {
                bms.Write(writer);
            }
        }

        protected override void Parse(AssetsReader reader)
        {
            base.Parse(reader);
            LevelID = reader.ReadString();
            SongName = reader.ReadString();
            SongSubName = reader.ReadString();
            SongAuthorName = reader.ReadString();
            LevelAuthorName = reader.ReadString();
            AudioClip = new PPtr(reader);
            BeatsPerMinute = reader.ReadSingle();
            SongTimeOffset = reader.ReadSingle();
            Shuffle = reader.ReadSingle();
            ShufflePeriod = reader.ReadSingle();
            PreviewStartTime = reader.ReadSingle();
            PreviewDuration = reader.ReadSingle();
            CoverImageTexture2D = new PPtr(reader);
            EnvironmentSceneInfo = new PPtr(reader);
            DifficultyBeatmapSets = reader.ReadArrayOf(x => new DifficultyBeatmapSet(x));
        }

        
        

    }



}