using System;
using System.IO;
using QuestomAssets.AssetsChanger;
using QuestomAssets.BeatSaber;
using System.Collections.Generic;
using System.Linq;
using QuestomAssets.Utils;
using Newtonsoft.Json;

namespace QuestomAssets
{

    public class QuestomAssetsEngine
    {
        private string _apkFilename;
        private bool _readOnly;
        private string _pemData;

        //TODO: fix cross-asset file loading of stuff before turning this to false, some of the OST Vol 1 songs are in another file
        public bool HideOriginalPlaylists { get; private set; } = true;

        /// <summary>
        /// Create a new instance of the class and open the apk file
        /// </summary>
        /// <param name="apkFilename">The path to the Beat Saber APK file</param>
        /// <param name="readOnly">True to open the APK read only</param>
        /// <param name="pemCertificateData">The contents of the PEM certificate that will be used to sign the APK.  If omitted, a new self signed cert will be generated.</param>
        public QuestomAssetsEngine(string apkFilename, bool readOnly = false, string pemCertificateData = BSConst.DebugCertificatePEM)
        {
            _readOnly = readOnly;
            _apkFilename = apkFilename;
            _pemData = pemCertificateData;
        }

        private MainLevelPackCollectionObject GetMainLevelPack(AssetsManager manager)
        {
            var mainLevelPack = manager.MassFirstOrDefaultAsset<MainLevelPackCollectionObject>(x => true)?.Object;
            if (mainLevelPack == null)
                throw new Exception("Unable to find the main level pack collection object!");
            return mainLevelPack;
        }

        private void UpdatePlaylistConfig(AssetsManager manager, BeatSaberPlaylist playlist)
        {
            Log.LogMsg($"Processing playlist ID {playlist.PlaylistID}...");
            var songsAssetFile = manager.GetAssetsFile(BSConst.KnownFiles.SongsAssetsFilename);
            CustomLevelLoader loader = new CustomLevelLoader(songsAssetFile);
            BeatmapLevelPackObject levelPack = songsAssetFile.FindAsset<BeatmapLevelPackObject>(x => x.Object.PackID == playlist.PlaylistID)?.Object;
            //create a new level pack if one waasn't found
            if (levelPack == null)
            {
                Log.LogMsg($"Level pack for playlist '{playlist.PlaylistID}' was not found and will be created");
                levelPack = new BeatmapLevelPackObject(songsAssetFile)
                {
                    Enabled = 1,
                    GameObjectPtr = null,
                    IsPackAlwaysOwned = true,
                    PackID = playlist.PlaylistID,
                    Name = playlist.PlaylistID + BSConst.NameSuffixes.LevelPack,
                    PackName = playlist.PlaylistName
                };
                songsAssetFile.AddObject(levelPack, true);
                var col = new BeatmapLevelCollectionObject(songsAssetFile)
                { Name = playlist.PlaylistID + BSConst.NameSuffixes.LevelCollection };
                songsAssetFile.AddObject(col, true);
                levelPack.BeatmapLevelCollection = col.PtrFrom(levelPack);
            }

            playlist.LevelPackObject = levelPack;
            

            levelPack.PackName = playlist.PlaylistName??levelPack.PackName;
            if (playlist.CoverArt != null)
            {
                Log.LogMsg($"Loading cover art for playlist ID '{playlist.PlaylistID}'");

                playlist.CoverArtSprite = loader.LoadPackCover(playlist.PlaylistID, playlist.CoverArt);
                playlist.LevelPackObject.CoverImage = playlist.CoverArtSprite.PtrFrom(playlist.LevelPackObject);
            }
            else
            {
                if (playlist.LevelPackObject.CoverImage != null)
                {
                    playlist.CoverArtSprite = playlist.LevelPackObject.CoverImage.Object;
                }
                else
                {
                    playlist.CoverArtSprite = loader.LoadPackCover(playlist.PlaylistID, null);
                }
                playlist.LevelPackObject.CoverImage = playlist.CoverArtSprite.PtrFrom(playlist.LevelPackObject);
            }

            //clear out any levels, we'll add them back
            var levelCollection = levelPack.BeatmapLevelCollection.Object;
            levelCollection.BeatmapLevels.ForEach(x => x.Dispose());
            levelCollection.BeatmapLevels.Clear();
            int songCount = 0;
            Log.LogMsg($"Processing songs for playlist ID {playlist.PlaylistID}...");
            var totalSongs = playlist.SongList.Count();
            var songMod = Math.Ceiling((double)totalSongs / (double)10);
            if (songMod < 1)
                songMod = 1;
            foreach (var song in playlist.SongList.ToList())
            {
                songCount++;
                if (songCount % songMod == 0)
                    Console.WriteLine($"{songCount.ToString().PadLeft(5)} of {totalSongs}...");

                if (UpdateSongConfig(manager, song, loader))
                {
                    if (levelCollection.BeatmapLevels.Any(x => x.Object.LevelID == song.LevelData.LevelID))
                    {
                        Log.LogErr($"Playlist ID '{playlist.PlaylistID}' already contains song ID '{song.SongID}' once, removing the second link");
                    }
                    else
                    {
                        levelCollection.BeatmapLevels.Add(song.LevelData.PtrFrom(levelCollection));
                        continue;
                    }
                }

                playlist.SongList.Remove(song);
            }
            Console.WriteLine($"Proccessed {totalSongs} for playlist ID {playlist.PlaylistID}");
        }

