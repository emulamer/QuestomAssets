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

namespace QuestomAssets.BeatSaber
{
    public class CustomLevelLoader
    {

        public static BeatmapLevelDataObject DeserializeFromJson(AssetsFile assetsFile, string songPath, string overrideLevelID)
        {
            try
            {
                var jsonSettings = new JsonSerializerSettings()
                {
                    ContractResolver = new MetadataResolver(assetsFile)
                };

                string infoFile = Path.Combine(songPath, "Info.dat");

                BeatmapLevelDataObject bml = null;
                using (var sr = new StreamReader(infoFile))
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


        public static BeatmapLevelDataObject LoadSongToAsset(BeatmapLevelDataObject beatmapLevel, string songPath, AssetsFile assetsFile, out string oggFileName, bool includeCovers = true)
        {
            oggFileName = null;
            try
            {
                beatmapLevel.Name = $"{beatmapLevel.LevelID}Level";

                //cover art
                Texture2DObject coverImage = null;
                if (includeCovers)
                {
                    coverImage = LoadSongCover(songPath, beatmapLevel, assetsFile);
                }

                //audio
                var audioAsset = LoadSongAudioAsset(songPath, beatmapLevel, assetsFile);
                if (audioAsset == null)
                {
                    Log.LogErr($"failed to get audio for song at path {songPath}");
                    return null;
                }
                oggFileName = Path.Combine(songPath, beatmapLevel.SongFilename); ;

                foreach (var d in beatmapLevel.DifficultyBeatmapSets)
                {
                    switch (d.BeatmapCharacteristicName)
                    {
                        case Characteristic.OneSaber:
                            d.BeatmapCharacteristic = KnownObjects.File17.OneSaberCharacteristic;
                            break;
                        case Characteristic.NoArrows:
                            d.BeatmapCharacteristic = KnownObjects.File17.NoArrowsCharacteristic;
                            break;
                        case Characteristic.Standard:
                            d.BeatmapCharacteristic = KnownObjects.File17.StandardCharacteristic;
                            break;
                    }
                    List<DifficultyBeatmap> toRemove = new List<DifficultyBeatmap>();
                    foreach (var dm in d.DifficultyBeatmaps)
                    {
                        var dataFile = Path.Combine(songPath, $"{dm.Difficulty.ToString()}.dat");
                        if (!File.Exists(dataFile))
                        {
                            Log.LogErr(dataFile + " is missing, skipping this difficulty");
                            toRemove.Add(dm);
                            continue;
                        }
                        string jsonData;
                        using (var sr = new StreamReader(dataFile))
                        {
                            jsonData = sr.ReadToEnd();
                        }
                        if (assetsFile != null)
                        {
                            dm.BeatmapData = new BeatmapDataObject(assetsFile.Metadata);
                        }
                        else
                        {
                            dm.BeatmapData = new BeatmapDataObject();
                        }

                        dm.BeatmapData.Name = beatmapLevel.LevelID + ((d.BeatmapCharacteristicName == Characteristic.Standard) ? "" : d.BeatmapCharacteristicName.ToString()) + dm.Difficulty.ToString() + "BeatmapData";
                        dm.BeatmapData.BeatsPerMinute = beatmapLevel.BeatsPerMinute;
                        dm.BeatmapData.Shuffle = beatmapLevel.Shuffle;
                        dm.BeatmapData.ShufflePeriod = beatmapLevel.ShufflePeriod;
                        dm.BeatmapData.JsonData = jsonData;
                        dm.BeatmapData.TransformToProjectedData();
                        dm.BeatmapData.JsonData = null;

                        assetsFile.AddObject(dm.BeatmapData, true);
                        dm.BeatmapDataPtr = dm.BeatmapData.ObjectInfo.LocalPtrTo;                        
                    }
                    toRemove.ForEach(x=>d.DifficultyBeatmaps.Remove(x));
                    if (d.DifficultyBeatmaps.Count < 1)
                    {
                        Log.LogErr($"Song at path {songPath} has no valid beatmaps for any difficulty, skipping song");
                        return null;
                    }
                }


                assetsFile.AddObject(audioAsset, true);
                if (coverImage != null)
                {
                    assetsFile.AddObject(coverImage);
                }

                beatmapLevel.AudioClip = audioAsset.ObjectInfo.LocalPtrTo;
                beatmapLevel.CoverImageTexture2D = (coverImage?.ObjectInfo?.ObjectID == null) ? KnownObjects.File17.BeatSaberCoverArt : coverImage.ObjectInfo.LocalPtrTo;

                switch (beatmapLevel.EnvironmentName)
                {
                    case "DefaultEnvironment":
                        beatmapLevel.EnvironmentSceneInfo = KnownObjects.File17.DefaultEnvironment;
                        break;
                    case "MonstercatEnvironment":
                        beatmapLevel.EnvironmentSceneInfo = KnownObjects.File17.MonstercatEnvironment;
                        break;
                    case "TriangleEnvironment":
                        beatmapLevel.EnvironmentSceneInfo = KnownObjects.File17.TriangleEnvironment;
                        break;
                    case "NiceEnvironment":
                        beatmapLevel.EnvironmentSceneInfo = KnownObjects.File17.NiceEnvironment;
                        break;
                    case "BigMirrorEnvironment":
                        beatmapLevel.EnvironmentSceneInfo = KnownObjects.File17.BigMirrorEnvironment;
                        break;
                    case "KDAEnvironment":
                        beatmapLevel.EnvironmentSceneInfo = KnownObjects.File17.KDAEnvironment;
                        break;
                    case "CrabRaveEnvironment":
                        beatmapLevel.EnvironmentSceneInfo = KnownObjects.File17.CrabRaveEnvironment;
                        break;
                    default:
                        Log.LogMsg($"Unknown environment name '{beatmapLevel.EnvironmentName}' on '{beatmapLevel.SongName}', falling back to default.");
                        beatmapLevel.EnvironmentSceneInfo = KnownObjects.File17.DefaultEnvironment;
                        break;
                }               

                assetsFile.AddObject(beatmapLevel, true);               

                return beatmapLevel;
            } catch(Exception ex)
            {
                Log.LogErr($"Error loading song from path {songPath}", ex);
                return null;
            }
        }

        public static AudioClipObject LoadSongAudioAsset(string songPath, BeatmapLevelDataObject levelData, AssetsFile assetsFile)
        {
            string audioClipFile = Path.Combine(songPath, levelData.SongFilename);
            string outputFileName = levelData.LevelID + ".ogg";
            int channels;
            int frequency;
            Single length;
            byte[] oggBytes = File.ReadAllBytes(audioClipFile);
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
            var audioClip = new AudioClipObject(assetsFile.Metadata)
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
                Resource = new StreamedResource(outputFileName, 0, Convert.ToUInt64(new FileInfo(audioClipFile).Length))
            };
            return audioClip;
        }

        public static Texture2DObject LoadSongCover(string songPath, BeatmapLevelDataObject levelData, AssetsFile assetsFile)
        {
            if (!string.IsNullOrWhiteSpace(levelData.CoverImageFilename) && File.Exists(Path.Combine(songPath, levelData.CoverImageFilename)))
            {
                try
                {
                    string coverFile = Path.Combine(songPath, levelData.CoverImageFilename);
                    Bitmap coverImage = (Bitmap)Bitmap.FromFile(coverFile);
                    int mips;
                    var imageBytes = ImageUtils.ConvertToRGBAndMipmap(coverImage, 256, 256, 6, out mips);

                    var coverAsset = new Texture2DObject(assetsFile.Metadata)
                    {
                        Name = levelData.LevelID + "Cover",
                        ForcedFallbackFormat = 4,
                        DownscaleFallback = false,
                        Width = 256,
                        Height = 256,
                        CompleteImageSize = imageBytes.Length,
                        TextureFormat = Texture2DObject.TextureFormatType.RGB24,
                        MipCount = mips,
                        IsReadable = false,
                        StreamingMipmaps = false,
                        StreamingMipmapsPriority = 0,
                        ImageCount = 1,
                        TextureDimension = 2,
                        TextureSettings = new GLTextureSettings()
                        {
                            FilterMode = 2,
                            Aniso = 1,
                            MipBias = -1,
                            WrapU = 1,
                            WrapV = 1,
                            WrapW = 0
                        },
                        LightmapFormat = 6,
                        ColorSpace = 1,
                        ImageData = imageBytes,
                        StreamData = new StreamingInfo()
                        {
                            offset = 0,
                            size = 0,
                            path = ""
                        }
                    };
                    return coverAsset;
                }
                catch (Exception ex)
                {
                    Log.LogErr("Error loading cover art asset", ex);
                }
            }
            return null;
        }

        public static SpriteObject LoadPackCover(string assetName, AssetsFile assetsFile, string fromFilename)
        {
            byte[] imageBytes = null;
            int mips = 11;
            if (!string.IsNullOrWhiteSpace(fromFilename))
            {
                try
                {
                    imageBytes = Utils.ImageUtils.ConvertToETC1AndMipmap(new Bitmap(fromFilename), 1024, 1024, 11, out mips);
                }
                catch (Exception ex)
                {
                    Log.LogErr($"Failed to convert {fromFilename} to ETC2 texture, falling back to default cover image", ex);

                }
            }
            if (imageBytes == null)
                imageBytes = Resources.Resource1.CustomSongsCover;

            var packCover = new Texture2DObject(assetsFile.Metadata)
            {
                Name = assetName,
                ForcedFallbackFormat = 4,
                DownscaleFallback = false,
                Width = 1024,
                Height = 1024,
                CompleteImageSize = imageBytes.Length,
                TextureFormat = AssetsChanger.Texture2DObject.TextureFormatType.ETC2_RGB,
                MipCount = mips,
                IsReadable = false,
                StreamingMipmaps = false,
                StreamingMipmapsPriority = 0,
                ImageCount = 1,
                TextureDimension = 2,
                TextureSettings = new GLTextureSettings()
                {
                    FilterMode = 2,
                    Aniso = 1,
                    MipBias = -1,
                    WrapU = 1,
                    WrapV = 1,
                    WrapW = 0
                },
                LightmapFormat = 6,
                ColorSpace = 1,
                ImageData = imageBytes,
                StreamData = new StreamingInfo()
                {
                    offset = 0,
                    size = 0,
                    path = ""
                }
            };
            assetsFile.AddObject(packCover, true);
            
            //slightly less hacky than before, but only a little
            var extrasCover = assetsFile.FindAsset<SpriteObject>("ExtrasCover");
            SpriteObject coverAsset = assetsFile.CopyAsset(extrasCover);
            coverAsset.Name = assetName;
            coverAsset.Texture = packCover.ObjectInfo.LocalPtrTo;
            assetsFile.AddObject(coverAsset, true);
            return coverAsset;
        }
    }
}
