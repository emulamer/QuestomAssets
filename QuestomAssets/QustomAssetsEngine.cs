using Emulamer.Utils;
using System;
using System.IO;
using QuestomAssets.AssetsChanger;
using QuestomAssets.BeatSaber;
using System.Collections.Generic;
using System.Linq;


namespace QuestomAssets
{
    [SmartPtrAware]
    public class Test : AssetsObject
    {
        public SmartPtr<MonoBehaviourObject> mo { get; set; }
    }
    public class QuestomAssetsEngine : IDisposable
    {
        private string _apkFilename;
        private Apkifier _apk;
        private bool _readOnly;

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
            Test t = new Test();
            //t.mo = new SmartPtr<MonoBehaviourObject>();
            _readOnly = readOnly;
            _apkFilename = apkFilename;
            _apk = new Apkifier(apkFilename, !readOnly, readOnly?null:pemCertificateData, readOnly);
        }

        public BeatSaberQuestomConfig GetCurrentConfig(bool suppressImages = false)
        {
            //BeatSaberQuestomConfig config = new BeatSaberQuestomConfig();
            //var file19 = OpenAssets(BSConst.KnownFiles.MainCollectionAssetsFilename);
            //var file17 = OpenAssets(BSConst.KnownFiles.SongsAssetsFilename);
            //var mainPack = GetMainLevelPack();
            //foreach (var packPtr in mainPack.BeatmapLevelPacks)
            //{
            //    if (file19.GetFilenameForFileID(packPtr.FileID) != BSConst.KnownFiles.SongsAssetsFilename)
            //        throw new NotImplementedException("Songs and packs are only supported in one file currently.");
            //    var pack = file17.GetAssetByID<BeatmapLevelPackObject>(packPtr.PathID);
            //    if (pack == null)
            //    {
            //        Log.LogErr($"Level pack with path ID {packPtr} was not found in {BSConst.KnownFiles.SongsAssetsFilename}!");
            //        continue;
            //    }
            //    if (HideOriginalPlaylists && BSConst.KnownLevelPackIDs.Contains(pack.PackID))
            //        continue;

            //    var packModel = new BeatSaberPlaylist() { PlaylistName = pack.PackName, PlaylistID = pack.PackID, LevelPackObject = pack };
            //    //TODO: check file ref?  right now they're all in 17
            //    var collection = file17.GetAssetByID<BeatmapLevelCollectionObject>(pack.BeatmapLevelCollection.PathID);
            //    if (collection == null)
            //    {
            //        Log.LogErr($"Failed to find level pack collection object for playlist {pack.PackName}");
            //        continue;
            //    }
            //    packModel.LevelCollection = collection;

            //    //get cover art for playlist
            //    if (!suppressImages)
            //    {
            //        try
            //        {
            //            var coverSprite = file17.GetAssetByID<SpriteObject>(pack.CoverImage.PathID);
            //            if (coverSprite == null)
            //                throw new Exception("Unable to find cover art sprite.");
            //            var coverTex = file17.GetAssetByID<Texture2DObject>(coverSprite.Texture.PathID);
            //            if (coverTex == null)
            //                throw new Exception("Unable to find cover art texture.");
            //            packModel.CoverArt = coverTex.ToBitmap();
            //            packModel.CoverArtBase64PNG = packModel.CoverArt.ToBase64PNG();
            //        }
            //        catch (Exception ex)
            //        {
            //            Log.LogErr($"Unable to convert texture for playlist ID '{pack.PackID}' cover art", ex);
            //        }
            //    }
            //    foreach (var songPtr in collection.BeatmapLevels)
            //    {
            //        var songObj = file17.GetAssetByID<BeatmapLevelDataObject>(songPtr.PathID);
            //        if (songObj == null)
            //        {
            //            Log.LogErr($"Failed to find beatmap level data for playlist {pack.PackName} with path id {songPtr.PathID}!");
            //            continue;
            //        }
            //        var songModel = new BeatSaberSong()
            //        {
            //            LevelAuthorName = songObj.LevelAuthorName,
            //            SongID = songObj.LevelID,
            //            SongAuthorName = songObj.SongAuthorName,
            //            SongName = songObj.SongName,
            //            SongSubName = songObj.SongSubName,
            //            LevelData = songObj
            //        };
            //        if (!suppressImages)
            //        {
            //            try
            //            {
            //                var songCover = file17.GetAssetByID<Texture2DObject>(songObj.CoverImageTexture2D.PathID);
            //                if (songCover == null)
            //                {
            //                    Log.LogErr($"The cover image for song id '{songObj.LevelID}' could not be found!");
            //                }
            //                else
            //                {
            //                    try
            //                    {
            //                        songModel.CoverArt = songCover.ToBitmap();
            //                        songModel.CoverArtBase64PNG = songModel.CoverArt.ToBase64PNG();
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                        Log.LogErr($"Unable to convert texture for song ID '{songModel.SongID}' cover", ex);
            //                    }
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                Log.LogErr($"Exception loading/converting the cover image for song id '{songObj.LevelID}'", ex);
            //            }
            //        }
            //        packModel.SongList.Add(songModel);
            //    }
            //    config.Playlists.Add(packModel);
            //}
            //return config;
            return null;
        }