        private bool UpdateSongConfig(AssetsManager manager, BeatSaberSong song, CustomLevelLoader loader)
        {

            var songsAssetFile = manager.GetAssetsFile(BSConst.KnownFiles.SongsAssetsFilename);
            BeatmapLevelDataObject level = null;
            if (!string.IsNullOrWhiteSpace(song.SongID))
            {
                var levels = songsAssetFile.FindAssets<BeatmapLevelDataObject>(x => x.Object.LevelID == song.SongID).Select(x=>x.Object).ToList();
                if (levels.Count() > 0)
                {
                    if (levels.Count() > 1)
                        Log.LogErr($"Song ID {song.SongID} already has more than one entry in the assets, this may cause problems!");
                    else
                        Log.LogMsg($"Song ID {song.SongID} exists already and won't be loaded");
                    level = levels.First();
                    song.LevelData = level;
                    return true;
                }
                else
                {
                    Log.LogMsg($"Song ID '{song.SongID}' does not exist and will be created");
                }
            }
            if (level != null && !string.IsNullOrWhiteSpace(song.CustomSongFolder))
            {
                Log.LogErr("SongID and CustomSongsFolder are both set and the level already exists.  The existing one will be used and CustomSongsFolder won'tbe imported again.");
                return false;
            }

            //load new song
            if (!string.IsNullOrWhiteSpace(song.CustomSongFolder))
            {
                try
                {
                    string oggPath;
                    var deser = loader.DeserializeFromJson(song.CustomSongFolder, song.SongID);
                    var found = songsAssetFile.FindAssets<BeatmapLevelDataObject>(x => x.Object.LevelID == deser.LevelID).Select(x=> x.Object).FirstOrDefault();
                    if (found != null)
                    {
                        Log.LogErr($"No song id was specified, but the level {found.LevelID} is already in the assets, skipping it.");
                        song.LevelData = found;
                        return true;
                    }
                    level = loader.LoadSongToAsset(deser, song.CustomSongFolder, out oggPath, true);
                    song.SourceOgg = oggPath;
                }
                catch (Exception ex)
                {
                    Log.LogErr($"Exception loading custom song folder '{song.CustomSongFolder}', skipping it", ex);
                    return false;
                }

                if (level == null)
                {
                    Log.LogErr($"Song at folder '{song.CustomSongFolder}' failed to load, skipping it");
                    return false;
                }

                song.LevelData = level;
                return true;
            }
            //level == null && string.IsNullOrWhiteSpace(song.CustomSongFolder)

            Log.LogErr($"Song ID '{song.SongID}' either was not specified or could not be found and no CustomSongFolder was specified, skipping it.");
            return false;

        }

