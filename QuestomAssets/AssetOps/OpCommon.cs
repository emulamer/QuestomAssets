using QuestomAssets.AssetsChanger;
using QuestomAssets.BeatSaber;
using QuestomAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static QuestomAssets.MusicConfigCache;

namespace QuestomAssets.AssetOps
{
    internal static class OpCommon
    {

        public static void DeletePlaylist(OpContext context, string playlistID, bool deleteSongsOnPlaylist)
        {
            var playlist = context.Cache.PlaylistCache[playlistID];

            if (deleteSongsOnPlaylist)
            {
                try
                {
                    foreach (var song in playlist.Songs.ToList())
                    {
                        OpCommon.DeleteSong(context, song.Key);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error while deleting songs from playlist id {playlist.Playlist.PackID}", ex);
                }
                playlist.Songs.Clear();
            }
            //this should be done in song delete already
            //playlist.Playlist.BeatmapLevelCollection.Object.BeatmapLevels.ForEach(x => { x.Target.ParentFile.DeleteObject(x.Object); x.Dispose(); });
            var mlp = context.Engine.GetMainLevelPack();
            var aop = context.Engine.GetAlwaysOwnedModel();
            var mlpptr = mlp.BeatmapLevelPacks.FirstOrDefault(x => x.Object.PackID == playlist.Playlist.PackID);
            var aopptr = aop.AlwaysOwnedPacks.FirstOrDefault(x => x.Object.PackID == playlist.Playlist.PackID);
            if (mlpptr != null)
            {
                mlp.BeatmapLevelPacks.Remove(mlpptr);
                mlpptr.Dispose();
            }
            else
            {
                Log.LogErr($"The playlist id {playlist.Playlist.PackID} didn't exist in the main level packs");
            }
            if (aopptr != null)
            {
                aop.AlwaysOwnedPacks.Remove(aopptr);
                aopptr.Dispose();
            }
            else
            {
                Log.LogErr($"The playlist id {playlist.Playlist.PackID} didn't exist in the always owned level packs");
            }
            //don't delete built in packs assets, just unlink them
            if (!BSConst.KnownLevelPackIDs.Contains(playlist.Playlist.PackID))
            {
                var plParent = playlist.Playlist.ObjectInfo.ParentFile;
                plParent.DeleteObject(playlist.Playlist.CoverImage.Object.Texture.Object);
                playlist.Playlist.CoverImage.Object.Texture.Dispose();
                plParent.DeleteObject(playlist.Playlist.CoverImage.Object);
                plParent.DeleteObject(playlist.Playlist);
                playlist.Playlist.CoverImage.Dispose();
            }
            context.Cache.PlaylistCache.Remove(playlistID);
        }

        public static BeatmapLevelPackObject CreatePlaylist(OpContext context, BeatSaberPlaylist playlist, AssetsFile songsAssetFile)
        {
            Log.LogMsg($"Playlist {playlist.PlaylistID} will be created");
            var levelPack = new BeatmapLevelPackObject(songsAssetFile)
            {
                Enabled = 1,
                GameObject = null,
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
            playlist.LevelPackObject = levelPack;
            var mainCol = context.Engine.GetMainLevelPack();
            var aoPacks = context.Engine.GetAlwaysOwnedModel();
            mainCol.BeatmapLevelPacks.Add(levelPack.PtrFrom(mainCol));
            aoPacks.AlwaysOwnedPacks.Add(levelPack.PtrFrom(aoPacks));
            context.Cache.PlaylistCache.Add(playlist.PlaylistID, new PlaylistAndSongs() { Playlist = levelPack, Order = context.Cache.PlaylistCache.Count });
            UpdateCoverImage(playlist, context, songsAssetFile);
            return levelPack;
        }

        public static void UpdateCoverImage(BeatSaberPlaylist playlist, OpContext context, AssetsFile songsAssetFile)
        {
            CustomLevelLoader loader = new CustomLevelLoader(songsAssetFile, context.Config);
            if (playlist.CoverImageBytes != null && playlist.CoverImageBytes.Length > 0)
            {
                Log.LogMsg($"Loading cover art for playlist ID '{playlist.PlaylistID}'");

                var oldCoverImage = playlist?.LevelPackObject?.CoverImage;
                var oldTex = playlist?.LevelPackObject?.CoverImage?.Object?.Texture;

                //todo: verify this is a good place to delete stuff      
                try
                {
                    playlist.CoverArtSprite = loader.LoadPackCover(playlist.PlaylistID, playlist.CoverImageBytes);
                    playlist.LevelPackObject.CoverImage = playlist.CoverArtSprite.PtrFrom(playlist.LevelPackObject);
                }
                catch (Exception ex)
                {
                    Log.LogErr("Exception in step 1!",ex);
                    throw;
                }
                try
                {
                    //don't erase base content from the assets
                    if (!BSConst.KnownLevelPackIDs.Contains(playlist.PlaylistID))
                    {
                        if (oldTex != null)
                        {
                            if (oldTex?.Object != null)
                                songsAssetFile.DeleteObject(oldTex.Object);

                            oldTex.Dispose();
                        }
                        if (oldCoverImage != null)
                        {
                            if (oldCoverImage.Object != null)
                                songsAssetFile.DeleteObject(oldCoverImage.Object);

                            oldCoverImage.Dispose();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.LogErr("Exception trying to clean up playlist cover art!  This may leak cover images!", ex);
                }
            }
            else
            {
                try
                {
                    if (playlist.LevelPackObject.CoverImage != null)
                    {
                        playlist.CoverArtSprite = playlist.LevelPackObject.CoverImage.Object;
                    }
                    else
                    {
                        playlist.CoverArtSprite = loader.LoadPackCover(playlist.PlaylistID, null);
                    }
                }
                catch (Exception ex)
                {
                    Log.LogErr("Exception in the cover art sprite part!", ex);
                    throw;
                }
                try
                {
                    playlist.LevelPackObject.CoverImage = playlist.CoverArtSprite.PtrFrom(playlist.LevelPackObject);
                }
                catch (Exception ex)
                {
                    Log.LogErr("Exception in the final step!", ex);
                    throw;
                }
            }
            //try queueing a file write op to output the playlist cover image
            try
            {
                var tex = playlist.LevelPackObject?.CoverImage?.Object?.Texture?.Object;
                if (tex == null)
                    throw new Exception("Texture couldn't be loaded from the playlist even though it should have just been set...");

                var qfo = new QueuedFileOp() {
                    TargetPath = context.Config.PlaylistsPath.CombineFwdSlash(playlist.PlaylistID + ".png"),
                    Type = QueuedFileOperationType.WriteFile,
                    SourceData = Utils.ImageUtils.Instance.TextureToPngBytes(tex)
                };
                context.Engine.QueuedFileOperations.Add(qfo);
            }
            catch (Exception ex)
            {
                Log.LogErr($"Exception queueing write op for playlist art on {playlist.PlaylistID}!", ex);
            }
        }

        public static void DeleteSong(OpContext context, string songID)
        {
            var songsAssetFile = context.Engine.GetSongsAssetsFile();
            var song = context.Cache.SongCache[songID];
            var songPtr = song.Playlist.BeatmapLevelCollection.Object.BeatmapLevels.FirstOrDefault(x => x.Object == song.Song);
            if (songPtr == null)
                throw new Exception("Song pointer could not be found in the playlist.");

            song.Playlist.BeatmapLevelCollection.Object.BeatmapLevels.Remove(songPtr);
            songPtr.Dispose();
            //don't delete built in songs
            if (!BSConst.KnownLevelIDs.Contains(songID))
            {
                songsAssetFile.DeleteObject(song.Song.AudioClip.Object);
                songsAssetFile.DeleteObject(song.Song.CoverImageTexture2D.Object);
                song.Song.CoverImageTexture2D.Dispose();
                song.Song.DifficultyBeatmapSets.ForEach(x =>
                {
                    x.DifficultyBeatmaps.ForEach(y =>
                    {
                        songsAssetFile.DeleteObject(y.BeatmapDataPtr.Object);
                        y.BeatmapDataPtr.Dispose();
                    });
                    x.BeatmapCharacteristic.Dispose();
                });
                songsAssetFile.DeleteObject(song.Song);
                context.Engine.QueuedFileOperations.Add(new QueuedFileOp() { Type = QueuedFileOperationType.DeleteFolder, TargetPath = context.Config.SongsPath.CombineFwdSlash(songID) });
            }
            context.Cache.SongCache.Remove(song.Song.LevelID);
            context.Cache.PlaylistCache[song.Playlist.PackID].Songs.Remove(song.Song.LevelID);            
        }
    }
}