        private void UpdatePlaylistConfig(BeatSaberPlaylist playlist)
        {
            //Log.LogMsg($"Processing playlist ID {playlist.PlaylistID}...");
            //var songsAssetFile = OpenAssets(BSConst.KnownFiles.SongsAssetsFilename);
            //BeatmapLevelPackObject levelPack = null;
            //BeatmapLevelCollectionObject levelCollection = null;
            //levelPack = songsAssetFile.FindAssets<BeatmapLevelPackObject>(x=>x.PackID == playlist.PlaylistID).FirstOrDefault();
            ////create a new level pack if one waasn't found
            //if (levelPack == null)
            //{
            //    Log.LogMsg($"Level pack for playlist '{playlist.PlaylistID}' was not found and will be created");
            //    //don't try to find the cover name, just let it create a dupe, we'll try to clean up linked things we did later
            //    //var packCover = CustomLevelLoader.LoadPackCover(playlist.PlaylistID, songsAssetFile, playlist.CoverArtFile);
            //    //playlist.CoverArtSprite = packCover;
            //    levelPack = new BeatmapLevelPackObject(songsAssetFile)
            //    {
            //        Enabled = 1,
            //        GameObjectPtr = null,
            //        IsPackAlwaysOwned = true,
            //        PackID = playlist.PlaylistID,
            //        Name = playlist.PlaylistID + BSConst.NameSuffixes.LevelPack,
            //        PackName = playlist.PlaylistName
            //    };
            //    songsAssetFile.AddObject(levelPack, true);
            //}
            //else
            //{
            //    Log.LogMsg($"Level pack for playlist '{playlist.PlaylistID}' was found and will be updated");
            //    levelCollection = songsAssetFile.GetAssetByID<BeatmapLevelCollectionObject>(levelPack.BeatmapLevelCollection.Target.ObjectID);
            //    if (levelCollection == null)
            //    {
            //        Log.LogErr($"{nameof(BeatmapLevelCollectionObject)} was not found for playlist id {playlist.PlaylistID}!  It will be created, but something is wrong with the assets!");
            //    }
            //}
            //if (levelCollection == null)
            //{
            //    levelCollection = new BeatmapLevelCollectionObject(songsAssetFile)
            //    { Name = playlist.PlaylistID + BSConst.NameSuffixes.LevelCollection };
            //    songsAssetFile.AddObject(levelCollection, true);
            //    levelPack.BeatmapLevelCollection = levelCollection.ObjectInfo.LocalPtrTo;
            //}

            //playlist.LevelCollection = levelCollection;
            //playlist.LevelPackObject = levelPack;

            //levelPack.PackName = playlist.PlaylistName;
            //if (playlist.CoverArt != null)
            //{
            //    Log.LogMsg($"Loading cover art for playlist ID '{playlist.PlaylistID}'");

            //    playlist.CoverArtSprite = CustomLevelLoader.LoadPackCover(playlist.PlaylistID, songsAssetFile, playlist.CoverArt);
            //    playlist.LevelPackObject.CoverImage = playlist.CoverArtSprite.ObjectInfo.LocalPtrTo;
            //}
            //if (playlist.LevelPackObject.CoverImage != null)
            //{
            //    playlist.CoverArtSprite = songsAssetFile.GetAssetByID<SpriteObject>(playlist.LevelPackObject.CoverImage.PathID);
            //}
            //else
            //{
            //    playlist.CoverArtSprite = CustomLevelLoader.LoadPackCover(playlist.PlaylistID, songsAssetFile, null);
            //}
            //playlist.LevelPackObject.CoverImage = playlist.CoverArtSprite.ObjectInfo.LocalPtrTo;

            ////clear out any levels, we'll add them back
            //levelCollection.BeatmapLevels.Clear();
            //int songCount = 0;
            //Log.LogMsg($"Processing songs for playlist ID {playlist.PlaylistID}...");
            //var totalSongs = playlist.SongList.Count();
            //var songMod = Math.Ceiling((double)totalSongs / (double)10);
            //if (songMod < 1)
            //    songMod = 1;
            //foreach (var song in playlist.SongList.ToList())
            //{
            //    songCount++;
            //    if (songCount % songMod == 0)
            //        Console.WriteLine($"{songCount.ToString().PadLeft(5)} of {totalSongs}...");

            //    if (UpdateSongConfig(song))
            //    {
            //        if (playlist.LevelCollection.BeatmapLevels.Any(x => x.PathID == song.LevelData.ObjectInfo.ObjectID))
            //        {
            //            Log.LogErr($"Playlist ID '{playlist.PlaylistID}' already contains song ID '{song.SongID}' once, removing the second link");
            //        }
            //        else
            //        {
            //            playlist.LevelCollection.BeatmapLevels.Add(song.LevelData.ObjectInfo.LocalPtrTo);
            //            continue;
            //        }
            //    }
                
            //    playlist.SongList.Remove(song);
            //}
            //Console.WriteLine($"Proccessed {totalSongs} for playlist ID {playlist.PlaylistID}");
        }

