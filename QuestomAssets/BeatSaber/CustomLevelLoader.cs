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
        public static BeatmapLevelDataObject LoadSongFromPathToAsset(string songPath, AssetsFile assetsFile, bool includeCovers = true)
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

                bml.Name = $"{bml.LevelID}Level";
                if (assetsFile.Objects.Any(x=> x is BeatmapLevelDataObject && ((BeatmapLevelDataObject)x).LevelID == bml.LevelID))
                {
                    Console.WriteLine($"Level ID {bml.LevelID} is already in the assets, skipping it.");
                    return null;
                }

                //cover art
                Texture2DObject coverImage = null;
                if (includeCovers)
                {
                    coverImage = LoadSongCover(songPath, bml, assetsFile);
                }

                //audio
                var audioAsset = LoadSongAudioAsset(songPath, bml, assetsFile);
                if (audioAsset == null)
                {
                    Console.WriteLine("ERROR: failed to get audio for song at path {0}", songPath);
                    return null;
                }

                foreach (var d in bml.DifficultyBeatmapSets)
                {
                    switch (d.BeatmapCharacteristicName)
                    {
                        case Characteristic.OneSaber:
                            d.BeatmapCharacteristic = BeatSaberConstants.KnownObjects.OneSaberCharacteristic;
                            break;
                        case Characteristic.NoArrows:
                            d.BeatmapCharacteristic = BeatSaberConstants.KnownObjects.NoArrowsCharacteristic;
                            break;
                        case Characteristic.Standard:
                            d.BeatmapCharacteristic = BeatSaberConstants.KnownObjects.StandardCharacteristic;
                            break;
                    }
                    List<DifficultyBeatmap> toRemove = new List<DifficultyBeatmap>();
                    foreach (var dm in d.DifficultyBeatmaps)
                    {
                        var dataFile = Path.Combine(songPath, $"{dm.Difficulty.ToString()}.dat");
                        if (!File.Exists(dataFile))
                        {
                            Console.WriteLine(dataFile + " is missing, skipping this difficulty");
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

                        dm.BeatmapData.Name = bml.LevelID + ((d.BeatmapCharacteristicName == Characteristic.Standard) ? "" : d.BeatmapCharacteristicName.ToString()) + dm.Difficulty.ToString() + "BeatmapData";
                        dm.BeatmapData.BeatsPerMinute = bml.BeatsPerMinute;
                        dm.BeatmapData.Shuffle = bml.Shuffle;
                        dm.BeatmapData.ShufflePeriod = bml.ShufflePeriod;
                        dm.BeatmapData.JsonData = jsonData;
                        dm.BeatmapData.TransformToProjectedData();
                        dm.BeatmapData.JsonData = null;

                        assetsFile.AddObject(dm.BeatmapData, true);
                        dm.BeatmapDataPtr = dm.BeatmapData.ObjectInfo.LocalPtrTo;                        
                    }
                    toRemove.ForEach(x=>d.DifficultyBeatmaps.Remove(x));
                    if (d.DifficultyBeatmaps.Count < 1)
                    {
                        Console.WriteLine($"Song at path {songPath} has no valid beatmaps for any difficulty, skipping song");
                        return null;
                    }
                }


                assetsFile.AddObject(audioAsset, true);
                if (coverImage != null)
                {
                    assetsFile.AddObject(coverImage);
                }

                bml.AudioClip = audioAsset.ObjectInfo.LocalPtrTo;
                bml.CoverImageTexture2D = (coverImage?.ObjectInfo?.ObjectID == null) ? BeatSaberConstants.KnownObjects.BeatSaberCoverArt : coverImage.ObjectInfo.LocalPtrTo;

                switch (bml.EnvironmentName)
                {
                    case "DefaultEnvironment":
                        bml.EnvironmentSceneInfo = KnownObjects.File17.DefaultEnvironment;
                        break;
                    case "MonstercatEnvironment":
                        bml.EnvironmentSceneInfo = KnownObjects.File17.MonstercatEnvironment;
                        break;
                    case "TriangleEnvironment":
                        bml.EnvironmentSceneInfo = KnownObjects.File17.TriangleEnvironment;
                        break;
                    case "NiceEnvironment":
                        bml.EnvironmentSceneInfo = KnownObjects.File17.NiceEnvironment;
                        break;
                    case "BigMirrorEnvironment":
                        bml.EnvironmentSceneInfo = KnownObjects.File17.BigMirrorEnvironment;
                        break;
                    case "KDAEnvironment":
                        bml.EnvironmentSceneInfo = KnownObjects.File17.KDAEnvironment;
                        break;
                    case "CrabRaveEnvironment":
                        bml.EnvironmentSceneInfo = KnownObjects.File17.CrabRaveEnvironment;
                        break;
                    default:
                        Log.LogMsg($"Unknown environment name '{bml.EnvironmentName}' on '{bml.SongName}', falling back to default.");
                        bml.EnvironmentSceneInfo = KnownObjects.File17.DefaultEnvironment;
                        break;
                }               

                assetsFile.AddObject(bml, true);               

                return bml;
            } catch(Exception ex)
            {
                Console.WriteLine("Error loading song from path {0}: {1} {2}", songPath, ex.Message, ex.StackTrace);
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
                    Console.WriteLine("Exception parsing ogg file {2}: {0} {1}", ex.Message, ex.StackTrace, audioClipFile);
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
                    var imageBytes = ImageUtils.LoadToRGBAndMipmap(coverImage, 256, 256, 6, out mips);

                    var coverAsset = new Texture2DObject(assetsFile.Metadata)
                    {
                        Name = levelData.LevelID + "Cover",
                        ForcedFallbackFormat = 4,
                        DownscaleFallback = false,
                        Width = coverImage.Width,
                        Height = coverImage.Height,
                        CompleteImageSize = imageBytes.Length,
                        TextureFormat = 3,
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
                    Console.WriteLine("Error loading cover art asset: {0} {1}", ex.Message, ex.StackTrace);
                }
            }
            return null;
        }

        public static PPtr LoadPackCover(AssetsFile assetsFile)
        {

            //var extrasSprite = assetsFile.Objects.First(x => x.ObjectInfo.ObjectID == 45);
            //I don't want to write all the code for the Sprint class, so I'm encoding the binary and swapping specific bits around
            string spriteData = "CwAAAEV4dHJhc0NvdmVyAAAAAAAAAAAAAACARAAAgEQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIBEAAAAPwAAAD8BAAAAAAAAAKrborQQ7eZHhB371sswB+MgA0UBAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADgAAAAAAAAAAAAAAAAAAAAAAAAABAAAAAAAAAAYAAAAAAAAAAAAAAAAAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAQACAAIAAQADAAQAAAAOAAAAAAAAAwAAAAAAAAAAAAAAAAEAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAAAAAAAAvwAAAD8AAAAAAAAAPwAAAD8AAAAAAAAAvwAAAL8AAAAAAAAAPwAAAL8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIBEAACARAAAAAAAAAAAAACAvwAAgL8AAAAAAACARAAAAEQAAIBEAAAARAAAgD8AAAAAAAAAAA==";
            byte[] spriteBytes = Convert.FromBase64String(spriteData);

            var imageBytes = Resource1.CustomSongsCover;

            var packCover = new Texture2DObject(assetsFile.Metadata)
            {
                Name = "CustomSongsCover",
                ForcedFallbackFormat = 4,
                DownscaleFallback = false,
                Width = 1024,
                Height = 1024,
                CompleteImageSize = imageBytes.Length,
                TextureFormat = 34,
                MipCount = 11,
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
            byte[] finalBytes;
            using (MemoryStream ms = new MemoryStream(spriteBytes))
            {
                ms.Seek(116, SeekOrigin.Begin);
                using (AssetsWriter writer = new AssetsWriter(ms))
                    packCover.ObjectInfo.LocalPtrTo.Write(writer);

                finalBytes = ms.ToArray();
            }
            var nameBytes = System.Text.UTF8Encoding.UTF8.GetBytes("Custom");
            Array.Copy(nameBytes, 0, finalBytes, 4, nameBytes.Length);
            AssetsObject coverAsset = new AssetsObject(new ObjectInfo()
            {
                TypeIndex = assetsFile.Metadata.GetTypeIndexFromClassID(AssetsConstants.ClassID.SpriteClassID)
            })
            {
                Data = finalBytes
            };

            assetsFile.AddObject(coverAsset, true);
            return coverAsset.ObjectInfo.LocalPtrTo;
        }
    }
}
