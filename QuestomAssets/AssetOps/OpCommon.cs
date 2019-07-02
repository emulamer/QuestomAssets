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
            context.Cache.PlaylistCache.Add(playlist.PlaylistID, new PlaylistAndSongs() { Playlist = levelPack });
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
                playlist.CoverArtSprite = loader.LoadPackCover(playlist.PlaylistID, playlist.CoverImageBytes);
                playlist.LevelPackObject.CoverImage = playlist.CoverArtSprite.PtrFrom(playlist.LevelPackObject);
                if (oldCoverImage != null)
                {
                    if (oldCoverImage.Object != null)
                        songsAssetFile.DeleteObject(oldCoverImage.Object);

                    oldCoverImage.Dispose();
                }
                if (oldTex != null)
                {
                    if (oldTex?.Object != null)
                        songsAssetFile.DeleteObject(oldTex.Object);

                    oldTex.Dispose();
                }
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
        }

        public static void DeleteSong(OpContext context, string songID)
        {
            var songsAssetFile = context.Engine.GetSongsAssetsFile();
            var song = context.Cache.SongCache[songID];
            var songPtr = song.Playlist.BeatmapLevelCollection.Object.BeatmapLevels.FirstOrDefault(x => x.Object == song.Song);
            if (songPtr == null)
                throw new Exception("Song pointer could not be found in the playlist.");

            var sourceFile = song.Song?.AudioClip?.Object?.Resource?.Source;

            song.Playlist.BeatmapLevelCollection.Object.BeatmapLevels.Remove(songPtr);
            songPtr.Dispose();
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
            context.Cache.SongCache.Remove(song.Song.LevelID);
            context.Cache.PlaylistCache[song.Playlist.PackID].Songs.Remove(song.Song.LevelID);

            context.Engine.QueuedFileOperations.Add(new QueuedFileOp() { Type = QueuedFileOperationType.DeleteFolder, TargetPath = context.Config.SongsPath.CombineFwdSlash(songID) });
        }
    }
}