        private bool UpdateSongConfig(BeatSaberSong song)
        {
            return false;
            //var songsAssetFile = OpenAssets(BSConst.KnownFiles.SongsAssetsFilename);
            //BeatmapLevelDataObject level = null;
            //if (!string.IsNullOrWhiteSpace(song.SongID))
            //{
            //    var levels = songsAssetFile.FindAssets<BeatmapLevelDataObject>(x => x.LevelID == song.SongID);
            //    if (levels.Count() > 0)
            //    {
            //        if (levels.Count() > 1)
            //            Log.LogErr($"Song ID {song.SongID} already has more than one entry in the assets, this may cause problems!");
            //        else
            //            Log.LogMsg($"Song ID {song.SongID} exists already and won't be loaded");
            //        level = levels.First();
            //        song.LevelData = level;
            //        return true;
            //    }
            //    else
            //    {
            //        Log.LogMsg($"Song ID '{song.SongID}' does not exist and will be created");
            //    }
            //}
            //if (level != null && !string.IsNullOrWhiteSpace(song.CustomSongFolder))
            //{
            //    Log.LogErr("SongID and CustomSongsFolder are both set and the level already exists.  The existing one will be used and CustomSongsFolder won'tbe imported again.");
            //    return false;
            //}

            ////load new song
            //if (!string.IsNullOrWhiteSpace(song.CustomSongFolder))
            //{
            //    try
            //    {
            //        string oggPath;
            //        var deser = CustomLevelLoader.DeserializeFromJson(songsAssetFile, song.CustomSongFolder, song.SongID);
            //        var found = songsAssetFile.FindAssets<BeatmapLevelDataObject>(x => x.LevelID == deser.LevelID).FirstOrDefault();
            //        if (found != null)
            //        {
            //            Log.LogErr($"No song id was specified, but the level {found.LevelID} is already in the assets, skipping it.");
            //            song.LevelData = found;
            //            return true;
            //        }
            //        level = CustomLevelLoader.LoadSongToAsset(deser, song.CustomSongFolder, songsAssetFile, out oggPath, true);
            //        song.SourceOgg = oggPath;
            //    }
            //    catch (Exception ex)
            //    {
            //        Log.LogErr($"Exception loading custom song folder '{song.CustomSongFolder}', skipping it", ex);
            //        return false;
            //    }

            //    if (level == null)
            //    {
            //        Log.LogErr($"Song at folder '{song.CustomSongFolder}' failed to load, skipping it");
            //        return false;
            //    }

            //    song.LevelData = level;
            //    return true;
            //}
            ////level == null && string.IsNullOrWhiteSpace(song.CustomSongFolder)
            
            //Log.LogErr($"Song ID '{song.SongID}' either was not specified or could not be found and no CustomSongFolder was specified, skipping it.");
            //return false;
            
        }