        private void RemoveLevelAssets(AssetsManager manager, BeatmapLevelDataObject level, List<string> audioFilesToDelete)
        {
            Log.LogMsg($"Removing assets for song id '{level.LevelID}'");
            var file17 = manager.GetAssetsFile(BSConst.KnownFiles.SongsAssetsFilename);
            file17.DeleteObject(level);
            var cover = level.CoverImageTexture2D.Object;
            if (cover == null)
            {
                Log.LogErr($"Could not find cover for song id '{level.LevelID}' to remove it");
            }
            else
            {
                file17.DeleteObject(cover);
            }
            foreach (var diff in level.DifficultyBeatmapSets)
            {
                foreach (var diffbm in diff.DifficultyBeatmaps)
                {
                    file17.DeleteObject(diffbm.BeatmapDataPtr.Object);
                }
            }
            var audioClip = level.AudioClip.Object;
            if (audioClip == null)
            {
                Log.LogErr($"Could not find audio clip asset for song id '{level.LevelID}' to remove it");
            }
            else
            {
                audioFilesToDelete.Add(audioClip.Resource.Source);
                file17.DeleteObject(audioClip);
            }
            
        }

        private void RemoveLevelPackAssets(AssetsManager manager, BeatmapLevelPackObject levelPack)
        {
            var songsAssetFile = manager.GetAssetsFile(BSConst.KnownFiles.SongsAssetsFilename);

            Log.LogMsg($"Removing assets for playlist ID '{ levelPack.PackID}'");
            var collection = levelPack.BeatmapLevelCollection.Object;
            var sprite = levelPack.CoverImage.Object;
            var texture = sprite.Texture.Object;
            songsAssetFile.DeleteObject(levelPack);
            songsAssetFile.DeleteObject(collection);            
            songsAssetFile.DeleteObject(texture);            
            songsAssetFile.DeleteObject(sprite);
        }

        public BeatSaberQuestomConfig GetCurrentConfig(bool suppressImages = false)
        {
            using (var apkFileProvider = new ApkAssetsFileProvider(_apkFilename, ApkAssetsFileProvider.FileCacheMode.Memory, true))
            {
                var manager = new AssetsManager(apkFileProvider, BSConst.GetAssetTypeMap(), false);
                manager.GetAssetsFile("globalgamemanagers");

                var config = GetConfig(manager, suppressImages);

                //clear out any of the internal refs that were used so the GC can clean things up
                foreach (var p in config.Playlists)
                {
                    p.CoverArtSprite = null;
                    p.LevelPackObject = null;
                    foreach (var song in p.SongList)
                    {
                        song.LevelData = null;
                        song.SourceOgg = null;
                    }
                }
                return config;
            }
        }

