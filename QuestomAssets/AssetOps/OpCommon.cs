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
                        try
                        {
                            OpCommon.DeleteSong(context, song.Key);
                        }
                        catch (Exception ex)
                        {
                            Log.LogErr($"Exception trying to delete song id {song.Key} while deleting playlist id {playlist.Playlist.PackID}, will be unlinked in cache.  This may leave unused data in the assets.", ex);
                            try
                            {
                                context.Cache.SongCache.Remove(song.Value.Song.LevelID);
                                playlist.Songs.Remove(song.Value.Song.LevelID);
                            }
                            catch (Exception ex2)
                            {
                                Log.LogErr($"Exception cleaning up cache for song id {song.Key} in playlist if {playlist.Playlist.PackID} while recovering from a failed delete.", ex2);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //really this shouldn't ever get hit anymore.  Probably can remove it.
                    Log.LogErr($"Deleting songs on playlist ID {playlist?.Playlist?.PackID} failed! Attempting to recover by removing links to songs, although this may leave extra stuff in assets!");
                    try
                    {
                        var bmCol = playlist?.Playlist?.BeatmapLevelCollection?.Object?.BeatmapLevels?.ToList();
                        if (bmCol != null)
                        {
                            bmCol.ForEach(x => { playlist.Playlist.BeatmapLevelCollection.Object.BeatmapLevels.Remove(x); x.Dispose(); });
                        }
                    }
                    catch (Exception ex2)
                    {
                        Log.LogErr($"Failed to recover by removing song pointers while deleting playlist {playlist?.Playlist?.PackID}!  This will definitely leave stuff in assets.", ex2);
                    }
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
                plParent.DeleteObject(playlist.Playlist.CoverImage.Object.RenderData.Texture.Object);
                playlist.Playlist.CoverImage.Object.RenderData.Texture.Dispose();
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
                var oldTex = playlist?.LevelPackObject?.CoverImage?.Object?.RenderData?.Texture;

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
                    //don't erase base content from the assets, although this definitely leaks textures if you keep switching the stock level pack picture
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
                var tex = playlist.LevelPackObject?.CoverImage?.Object?.RenderData?.Texture?.Object;
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
                throw new Exception($"Song pointer is null trying to delete song ID {songID}");
            try
            {
                song.Playlist.BeatmapLevelCollection.Object.BeatmapLevels.Remove(songPtr);
                songPtr.Dispose();
                //don't delete built in songs
                if (!BSConst.KnownLevelIDs.Contains(songID))
                {
                    if (song.Song.AudioClip?.Target?.Object != null)
                    {
                        songsAssetFile.DeleteObject(song.Song.AudioClip.Object);
                    }
                    else
                    {
                        Log.LogErr($"Trying to delete song ID {songID} and audio clip is null!");
                    }
                    if (song.Song?.CoverImageTexture2D?.Target?.Object != null)
                    {
                        songsAssetFile.DeleteObject(song.Song.CoverImageTexture2D.Object);
                    }
                    else
                    {
                        Log.LogErr($"Trying to delete song ID {songID} and cover image is null!");
                    }
                    if (song.Song.CoverImageTexture2D != null)
                    {
                        song.Song.CoverImageTexture2D.Dispose();
                    }
                    if (song?.Song?.DifficultyBeatmapSets == null)
                    {
                        Log.LogErr($"Trying to delete song id {songID}, but DifficultyBeatmapSets is null!");
                    }
                    else
                    {
                        song.Song.DifficultyBeatmapSets.ForEach(x =>
                        {
                            if (x?.DifficultyBeatmaps == null)
                            {
                                Log.LogErr($"Trying to delete song id {songID} beatmapset, but DifficultyBeatmaps is null!");
                            }
                            else
                            {
                                x.DifficultyBeatmaps.ForEach(y =>
                                {
                                    if (y != null && y.BeatmapDataPtr != null)
                                    {
                                        if (y?.BeatmapDataPtr?.Object != null)
                                        {
                                            songsAssetFile.DeleteObject(y.BeatmapDataPtr.Object);
                                        }
                                        else
                                        {
                                            Log.LogErr($"Trying to delete song id {songID} but BeatmapDataPtr.Object is null!");
                                        }
                                        y.BeatmapDataPtr.Dispose();
                                    }
                                    else
                                    {
                                        Log.LogErr($"Trying to delete song id {songID} but BeatmapDataPtr is null!");
                                    }
                                });
                            }
                            x.BeatmapCharacteristic.Dispose();
                        });
                    }
                    songsAssetFile.DeleteObject(song.Song);
                    context.Engine.QueuedFileOperations.Add(new QueuedFileOp() { Tag = song.Song.LevelID, Type = QueuedFileOperationType.DeleteFolder, TargetPath = context.Config.SongsPath.CombineFwdSlash(songID) });
                }
                context.Cache.SongCache.Remove(song.Song.LevelID);
                context.Cache.PlaylistCache[song.Playlist.PackID].Songs.Remove(song.Song.LevelID);
            }
            catch ( Exception ex)
            {
                Log.LogErr($"Exception deleting song ID {songID}!", ex);
            }
        }
    }
}
