using Emulamer.Utils;
using System;
using System.IO;
using QuestomAssets.AssetsChanger;
using QuestomAssets.BeatSaber;
using System.Collections.Generic;
using System.Linq;


namespace QuestomAssets
{

    public class QuestomAssetsEngine : IDisposable
    {
        private string _apkFilename;
        private Apkifier _apk;
        private bool _readOnly;

        private AssetsManager _manager;
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
            _apk = new Apkifier(apkFilename, !readOnly, readOnly?null:pemCertificateData, readOnly);
            _manager = new AssetsManager(_apk, BSConst.GetAssetTypeMap());
            _manager.GetAssetsFile("globalgamemanagers");
        }

        public BeatSaberQuestomConfig GetCurrentConfig(bool suppressImages = false)
        {
            BeatSaberQuestomConfig config = new BeatSaberQuestomConfig();
            var file19 = _manager.GetAssetsFile(BSConst.KnownFiles.MainCollectionAssetsFilename);
            var file17 = _manager.GetAssetsFile(BSConst.KnownFiles.SongsAssetsFilename);
            var mainPack = GetMainLevelPack();
            foreach (var packPtr in mainPack.BeatmapLevelPacks)
            {
                var pack = packPtr.Target.Object;
                if (HideOriginalPlaylists && BSConst.KnownLevelPackIDs.Contains(pack.PackID))
                    continue;

                var packModel = new BeatSaberPlaylist() { PlaylistName = pack.PackName, PlaylistID = pack.PackID, LevelPackObject = pack };
                var collection = pack.BeatmapLevelCollection.Object;
                packModel.LevelCollection = collection;

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

        private void UpdatePlaylistConfig(BeatSaberPlaylist playlist)
        {
            Log.LogMsg($"Processing playlist ID {playlist.PlaylistID}...");
            var songsAssetFile = _manager.GetAssetsFile(BSConst.KnownFiles.SongsAssetsFilename);
            BeatmapLevelPackObject levelPack = null;
            BeatmapLevelCollectionObject levelCollection = null;
            levelPack = songsAssetFile.FindAsset<BeatmapLevelPackObject>(x=> x.Object.PackID == playlist.PlaylistID)?.Object;
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
            }
            else
            {
                Log.LogMsg($"Level pack for playlist '{playlist.PlaylistID}' was found and will be updated");
                levelCollection = levelPack.BeatmapLevelCollection.Object;
                if (levelCollection == null)
                {
                    Log.LogErr($"{nameof(BeatmapLevelCollectionObject)} was not found for playlist id {playlist.PlaylistID}!  It will be created, but something is wrong with the assets!");
                }
            }
            if (levelCollection == null)
            {
                levelCollection = new BeatmapLevelCollectionObject(songsAssetFile)
                { Name = playlist.PlaylistID + BSConst.NameSuffixes.LevelCollection };
                songsAssetFile.AddObject(levelCollection, true);
                levelPack.BeatmapLevelCollection = levelCollection.PtrFrom(levelPack);
            }

            playlist.LevelCollection = levelCollection;
            playlist.LevelPackObject = levelPack;

            levelPack.PackName = playlist.PlaylistName;
            if (playlist.CoverArt != null)
            {
                Log.LogMsg($"Loading cover art for playlist ID '{playlist.PlaylistID}'");

                playlist.CoverArtSprite = CustomLevelLoader.LoadPackCover(playlist.PlaylistID, songsAssetFile, playlist.CoverArt);
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
                    playlist.CoverArtSprite = CustomLevelLoader.LoadPackCover(playlist.PlaylistID, songsAssetFile, null);
                }
                playlist.LevelPackObject.CoverImage = playlist.CoverArtSprite.PtrFrom(playlist.LevelPackObject);
            }

            //clear out any levels, we'll add them back
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

                if (UpdateSongConfig(song))
                {
                    if (playlist.LevelCollection.BeatmapLevels.Any(x => x.Object.LevelID == song.LevelData.LevelID))
                    {
                        Log.LogErr($"Playlist ID '{playlist.PlaylistID}' already contains song ID '{song.SongID}' once, removing the second link");
                    }
                    else
                    {
                        playlist.LevelCollection.BeatmapLevels.Add(song.LevelData.PtrFrom(playlist.LevelCollection));
                        continue;
                    }
                }

                playlist.SongList.Remove(song);
            }
            Console.WriteLine($"Proccessed {totalSongs} for playlist ID {playlist.PlaylistID}");
        }

        private bool UpdateSongConfig(BeatSaberSong song)
        {

            var songsAssetFile = _manager.GetAssetsFile(BSConst.KnownFiles.SongsAssetsFilename);
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
                    var deser = CustomLevelLoader.DeserializeFromJson(songsAssetFile, song.CustomSongFolder, song.SongID);
                    var found = songsAssetFile.FindAssets<BeatmapLevelDataObject>(x => x.Object.LevelID == deser.LevelID).Select(x=> x.Object).FirstOrDefault();
                    if (found != null)
                    {
                        Log.LogErr($"No song id was specified, but the level {found.LevelID} is already in the assets, skipping it.");
                        song.LevelData = found;
                        return true;
                    }
                    level = CustomLevelLoader.LoadSongToAsset(deser, song.CustomSongFolder, songsAssetFile, out oggPath, true);
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

        private void RemoveLevelAssets(BeatmapLevelDataObject level, List<string> audioFilesToDelete)
        {
            Log.LogMsg($"Removing assets for song id '{level.LevelID}'");
            var file17 = _manager.GetAssetsFile(BSConst.KnownFiles.SongsAssetsFilename);
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

        private void RemoveLevelPackAssets(BeatmapLevelPackObject levelPack)
        {
            var songsAssetFile = _manager.GetAssetsFile(BSConst.KnownFiles.SongsAssetsFilename);

            Log.LogMsg($"Removing assets for playlist ID '{ levelPack.PackID}'");

            songsAssetFile.DeleteObject(levelPack);
            var collection = levelPack.BeatmapLevelCollection.Object;
            var sprite = levelPack.CoverImage.Object;
            var texture = sprite.Texture.Object;
            songsAssetFile.DeleteObject(collection);            
            songsAssetFile.DeleteObject(texture);            
            songsAssetFile.DeleteObject(sprite);            
        }


        public void UpdateConfig(BeatSaberQuestomConfig config)
        {
            //todo: basic validation of the config
            if (_readOnly)
                throw new InvalidOperationException("Cannot update in read only mode.");
 
            //get the old config before we start on this
            var originalConfig = GetCurrentConfig();

            //get existing playlists and their songs
            //compare with new ones
            //generate a diff
            //etc.
            var songsAssetFile = _manager.GetAssetsFile(BSConst.KnownFiles.SongsAssetsFilename);

            foreach (var playlist in config.Playlists)
            {
                UpdatePlaylistConfig(playlist);
            }


            //open the assets with the main levels collection, find the file index of sharedassets17.assets, and add the playlists to it
            var mainLevelsFile = _manager.GetAssetsFile(BSConst.KnownFiles.MainCollectionAssetsFilename);
            var file17Index = mainLevelsFile.GetFileIDForFilename(BSConst.KnownFiles.SongsAssetsFilename);
            var mainLevelPack = GetMainLevelPack();


            var packsToUnlink = mainLevelPack.BeatmapLevelPacks.Where(x => !HideOriginalPlaylists || !BSConst.KnownLevelPackIDs.Contains(x.Object.PackID)).ToList();
            var packsToRemove = mainLevelPack.BeatmapLevelPacks.Where(x => !BSConst.KnownLevelPackIDs.Contains(x.Object.PackID)).ToList();
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
            removeSongs.ForEach(x => RemoveLevelAssets(x, audioFilesToDelete));

            packsToRemove.ForEach(x => x.Dispose());
            packsToRemove.ForEach(x => RemoveLevelPackAssets(x.Object));

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
                sizeGuess -= _apk.GetFileSize(BSConst.KnownFiles.AssetsRootPath + toDelete);
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
                Log.LogErr($"Proceeding anyways, but you've been warned");
            }

            ////////START WRITING DATA
            foreach (var pl in config.Playlists)
            {
                foreach (var sng in pl.SongList)
                {
                    if (sng.SourceOgg != null)
                    {
                        var clip = sng.LevelData.AudioClip.Object;
                        _apk.Write(sng.SourceOgg, BSConst.KnownFiles.AssetsRootPath + clip.Resource.Source, true, false);
                        //saftey check to make sure we aren't removing a file we just put here
                        if (audioFilesToDelete.Contains(clip.Resource.Source))
                        {
                            Log.LogErr($"Level id '{sng.LevelData.LevelID}' wrote file '{clip.Resource.Source}' that was on the delete list...");
                            audioFilesToDelete.Remove(clip.Resource.Source);
                        }
                    }
                }
            }

            if (audioFilesToDelete.Count > 0)
            {
                Log.LogMsg($"Deleting {audioFilesToDelete.ToString()} audio files");
                foreach (var toDelete in audioFilesToDelete)
                {
                    //Log.LogMsg($"Deleting audio file {toDelete}");
                    _apk.Delete(BSConst.KnownFiles.AssetsRootPath + toDelete);
                }
            }

            Log.LogMsg("Serializing all assets...");
            _manager.WriteAllOpenAssets();
        }



        private MainLevelPackCollectionObject GetMainLevelPack()
        {
            var mainLevelPack = _manager.MassFirstOrDefaultAsset<MainLevelPackCollectionObject>(x => true)?.Object;
            if (mainLevelPack == null)
                throw new Exception("Unable to find the main level pack collection object!");
            return mainLevelPack;
        }

        public bool ApplyBeatmapSignaturePatch()
        {
            return Utils.Patcher.PatchBeatmapSigCheck(_apk);
        }

        #region Helper Functions

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (_apk != null)
                        _apk.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

    }
}