        private BeatSaberQuestomConfig GetConfig(AssetsManager manager, bool suppressImages)
        {
            BeatSaberQuestomConfig config = new BeatSaberQuestomConfig();
            var file19 = manager.GetAssetsFile(BSConst.KnownFiles.MainCollectionAssetsFilename);
            var file17 = manager.GetAssetsFile(BSConst.KnownFiles.SongsAssetsFilename);
            var mainPack = GetMainLevelPack(manager);
            foreach (var packPtr in mainPack.BeatmapLevelPacks)
            {
                var pack = packPtr.Target.Object;
                if (HideOriginalPlaylists && BSConst.KnownLevelPackIDs.Contains(pack.PackID))
                    continue;

                var packModel = new BeatSaberPlaylist() { PlaylistName = pack.PackName, PlaylistID = pack.PackID, LevelPackObject = pack };
                var collection = pack.BeatmapLevelCollection.Object;

                //get cover art for playlist
                if (!suppressImages)
                {
                    try
                    {
                        var coverSprite = pack.CoverImage.Object;
                        var coverTex = coverSprite.Texture.Object;
                        packModel.CoverArt = coverTex.ToBitmap();
                        packModel.CoverArtBase64PNG = packModel.CoverArt.ToBase64PNG();
                    }
                    catch (Exception ex)
                    {
                        Log.LogErr($"Unable to convert texture for playlist ID '{pack.PackID}' cover art", ex);
                    }
                }
                foreach (var songPtr in collection.BeatmapLevels)
                {
                    var songObj = songPtr.Object;
                    var songModel = new BeatSaberSong()
                    {
                        LevelAuthorName = songObj.LevelAuthorName,
                        SongID = songObj.LevelID,
                        SongAuthorName = songObj.SongAuthorName,
                        SongName = songObj.SongName,
                        SongSubName = songObj.SongSubName,
                        LevelData = songObj
                    };
                    if (!suppressImages)
                    {
                        try
                        {
                            var songCover = songObj.CoverImageTexture2D.Object;
                            try
                            {
                                songModel.CoverArt = songCover.ToBitmap();
                                songModel.CoverArtBase64PNG = songModel.CoverArt.ToBase64PNG();
                            }
                            catch (Exception ex)
                            {
                                Log.LogErr($"Unable to convert texture for song ID '{songModel.SongID}' cover", ex);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.LogErr($"Exception loading/converting the cover image for song id '{songObj.LevelID}'", ex);
                        }
                    }
                    packModel.SongList.Add(songModel);
                }
                config.Playlists.Add(packModel);
            }
            return config;
        }

        public void UpdateConfig(BeatSaberQuestomConfig config)
        {
            //todo: basic validation of the config
            if (_readOnly)
                throw new InvalidOperationException("Cannot update in read only mode.");

            using (var apkFileProvider = new ApkAssetsFileProvider(_apkFilename, ApkAssetsFileProvider.FileCacheMode.Memory, false))
            {
                var manager = new AssetsManager(apkFileProvider, BSConst.GetAssetTypeMap(), false);
                manager.GetAssetsFile("globalgamemanagers");

                //get the old config before we start on this
                var originalConfig = GetConfig(manager, false);

                //get existing playlists and their songs
                //compare with new ones
                //generate a diff
                //etc.

                UpdateColorConfig(config.Colors);

                UpdateTextConfig(config.TextChanges);

                var songsAssetFile = manager.GetAssetsFile(BSConst.KnownFiles.SongsAssetsFilename);
                foreach (var playlist in config.Playlists)
                {
                    UpdatePlaylistConfig(manager, playlist);
                }

                //open the assets with the main levels collection, find the file index of sharedassets17.assets, and add the playlists to it
                var mainLevelsFile = manager.GetAssetsFile(BSConst.KnownFiles.MainCollectionAssetsFilename);
                var file17Index = mainLevelsFile.GetFileIDForFilename(BSConst.KnownFiles.SongsAssetsFilename);
                var mainLevelPack = GetMainLevelPack(manager);


                var packsToUnlink = mainLevelPack.BeatmapLevelPacks.Where(x => !HideOriginalPlaylists || !BSConst.KnownLevelPackIDs.Contains(x.Object.PackID)).ToList();
                var packsToRemove = mainLevelPack.BeatmapLevelPacks.Where(x => !BSConst.KnownLevelPackIDs.Contains(x.Object.PackID) && !config.Playlists.Any(y => y.PlaylistID == x.Object.PackID)).Select(x => x.Object).ToList();
                foreach (var unlink in packsToUnlink)
                {
                    mainLevelPack.BeatmapLevelPacks.Remove(unlink);
                    unlink.Dispose();
                }

                var oldSongs = originalConfig.Playlists.SelectMany(x => x.SongList).Select(x => x.LevelData).Distinct();
                var newSongs = config.Playlists.SelectMany(x => x.SongList).Select(x => x.LevelData).Distinct();

                //don't allow removal of the actual tracks or level packs that are built in, although you can unlink them from the main list
                var removeSongs = oldSongs.Where(x => !newSongs.Contains(x) && !BSConst.KnownLevelIDs.Contains(x.LevelID)).Distinct().ToList();

                var addedSongs = newSongs.Where(x => !oldSongs.Contains(x));

                var removedPlaylistCount = originalConfig.Playlists.Where(x => !config.Playlists.Any(y => y.PlaylistID == x.PlaylistID)).Count();
                var newPlaylistCount = config.Playlists.Where(x => !originalConfig.Playlists.Any(y => y.PlaylistID == x.PlaylistID)).Count();
                //
                //
                //TODO: clean up cover art, it's leaking!
                //
                //
                List<string> audioFilesToDelete = new List<string>();
                removeSongs.ForEach(x => RemoveLevelAssets(manager, x, audioFilesToDelete));

                packsToRemove.ForEach(x => RemoveLevelPackAssets(manager, x));

                //relink all the level packs in order
                var addPacks = config.Playlists.Select(x => x.LevelPackObject.PtrFrom(mainLevelPack));
                mainLevelPack.BeatmapLevelPacks.AddRange(addPacks);

                //do a first loop to guess at the file size
                Int64 originalApkSize = new FileInfo(_apkFilename).Length;
                Int64 sizeGuess = originalApkSize;
                foreach (var pl in config.Playlists)
                {
                    foreach (var sng in pl.SongList)
                    {
                        if (sng.SourceOgg != null)
                        {
                            var clip = sng.LevelData.AudioClip.Object;
                            sizeGuess += new FileInfo(sng.SourceOgg).Length;
                        }
                    }
                }
                foreach (var toDelete in audioFilesToDelete)
                {
                    sizeGuess -= apkFileProvider.GetFileSize(BSConst.KnownFiles.AssetsRootPath + toDelete);
                }

                Log.LogMsg("");
                Log.LogMsg("Playlists:");
                Log.LogMsg($"  Added:   {newPlaylistCount}");
                Log.LogMsg($"  Removed: {removedPlaylistCount}");
                Log.LogMsg("");
                Log.LogMsg("Songs:");
                Log.LogMsg($"  Added:   {addedSongs.Count()}");
                Log.LogMsg($"  Removed: {removeSongs.Count()}");
                Log.LogMsg("");
                Log.LogMsg($"Original APK size:     {originalApkSize:n0}");
                Log.LogMsg($"Guesstimated new size: {sizeGuess:n0}");
                Log.LogMsg("");

                if (sizeGuess > Int32.MaxValue)
                {
                    Log.LogErr("***************ERROR*****************");
                    Log.LogErr($"Guesstimating a file size around {sizeGuess / (Int64)1000000}MB , this will crash immediately upon launch.");
                    Log.LogErr($"The file size MUST be less than {Int32.MaxValue / (int)1000000}MB");
                    Log.LogErr("***************ERROR*****************");
                    throw new OverflowException("File might exceed 2.1GB, aborting.");
                }

                ////////START WRITING DATA


                Log.LogMsg("Serializing all assets...");
                manager.WriteAllOpenAssets();

                //todo: save here?


                foreach (var pl in config.Playlists)
                {
                    foreach (var sng in pl.SongList)
                    {
                        if (sng.SourceOgg != null)
                        {
                            var clip = sng.LevelData.AudioClip.Object;
                            apkFileProvider.WriteFile(sng.SourceOgg, BSConst.KnownFiles.AssetsRootPath + clip.Resource.Source, true, false);
                            //saftey check to make sure we aren't removing a file we just put here
                            if (audioFilesToDelete.Contains(clip.Resource.Source))
                            {
                                Log.LogErr($"Level id '{sng.LevelData.LevelID}' wrote file '{clip.Resource.Source}' that was on the delete list...");
                                audioFilesToDelete.Remove(clip.Resource.Source);
                            }
                        }

                        //todo: save on some interval to save ram?
                    }
                }

                if (audioFilesToDelete.Count > 0)
                {
                    Log.LogMsg($"Deleting {audioFilesToDelete.ToString()} audio files");
                    foreach (var toDelete in audioFilesToDelete)
                    {
                        //Log.LogMsg($"Deleting audio file {toDelete}");
                        apkFileProvider.Delete(BSConst.KnownFiles.AssetsRootPath + toDelete);
                    }
                }
                apkFileProvider.Save();
            }
        }

        public void UpdateColorConfig(SimpleColorSO[] colors)
        {
            var manager = GetColorManager();

            var colorA = colors[0];
            var colorB = colors[1];

            if (colorA != null)
            {
                (manager.ColorA.Object as SimpleColorSO).color = colorA.color;
            }
            if (colorB != null)
                (manager.ColorB.Object as SimpleColorSO).color = colorB.color;
            // Reset
            if (colorA == null && colorB == null)
            {
                (manager.ColorA.Object as SimpleColorSO).color = BSConst.Colors.DefaultColorA;
                (manager.ColorB.Object as SimpleColorSO).color = BSConst.Colors.DefaultColorB;
            }
        }

        public void UpdateTextConfig(List<(string, string)> texts)
        {
            var textAsset = GetBeatSaberTextAsset();
            var textKeyPairs = Utils.TextUtils.ReadLocaleText(textAsset.Script, new List<char>() { ',', ',', '\n' });
            Utils.TextUtils.ApplyWatermark(textKeyPairs);
            foreach (var kp in texts)
            {
                textKeyPairs[kp.Item1][textKeyPairs[kp.Item1].Count - 1] = kp.Item2;
            }
            textAsset.Script = Utils.TextUtils.WriteLocaleText(textKeyPairs, new List<char>() { ',', ',', '\n' });
        }

        private TextAsset GetBeatSaberTextAsset()
        {
            _manager.GetAssetsFile(BSConst.KnownFiles.TextAssetFilename);
            var textAssets = _manager.MassFindAssets<TextAsset>(x => true).ToList();
            if (textAssets.Count == 0)
                throw new Exception("Unable to find any TextAssets! Perhaps the ClassID/ScriptHash are invalid?");
            // Literally the only object in the TextAssetFile is "BeatSaber" at PathID=1
            return textAssets.Find(a => a.Object.Name == "BeatSaber").Object;
        }

        private ColorManager GetColorManager()
        {
            _manager.GetAssetsFile(BSConst.KnownFiles.ColorAssetsFilename);
            var colorManager = _manager.MassFirstOrDefaultAsset<ColorManager>(x => true)?.Object;
            if (colorManager == null)
                throw new Exception("Unable to find the color manager asset!");
            return colorManager;
        }
        public bool ApplyPatchSettingsFile()
        {
            string filename = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "patchsettings.json");
            if (!File.Exists(filename))
            {
                Log.LogErr($"Can't apply patch settings from file, {filename} does not exist!");
                return false;
            }
            List<FilePatch> patches = null;
            using (StreamReader sr = new StreamReader(filename))
            using (JsonReader jr = new JsonTextReader(sr))
                patches = new JsonSerializer().Deserialize<List<FilePatch>>(jr);
            Log.LogMsg($"Found {patches.Count} files to patch in {filename}");
            foreach (var patch in patches)
            {
                if (!ApplyPatch(patch))
                {
                    Log.LogErr($"Failed to apply patch... aborting any further patching...");
                    return false;
                }
            }
            return true;
        }

        public bool ApplyPatch(FilePatch patch)
        {
            using (var apkFileProvider = new ApkAssetsFileProvider(_apkFilename, ApkAssetsFileProvider.FileCacheMode.Memory, false))
            {
                if (!Patcher.Patch(apkFileProvider, patch))
                {
                    Log.LogErr($"File {patch.Filename} failed to patch!");
                    return false;
                }
            }
            return true;
        }

        public void SignAPK()
        {
            using (var apkFileProvider = new ApkAssetsFileProvider(_apkFilename, ApkAssetsFileProvider.FileCacheMode.None, false))
            {
                ApkSigner signer = new ApkSigner(_pemData);
                signer.Sign(apkFileProvider);
            }
        }

    }
}