        private void RemoveLevelAssets(BeatmapLevelDataObject level, List<string> audioFilesToDelete)
        {
            //Log.LogMsg($"Removing assets for song id '{level.LevelID}'");
            //var file17 = OpenAssets(BSConst.KnownFiles.SongsAssetsFilename);
            //var cover = file17.GetAssetByID<Texture2DObject>(level.CoverImageTexture2D.PathID);
            //if (cover == null)
            //{
            //    Log.LogErr($"Could not find cover for song id '{level.LevelID}' to remove it");
            //}
            //else
            //{
            //    file17.DeleteObject(cover);
            //}
            //foreach (var diff in level.DifficultyBeatmapSets)
            //{
            //    foreach (var diffbm in diff.DifficultyBeatmaps)
            //    {                    
            //        file17.DeleteObject(file17.GetAssetByID<AssetsObject>(diffbm.BeatmapDataPtr.PathID));
            //    }
            //}
            //var audioClip = file17.GetAssetByID<AudioClipObject>(level.AudioClip.PathID);
            //if (audioClip == null)
            //{
            //    Log.LogErr($"Could not find audio clip asset for song id '{level.LevelID}' to remove it");
            //}
            //else
            //{
            //    audioFilesToDelete.Add(audioClip.Resource.Source);
            //    file17.DeleteObject(audioClip);
            //}
            //file17.DeleteObject(level);
        }

        private void RemoveLevelPackAssets(BeatmapLevelPackObject levelPack)
        {
            //var songsAssetFile = OpenAssets(BSConst.KnownFiles.SongsAssetsFilename);

            //Log.LogMsg($"Removing assets for playlist ID '{ levelPack.PackID}'");
            //var file17 = OpenAssets(BSConst.KnownFiles.SongsAssetsFilename);
            //var collection = file17.GetAssetByID<BeatmapLevelCollectionObject>(levelPack.BeatmapLevelCollection.PathID);
            //file17.DeleteObject(collection);
            //file17.DeleteObject(levelPack);
            //var sprite = songsAssetFile.GetAssetByID<SpriteObject>(levelPack.CoverImage.PathID);
            //songsAssetFile.DeleteObject(sprite);
            //var texture = songsAssetFile.GetAssetByID<Texture2DObject>(sprite.Texture.PathID);
            //songsAssetFile.DeleteObject(texture);
        }


