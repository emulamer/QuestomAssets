using QuestomAssets.AssetsChanger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
using QuestomAssets.Utils;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace QuestomAssets.BeatSaber
{
    public class CustomLevelLoader
    {
        public CustomLevelLoader(AssetsFile assetsFile, QaeConfig config)
        {
            _assetsFile = assetsFile;
            _config = config;
        }

        private QaeConfig _config;
        private AssetsFile _assetsFile;

        private Dictionary<string, MonoBehaviourObject> _characteristicCache = new Dictionary<string, MonoBehaviourObject>();

        public BeatmapLevelDataObject DeserializeFromJson(string songPath, string overrideLevelID)
        {
            try
            {
                var jsonSettings = new JsonSerializerSettings()
                {
                    ContractResolver = new MetadataResolver(_assetsFile)
                };

                string infoFile = Path.Combine(songPath, "Info.dat");

                BeatmapLevelDataObject bml = null;
                
                using (var sr = new StreamReader(_config.SongFileProvider.GetReadStream(infoFile)))
                {
                    bml = JsonConvert.DeserializeObject<BeatmapLevelDataObject>(sr.ReadToEnd(), jsonSettings);
                }

                if (!string.IsNullOrWhiteSpace(overrideLevelID))
                {
                    bml.LevelID = overrideLevelID;
                }
                return bml;
            }
            catch (Exception ex)
            {
                Log.LogErr("Exception deserializing level", ex);
                return null;
            }
        }

        public BeatmapLevelDataObject LoadSongToAsset(BeatmapLevelDataObject beatmapLevel, string songPath, bool includeCovers = true)
        {
            try
            {
                beatmapLevel.Name = $"{beatmapLevel.LevelID}Level";

                //cover art
                Texture2DObject coverImage = null;
                if (includeCovers)
                {
                    coverImage = LoadSongCover(songPath, beatmapLevel);
                }

                //audio
                var audioAsset = LoadSongAudioAsset(songPath, beatmapLevel);
                if (audioAsset == null)
                {
                    Log.LogErr($"failed to get audio for song at path {songPath}");
                    return null;
                }
                
                foreach (var difficultySet in beatmapLevel.DifficultyBeatmapSets)
                {
                    difficultySet.BeatmapCharacteristic = GetCharacteristicAsset(difficultySet.BeatmapCharacteristicName).PtrFrom(beatmapLevel);
                    List<DifficultyBeatmap> toRemove = new List<DifficultyBeatmap>();
                    foreach (var difficultyBeatmap in difficultySet.DifficultyBeatmaps)
                    {
                        var dataFile = songPath.CombineFwdSlash($"{difficultyBeatmap.Difficulty.ToString()}.dat");
                        if (!_config.SongFileProvider.FileExists(dataFile))
                        {
                            Log.LogErr(dataFile + " is missing, skipping this difficulty");
                            toRemove.Add(difficultyBeatmap);
                            continue;
                        }
                        string jsonData = _config.SongFileProvider.ReadToString(dataFile);
                        if (_assetsFile != null)
                        {
                            difficultyBeatmap.BeatmapData = new BeatmapDataObject(_assetsFile);
                        }

                        difficultyBeatmap.BeatmapData.Name = beatmapLevel.LevelID + ((difficultySet.BeatmapCharacteristicName == Characteristic.Standard) ? "" : difficultySet.BeatmapCharacteristicName.ToString()) + difficultyBeatmap.Difficulty.ToString() + "BeatmapData";
                        difficultyBeatmap.BeatmapData.BeatsPerMinute = beatmapLevel.BeatsPerMinute;
                        difficultyBeatmap.BeatmapData.Shuffle = beatmapLevel.Shuffle;
                        difficultyBeatmap.BeatmapData.ShufflePeriod = beatmapLevel.ShufflePeriod;
                        difficultyBeatmap.BeatmapData.JsonData = jsonData;

                        _assetsFile.AddObject(difficultyBeatmap.BeatmapData, true);
                        difficultyBeatmap.BeatmapDataPtr = difficultyBeatmap.BeatmapData.PtrFrom(beatmapLevel);
                    }
                    toRemove.ForEach(x => difficultySet.DifficultyBeatmaps.Remove(x));
                    if (difficultySet.DifficultyBeatmaps.Count < 1)
                    {
                        Log.LogErr($"Song at path {songPath} has no valid beatmaps for any difficulty, skipping song");
                        return null;
                    }
                }

                _assetsFile.AddObject(audioAsset, true);
                if (coverImage != null)
                {
                    _assetsFile.AddObject(coverImage);
                }

                beatmapLevel.AudioClip = audioAsset.PtrFrom(beatmapLevel);
                if (coverImage == null)
                {
                    beatmapLevel.CoverImageTexture2D = _assetsFile.FindAsset<Texture2DObject>(x => x.Object.Name == "BeatSaberCover").PtrFrom(beatmapLevel);
                }
                else
                {
                    beatmapLevel.CoverImageTexture2D = coverImage.PtrFrom(beatmapLevel);
                }
                var environment = _assetsFile.Manager.MassFirstOrDefaultAsset<MonoBehaviourObject>(x => x.Object.Name == $"{beatmapLevel.EnvironmentName}SceneInfo");
                if (environment == null)
                {
                    Log.LogMsg($"Unknown environment name '{beatmapLevel.EnvironmentName}' on '{beatmapLevel.SongName}', falling back to default.");
                    environment = _assetsFile.Manager.MassFirstAsset<MonoBehaviourObject>(x => x.Object.Name == "DefaultEnvironmentSceneInfo");
                }

                beatmapLevel.EnvironmentSceneInfo = environment.PtrFrom(beatmapLevel);
                _assetsFile.AddObject(beatmapLevel, true);
                return beatmapLevel;
            }
            catch (Exception ex)
            {
                Log.LogErr($"Error loading song from path {songPath}", ex);
                return null;
            }
        }

        public AudioClipObject LoadSongAudioAsset(string songPath, BeatmapLevelDataObject levelData)
        {
            string audioClipFile = songPath.CombineFwdSlash(levelData.SongFilename);
            //string outputFileName = levelData.LevelID + ".ogg";
            int channels;
            int frequency;
            Single length;
            byte[] oggBytes = _config.SongFileProvider.Read(audioClipFile);
            unsafe
            {

                GCHandle pinnedArray = GCHandle.Alloc(oggBytes, GCHandleType.Pinned);
                try
                {
                    IntPtr pointer = pinnedArray.AddrOfPinnedObject();
                    int error;
                    StbSharp.StbVorbis.stb_vorbis_alloc alloc;
                    StbSharp.StbVorbis.stb_vorbis v = StbSharp.StbVorbis.stb_vorbis_open_memory((byte*)pointer.ToPointer(), oggBytes.Length, &error, &alloc);
                    channels = v.channels;
                    frequency = (int)v.sample_rate;
                    length = StbSharp.StbVorbis.stb_vorbis_stream_length_in_seconds(v);
                    StbSharp.StbVorbis.stb_vorbis_close(v);
                }
                catch (Exception ex)
                {
                    Log.LogErr($"Exception parsing ogg file {audioClipFile}", ex);
                    return null;
                }
                finally
                {
                    pinnedArray.Free();
                }
            }
            var audioClip = new AudioClipObject(_assetsFile)
            {
                Name = levelData.LevelID,
                LoadType = 1,
                IsTrackerFormat = false,
                Ambisonic = false,
                SubsoundIndex = 0,
                PreloadAudioData = false,
                LoadInBackground = true,
                Legacy3D = true,
                CompressionFormat = 1,
                BitsPerSample = 16,
                Channels = channels,
                Frequency = frequency,
                Length = (Single)length,
                Resource = new StreamedResource(audioClipFile, 0, Convert.ToUInt64(_config.SongFileProvider.GetFileSize(audioClipFile)))
            };
            return audioClip;
        }

        public Texture2DObject LoadSongCover(string songPath, BeatmapLevelDataObject levelData)
        {
            if (!string.IsNullOrWhiteSpace(levelData.CoverImageFilename) && _config.SongFileProvider.FileExists(_config.SongsPath.CombineFwdSlash(songPath).CombineFwdSlash(levelData.CoverImageFilename)))
            {
                try
                {
                    string coverFile = songPath.CombineFwdSlash(levelData.CoverImageFilename);

                    var coverAsset = new Texture2DObject(_assetsFile)
                    {
                        Name = levelData.LevelID + "Cover"
                    };
                    byte[] imageBytes = _config.SongFileProvider.Read(coverFile);
                    ImageUtils.Instance.AssignImageToTexture(imageBytes, coverAsset, 256, 256);
                    return coverAsset;
                }
                catch (Exception ex)
                {
                    Log.LogErr("Error loading cover art asset", ex);
                }
            }
            return null;
        }

        public SpriteObject LoadPackCover(string assetName, string packCoverImage)
        {
            Texture2DObject packCover = null;
            if (!string.IsNullOrWhiteSpace(packCoverImage))
            {
                try
                {
                    var loadedCover = new Texture2DObject(_assetsFile)
                    {
                        Name = assetName
                    };
                    byte[] coverImageBytes = _config.SongFileProvider.Read(_config.PlaylistArtPath.CombineFwdSlash(packCoverImage));
                    ImageUtils.Instance.AssignImageToTexture(coverImageBytes, loadedCover, 1024, 1024);
                    packCover = loadedCover;
                }
                catch (Exception ex)
                {
                    Log.LogErr($"Failed to convert to texture, falling back to default cover image", ex);
                }
            }
            if (packCover == null)
            {
                packCover = new Texture2DObject(_assetsFile)
                {
                    Name = assetName
                };
                SetFallbackCoverTexture(packCover);
            }
            _assetsFile.AddObject(packCover, true);

            //slightly less hacky than before, but only a little
            var extrasCover = _assetsFile.Manager.MassFirstAsset<SpriteObject>(x => x.Object.Name == "ExtrasCover");
            SpriteObject coverAsset = extrasCover.Clone();
            coverAsset.Name = assetName;
            coverAsset.Texture = packCover.PtrFrom(coverAsset);
            _assetsFile.AddObject(coverAsset, true);
            return coverAsset;
        }

        //todo: not sure if this is always safe to assume the folder where the ogg is has the info.dat, but I hope it is
        public string GetCoverImageFilename(BeatmapLevelDataObject levelData)
        {
            var path = levelData?.AudioClip?.Object?.Resource?.Source;
            if (path == null)
            {
                Log.LogErr("Trying to get the cover image file, audio clip resource object was null!");
                return null;
            }
            if (!path.StartsWith(_config.SongsPath))
            {
                Log.LogErr($"The audio clip path for level ID {levelData.LevelID} of '{path}' doesn't match with the configured songs path of '{_config.SongsPath}'.  Can't get cover art image location!");
                return null;
            }

            //path = path.Substring(_config.SongsPath.Length).TrimStart('/');
            int idx = path.LastIndexOf("/");
            path = path.Substring(0, idx);

            var infodatfile = path.CombineFwdSlash("info.dat");
            if (!_config.SongFileProvider.FileExists(infodatfile))
            {
                Log.LogErr($"Could not find {infodatfile} for level ID {levelData.LevelID}");
                return null;
            }

            //I hope this is a faster way than deserializing the entire object and worth the double code paths for parsing the same serialized type
            string imageFilename = null;
            try
            {
                using (var jr = new JsonTextReader(new StreamReader(_config.SongFileProvider.GetReadStream(infodatfile))))
                {
                    while (jr.Read())
                    {
                        if (jr.TokenType == JsonToken.PropertyName && jr.Value?.ToString() == "_coverImageFilename")
                        {
                            if (jr.Read() && jr.TokenType == JsonToken.String)
                            {
                                imageFilename = jr.Value?.ToString();
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogErr($"Exception trying to read info.dat to get _coverImageFilename for level ID {levelData.LevelID} from '{infodatfile}'.", ex);
                return null;
            }
            return imageFilename;
        }

        private static void SetFallbackCoverTexture(Texture2DObject texture)
        {
            byte[] imageBytes;
            var assembly = Assembly.GetExecutingAssembly();
            var resourceStream = assembly.GetManifestResourceStream("QuestomAssets.CustomSongsCover.ETC_RGB4");
            using (var reader = new BinaryReader(resourceStream, Encoding.UTF8))
            {
                imageBytes = reader.ReadBytes((int)resourceStream.Length);
            }
              //Resource File.ReadAllBytes(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "CustomSongsCover.ETC_RGB4"));
            int mips = 11;

            texture.ForcedFallbackFormat = 4;
            texture.DownscaleFallback = false;
            texture.Width = 1024;
            texture.Height = 1024;
            texture.CompleteImageSize = imageBytes.Length;
            texture.TextureFormat = AssetsChanger.Texture2DObject.TextureFormatType.ETC2_RGB;
            texture.MipCount = mips;
            texture.IsReadable = false;
            texture.StreamingMipmaps = false;
            texture.StreamingMipmapsPriority = 0;
            texture.ImageCount = 1;
            texture.TextureDimension = 2;
            texture.TextureSettings = new GLTextureSettings()
            {
                FilterMode = 2,
                Aniso = 1,
                MipBias = -1,
                WrapU = 1,
                WrapV = 1,
                WrapW = 0
            };
            texture.LightmapFormat = 6;
            texture.ColorSpace = 1;
            texture.ImageData = imageBytes;
            texture.StreamData = new StreamingInfo()
            {
                offset = 0,
                size = 0,
                path = ""
            };
        }
        
        private MonoBehaviourObject GetCharacteristicAsset(Characteristic characteristic)
        {
            string name = "StandardBeatmapCharacteristic";
            switch (characteristic)
            {
                case Characteristic.OneSaber:
                    name = "OneColorBeatmapCharacteristic";
                    break;
                case Characteristic.NoArrows:
                    name = "NoArrowsBeatmapCharacteristic";
                    break;
                case Characteristic.Standard:
                    name = "StandardBeatmapCharacteristic";
                    break;
            }
            MonoBehaviourObject charObj = null;
            if (!_characteristicCache.ContainsKey(name))
            {
                charObj = _assetsFile.Manager.MassFirstAsset<MonoBehaviourObject>(x => x.Object.Name == name).Object;
                _characteristicCache.Add(name, charObj);
            }
            else
            {
                charObj = _characteristicCache[name];
            }
            return charObj;
        }        
    }
}
