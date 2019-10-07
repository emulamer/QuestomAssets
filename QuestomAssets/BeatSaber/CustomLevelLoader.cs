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

        //TODO: move the cache somewhere else or create an instance of this class and pass it around

        public CustomLevelLoader(AssetsFile assetsFile, QaeConfig config)
        {
            _assetsFile = assetsFile;
            _config = config;
        }

        private QaeConfig _config;
        private AssetsFile _assetsFile;

        private Dictionary<string, BeatmapCharacteristicObject> _characteristicCache = new Dictionary<string, BeatmapCharacteristicObject>();

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

                var toRemoveSet = new List<DifficultyBeatmapSet>();
                foreach (var difficultySet in beatmapLevel.DifficultyBeatmapSets)
                {
                    var characteristic = GetCharacteristicAsset(difficultySet.BeatmapCharacteristicName)?.PtrFrom(beatmapLevel);
                    if (characteristic == null)
                    {
                        Log.LogErr($"Characteristic {difficultySet.BeatmapCharacteristicName} couldn't be found.  Set will be removed.");
                        toRemoveSet.Add(difficultySet);
                        continue;
                    }
                    difficultySet.BeatmapCharacteristic = characteristic;
                    List<DifficultyBeatmap> toRemove = new List<DifficultyBeatmap>();
                    foreach (var difficultyBeatmap in difficultySet.DifficultyBeatmaps)
                    {
                        string dataFile = null;
                        if (!string.IsNullOrWhiteSpace(difficultyBeatmap.BeatmapFilename))
                        {
                            dataFile = songPath.CombineFwdSlash(difficultyBeatmap.BeatmapFilename);
                            if (!_config.SongFileProvider.FileExists(dataFile))
                            {
                                Log.LogErr($"BeatmapFilename was set to {dataFile} but the file didn't exist, will try to fall back to difficulty name.");
                                dataFile = null;
                            }
                        }
                        if (dataFile == null)
                            dataFile = songPath.CombineFwdSlash($"{difficultyBeatmap.Difficulty.ToString()}.dat");

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
                        Log.LogErr($"Song at path {songPath} has no valid beatmaps for any difficulty on set {difficultySet.BeatmapCharacteristicName}, removing it");
                        toRemoveSet.Add(difficultySet);
                        continue;
                    }
                }
                toRemoveSet.ForEach(x => beatmapLevel.DifficultyBeatmapSets.Remove(x));
                if (beatmapLevel.DifficultyBeatmapSets.Count < 1)
                {
                    Log.LogErr($"Song at path {songPath} has no valid characterstics, it will not be imported");
                    return null;
                }
                _assetsFile.AddObject(audioAsset, true);
                if (coverImage != null)
                {
                    _assetsFile.AddObject(coverImage);
                }

                beatmapLevel.AudioClip = audioAsset.PtrFrom(beatmapLevel);
                if (coverImage == null)
                {
                    var bsCover = _assetsFile.FindAsset<Texture2DObject>(x => x.Object.Name == "BeatSaberCover");
                    if (bsCover == null)
                    {
                        Log.LogErr("Unable to find BeatSaberCover in assets!  How is that gone?");
                        throw new Exception("Could not find beat saber cover in assets!  That should never be missing.");
                    }
                    var cover = bsCover.Clone();
                    
                    _assetsFile.AddObject(cover, true);
                    beatmapLevel.CoverImageTexture2D = cover.PtrFrom(beatmapLevel);
                }
                else
                {
                    beatmapLevel.CoverImageTexture2D = coverImage.PtrFrom(beatmapLevel);
                }
                var environment = GetEnvironment(beatmapLevel.EnvironmentName);
                if (environment == null)
                {
                    Log.LogMsg($"Unknown environment name '{beatmapLevel.EnvironmentName}' on '{beatmapLevel.SongName}', falling back to default.");
                    environment = GetEnvironment("DefaultEnvironment");
                    if (environment == null)
                        throw new Exception("Unable to find the default environment!");
                }

                beatmapLevel.EnvironmentInfo = environment.PtrFrom(beatmapLevel);
                _assetsFile.AddObject(beatmapLevel, true);
                return beatmapLevel;
            }
            catch (Exception ex)
            {
                Log.LogErr($"Error loading song from path {songPath}", ex);
                return null;
            }
        }
        private Dictionary<string, EnvironmentInfoObject> _environmentCache = new Dictionary<string, EnvironmentInfoObject>();
        private EnvironmentInfoObject GetEnvironment(string name)
        {
            if (_environmentCache.ContainsKey(name))
                return _environmentCache[name];

            var environment = _assetsFile.Manager.MassFirstOrDefaultAsset<EnvironmentInfoObject>(x => x.Object.SerializedName == $"{name}Environment" || x.Object.SerializedName == $"{name}")?.Object;
            if (environment != null)
                _environmentCache.Add(name, environment);

            return environment;
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
            if (!string.IsNullOrWhiteSpace(levelData.CoverImageFilename) && _config.SongFileProvider.FileExists(songPath.CombineFwdSlash(levelData.CoverImageFilename)))
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

        public SpriteObject LoadPackCover(string assetName, byte[] coverImageBytes)
        {
            Texture2DObject packCover = null;

            if (coverImageBytes != null && coverImageBytes.Length > 0)
            {
                try
                {
                    var loadedCover = new Texture2DObject(_assetsFile)
                    {
                        Name = assetName
                    };
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
                Log.LogMsg($"Using default cover image for asset name {assetName}");
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
            coverAsset.RenderData.Texture = packCover.PtrFrom(coverAsset);
            _assetsFile.AddObject(coverAsset, true);
            return coverAsset;
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
        
        private BeatmapCharacteristicObject GetCharacteristicAsset(Characteristic characteristic)
        {
            //TODO: fix the lightshow and stuff
            if (characteristic == Characteristic.Lightshow || characteristic == Characteristic.Lawless)
                return null;
            string name = MiscUtils.GetCharacteristicAssetName(characteristic);
            if (name == null)
                name = "StandardBeatmapCharacteristic";
            BeatmapCharacteristicObject charObj = null;
            if (!_characteristicCache.ContainsKey(name))
            {
                charObj = _assetsFile.Manager.MassFirstOrDefaultAsset<BeatmapCharacteristicObject>(x => x.Object.Name == name)?.Object;
                if (charObj == null)
                    charObj = CreateCharacteristic(characteristic);
                if (charObj == null)
                    return null;
                _characteristicCache.Add(name, charObj);
            }
            else
            {
                charObj = _characteristicCache[name];
            }
            return charObj;
        }

        private BeatmapCharacteristicObject CreateCharacteristic(Characteristic characteristic)
        {
            if (characteristic == Characteristic.Standard)
                throw new Exception("Tried to create standard beatmap characteristic which means it's missing.  Assets are broken.");

            BeatmapCharacteristicObject standardCharacteristic = GetCharacteristicAsset(Characteristic.Standard);
            if (standardCharacteristic == null)
            {
                Log.LogErr($"Unable to locate the standard beatmap characteristic while verifying characteristics!");
                throw new Exception("Could not locate standard beatmap characteristic!");
            }
            int count = _assetsFile.Manager.MassFindAssets<BeatmapCharacteristicObject>(x => true, false).Count();
            try
            {
                string characteristicName = $"LEVEL_{characteristic.ToString().ToUpper()}";
                string hintText = $"{characteristicName}_HINT";
                string assetName = MiscUtils.GetCharacteristicAssetName(characteristic);
                var lightshowAsset = (BeatmapCharacteristicObject)standardCharacteristic.ObjectInfo.Clone(standardCharacteristic.ObjectInfo.ParentFile);
                
                lightshowAsset.Name = assetName;
                lightshowAsset.SerializedName = characteristic.ToString();
                lightshowAsset.SortingOrder = count;
                lightshowAsset.CompoundIdPartName = characteristic.ToString();
                //todo: text translation stuff
                //lightshowAsset.CharacteristicName = characteristicName;
                //lightshowAsset.HintText = hintText;
                var allChar = _assetsFile.Manager.MassFirstOrDefaultAsset<BeatmapCharacteristicCollectionObject>(x => true);
                if (allChar == null)
                    throw new Exception("Unable to find AllBeatmapCharacteristics object!");
                if (!allChar.Object.BeatmapCharacteristics.Any(x=> x.Object.Name == lightshowAsset.Name))
                {
                    allChar.Object.BeatmapCharacteristics.Add(lightshowAsset.PtrFrom(allChar.Object));
                }
                try
                {
                    byte[] lightshowIcon = _config.EmbeddedResourcesFileProvider.Read($"{characteristic}.png");
                    if (lightshowIcon == null || lightshowIcon.Length < 1)
                        throw new Exception($"{characteristic}.png read was null or empty!");
                    var clonedSprite = (SpriteObject)standardCharacteristic.Icon.Object.ObjectInfo.Clone(standardCharacteristic.ObjectInfo.ParentFile);
                    var newTexture =new Texture2DObject(standardCharacteristic.ObjectInfo.ParentFile)
                    {
                        Name = assetName
                    };
                    clonedSprite.RenderData.AtlasRectOffset.X = -1;
                    clonedSprite.RenderData.AtlasRectOffset.Y = -1;
                    clonedSprite.RenderData.TextureRect.X = 0;
                    clonedSprite.RenderData.TextureRect.Y = 0;
                    clonedSprite.RenderData.Texture = newTexture.PtrFrom(clonedSprite);
                    clonedSprite.Name = assetName + "Icon";
                    ImageUtils.Instance.AssignImageToTexture(lightshowIcon, newTexture, 128, 128, Int32.MaxValue, TextureConversionFormat.Auto);
                    lightshowAsset.Icon = clonedSprite.PtrFrom(lightshowAsset);
                    standardCharacteristic.ObjectInfo.ParentFile.AddObject(clonedSprite);
                    standardCharacteristic.ObjectInfo.ParentFile.AddObject(newTexture);
                    standardCharacteristic.ObjectInfo.ParentFile.AddObject(lightshowAsset);
                }
                catch (Exception ex)
                {
                    Log.LogErr($"Failed to load {characteristic}'s png icon!", ex);
                    throw;
                }
                return lightshowAsset;
            }
            catch (Exception ex)
            {
                Log.LogErr($"Exception trying to create {characteristic} characteristic!", ex);
                throw new Exception($"Error trying to create characteristic {characteristic}!", ex);
            }
       }
    }
}