        public void UpdateConfig(BeatSaberQuestomConfig config)
        {
            ////todo: basic validation of the config
            //if (_readOnly)
            //    throw new InvalidOperationException("Cannot update in read only mode.");
            //UpdateKnownObjects();

            ////get the old config before we start on this
            //var originalConfig = GetCurrentConfig();

            ////get existing playlists and their songs
            ////compare with new ones
            ////generate a diff
            ////etc.
            //var songsAssetFile = OpenAssets(BSConst.KnownFiles.SongsAssetsFilename);

            //foreach (var playlist in config.Playlists)
            //{
            //    UpdatePlaylistConfig(playlist);            
            //}


            ////open the assets with the main levels collection, find the file index of sharedassets17.assets, and add the playlists to it
            //var mainLevelsFile = OpenAssets(BSConst.KnownFiles.MainCollectionAssetsFilename);
            //var file17Index = mainLevelsFile.GetFileIDForFilename(BSConst.KnownFiles.SongsAssetsFilename);
            //var mainLevelPack = mainLevelsFile.FindAsset<MainLevelPackCollectionObject>();


            ////TODO: move this all to RemoveLevelPackAsset
            //List<BeatmapLevelPackObject> packsToRemove = new List<BeatmapLevelPackObject>();
            //List<PPtr> levelPackPointersToUnlink = new List<PPtr>();
            ////List<BeatmapLevelCollectionObject> collectionsToRemove = new List<BeatmapLevelCollectionObject>();
            //foreach (var packPtr in mainLevelPack.BeatmapLevelPacks)
            //{
            //    if (packPtr.FileID != file17Index)
            //    {
            //        Log.LogMsg("One of the beatmap level packs is in another file, not removing it");
            //        continue;
            //    }
            //    var pack = songsAssetFile.GetAssetByID<BeatmapLevelPackObject>(packPtr.PathID);
            //    //not sure if I should remove it or leave it here... if one ends up in another asset file it'll break
            //    if (pack == null)
            //    {
            //        Log.LogErr("Unable to locate one of the beatmap level packs referenced in the main collection, removing the link");
            //        levelPackPointersToUnlink.Add(packPtr);
            //        continue;
            //    }
                
            //    if (pack.BeatmapLevelCollection.FileID != 0)
            //    {
            //        Log.LogMsg("One of the beatmap level pack collections is in another file, not removing it");
            //        continue;
            //    }
            //    var packCollection = songsAssetFile.GetAssetByID<BeatmapLevelCollectionObject>(pack.BeatmapLevelCollection.PathID);
            //    if (packCollection == null)
            //    {
            //        Log.LogErr($"Unable to locate the level collection for '{pack.PackID}', removing the link");
            //        levelPackPointersToUnlink.Add(packPtr);
            //        continue;
            //    }
            //    if (config.Playlists.Any(x => x.LevelPackObject.ObjectInfo.ObjectID == pack.ObjectInfo.ObjectID))
            //    {
            //        //unlink it so we can relink it in order
            //        levelPackPointersToUnlink.Add(packPtr);
            //        continue;
            //    }
            //    if (BSConst.KnownLevelPackIDs.Contains(pack.PackID))
            //    {
            //        if (!HideOriginalPlaylists)
            //            levelPackPointersToUnlink.Add(packPtr);
            //        continue;
            //    }

            //    levelPackPointersToUnlink.Add(packPtr);
            //    packsToRemove.Add(pack);
            //}
  
            //var oldSongs = originalConfig.Playlists.SelectMany(x => x.SongList).Select(x => x.LevelData).Distinct();
            //var newSongs = config.Playlists.SelectMany(x => x.SongList).Select(x => x.LevelData).Distinct();
                        
            ////don't allow removal of the actual tracks or level packs that are built in, although you can unlink them from the main list
            //var removeSongs = oldSongs.Where(x => !newSongs.Contains(x) && !BSConst.KnownLevelIDs.Contains(x.LevelID)).Distinct().ToList();

            //var addedSongs = newSongs.Where(x => !oldSongs.Contains(x));

            //var removedPlaylistCount = originalConfig.Playlists.Where(x => !config.Playlists.Any(y => y.PlaylistID == x.PlaylistID)).Count();
            //var newPlaylistCount = config.Playlists.Where(x => !originalConfig.Playlists.Any(y => y.PlaylistID == x.PlaylistID)).Count();
            ////
            ////
            ////TODO: clean up cover art, it's leaking!
            ////
            ////
            //List<string> audioFilesToDelete = new List<string>();
            //removeSongs.ForEach(x => RemoveLevelAssets(x, audioFilesToDelete));

            //packsToRemove.ForEach(x => RemoveLevelPackAssets(x));

            //levelPackPointersToUnlink.ForEach(x => mainLevelPack.BeatmapLevelPacks.Remove(x));

            ////relink all the level packs in order
            //var addPacks = config.Playlists.Select(x => new PPtr(file17Index, x.LevelPackObject.ObjectInfo.ObjectID));
            //mainLevelPack.BeatmapLevelPacks.AddRange(addPacks);

            ////do a first loop to guess at the file size
            //Int64 originalApkSize = new FileInfo(_apkFilename).Length;
            //Int64 sizeGuess = originalApkSize;
            //foreach (var pl in config.Playlists)
            //{
            //    foreach (var sng in pl.SongList)
            //    {
            //        if (sng.SourceOgg != null)
            //        {
            //            var clip = songsAssetFile.GetAssetByID<AudioClipObject>(sng.LevelData.AudioClip.PathID);
            //            sizeGuess += new FileInfo(sng.SourceOgg).Length;
            //        }
            //    }
            //}
            //foreach (var toDelete in audioFilesToDelete)
            //{
            //    sizeGuess -= _apk.GetFileSize(BSConst.KnownFiles.AssetsRootPath + toDelete);
            //}

            //Log.LogMsg("");
            //Log.LogMsg("Playlists:");
            //Log.LogMsg($"  Added:   {newPlaylistCount}");
            //Log.LogMsg($"  Removed: {removedPlaylistCount}");
            //Log.LogMsg("");
            //Log.LogMsg("Songs:");
            //Log.LogMsg($"  Added:   {addedSongs.Count()}");
            //Log.LogMsg($"  Removed: {removeSongs.Count()}");
            //Log.LogMsg("");
            //Log.LogMsg($"Original APK size:     {originalApkSize:n0}");
            //Log.LogMsg($"Guesstimated new size: {sizeGuess:n0}");
            //Log.LogMsg("");
            

            //if (sizeGuess > Int32.MaxValue)
            //{
            //    Log.LogErr("***************ERROR*****************");
            //    Log.LogErr($"Guesstimating a file size around {sizeGuess / (Int64)1000000}MB , this will crash immediately upon launch.");
            //    Log.LogErr($"The file size MUST be less than {Int32.MaxValue / (int)1000000}MB");
            //    Log.LogErr("***************ERROR*****************");
            //    Log.LogErr($"Proceeding anyways, but you've been warned");
            //}

            //////////START WRITING DATA
            //foreach (var pl in config.Playlists)
            //{
            //    foreach (var sng in pl.SongList)
            //    {
            //        if (sng.SourceOgg != null)
            //        {
            //            var clip = songsAssetFile.GetAssetByID<AudioClipObject>(sng.LevelData.AudioClip.PathID);
            //            _apk.Write(sng.SourceOgg, BSConst.KnownFiles.AssetsRootPath+clip.Resource.Source, true, false);
            //            //saftey check to make sure we aren't removing a file we just put here
            //            if (audioFilesToDelete.Contains(clip.Resource.Source))
            //            {
            //                Log.LogErr($"Level id '{sng.LevelData.LevelID}' wrote file '{clip.Resource.Source}' that was on the delete list...");
            //                audioFilesToDelete.Remove(clip.Resource.Source);
            //            }
            //        }
            //    }
            //}

            //if (audioFilesToDelete.Count > 0)
            //{
            //    Log.LogMsg($"Deleting {audioFilesToDelete.ToString()} audio files");
            //    foreach (var toDelete in audioFilesToDelete)
            //    {
            //        //Log.LogMsg($"Deleting audio file {toDelete}");
            //        _apk.Delete(BSConst.KnownFiles.AssetsRootPath + toDelete);
            //    }
            //}

            //Log.LogMsg("Serializing all assets...");
            //WriteAllOpenAssets();
        }



        private MainLevelPackCollectionObject GetMainLevelPack()
        {
            //var file19 = OpenAssets(BSConst.KnownFiles.MainCollectionAssetsFilename);
            //var mainLevelPack = file19.FindAsset<MainLevelPackCollectionObject>();
            //if (mainLevelPack == null)
            //    throw new Exception("Unable to find the main level pack collection object!");
            //return mainLevelPack;
            return null;
        }

        public bool ApplyBeatmapSignaturePatch()
        {
            return Utils.Patcher.PatchBeatmapSigCheck(_apk);
        }

        //this is crap, I need to load all files and resolve file pointers properly
        private void UpdateKnownObjects()
        {
            throw new NotImplementedException();
            //var songsFile = OpenAssets(BeatSaber.BSConst.KnownFiles.SongsAssetsFilename);
            //if (!songsFile.Metadata.ExternalFiles.Any(x => x.FileName == BSConst.KnownFiles.File19))
            //{
            //    songsFile.Metadata.ExternalFiles.Add(new ExternalFile()
            //    {
            //        FileName = BSConst.KnownFiles.File19,
            //        AssetName = "",
            //        ID = Guid.Empty,
            //        Type = 0
            //    });
            //}
            //songsFile = OpenAssets(BeatSaber.BSConst.KnownFiles.SongsAssetsFilename);
            //if (!songsFile.Metadata.ExternalFiles.Any(x => x.FileName == BSConst.KnownFiles.File14))
            //{
            //    songsFile.Metadata.ExternalFiles.Add(new ExternalFile()
            //    {
            //        FileName = BSConst.KnownFiles.File19,
            //        AssetName = "",
            //        ID = Guid.Empty,
            //        Type = 0
            //    });
            //}
            //int file19 = songsFile.GetFileIDForFilename(BSConst.KnownFiles.File19);
            //int file14 = songsFile.GetFileIDForFilename(BSConst.KnownFiles.File14);

            //KnownObjects.File17.MonstercatEnvironment = new PPtr(file19, KnownObjects.File17.MonstercatEnvironment.PathID);
            //KnownObjects.File17.NiceEnvironment = new PPtr(file14, KnownObjects.File17.NiceEnvironment.PathID);
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
