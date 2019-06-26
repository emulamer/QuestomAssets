using QuestomAssets.BeatSaber;
using QuestomAssets.Models;
using System;
using System.Collections.Generic;
using System.Text;
using QuestomAssets.AssetsChanger;

namespace QuestomAssets.AssetOps
{
    public class AddOrUpdatePlaylistOp : AssetOp
    {
        public AddOrUpdatePlaylistOp(BeatSaberPlaylist playlist)
        {
            Playlist = playlist;
        }

        public BeatSaberPlaylist Playlist { get; private set; }
        internal override void PerformOp(OpContext context)
        {
            if (string.IsNullOrEmpty(Playlist.PlaylistID))
                throw new Exception("PlaylistID must be provided.");

            if (string.IsNullOrEmpty(Playlist.PlaylistName))
                throw new Exception($"PlaylistName must be provided for ID {Playlist.PlaylistName}");

            var songsAssetFile = context.Engine.GetSongsAssetsFile();
            BeatmapLevelPackObject levelPack = null;
            if (context.Cache.PlaylistCache.ContainsKey(Playlist.PlaylistID))
            {
                Log.LogMsg($"Playlist {Playlist.PlaylistID} will be updated");
                levelPack = context.Cache.PlaylistCache[Playlist.PlaylistID].Playlist;
                levelPack.Name = Playlist.PlaylistName;
                Playlist.LevelPackObject = levelPack;
            }
            else
            {
                Log.LogMsg($"Playlist {Playlist.PlaylistID} will be created");
                levelPack = new BeatmapLevelPackObject(songsAssetFile)
                {
                    Enabled = 1,
                    GameObject = null,
                    IsPackAlwaysOwned = true,
                    PackID = Playlist.PlaylistID,
                    Name = Playlist.PlaylistID + BSConst.NameSuffixes.LevelPack,
                    PackName = Playlist.PlaylistName
                };
                songsAssetFile.AddObject(levelPack, true);
                var col = new BeatmapLevelCollectionObject(songsAssetFile)
                { Name = Playlist.PlaylistID + BSConst.NameSuffixes.LevelCollection };
                songsAssetFile.AddObject(col, true);
                levelPack.BeatmapLevelCollection = col.PtrFrom(levelPack);
                Playlist.LevelPackObject = levelPack;
                var mainCol = context.Engine.GetMainLevelPack();
                var aoPacks = context.Engine.GetAlwaysOwnedModel();
                mainCol.BeatmapLevelPacks.Add(levelPack.PtrFrom(mainCol));
                aoPacks.AlwaysOwnedPacks.Add(levelPack.PtrFrom(aoPacks));
                context.Cache.PlaylistCache.Add(Playlist.PlaylistID, new QuestomAssetsEngine.MusicConfigCache.PlaylistAndSongs() { Playlist = levelPack });
            }
            UpdateCoverImage(context, songsAssetFile);
        }

        private void UpdateCoverImage(OpContext context, AssetsFile songsAssetFile)
        {        
            CustomLevelLoader loader = new CustomLevelLoader(songsAssetFile, context.Config);
            if (Playlist.CoverImageBytes != null && Playlist.CoverImageBytes.Length > 0)
            {
                Log.LogMsg($"Loading cover art for playlist ID '{Playlist.PlaylistID}'");

                var oldCoverImage = Playlist?.LevelPackObject?.CoverImage;
                var oldTex = Playlist?.LevelPackObject?.CoverImage?.Object?.Texture;

                //todo: verify this is a good place to delete stuff                
                Playlist.CoverArtSprite = loader.LoadPackCover(Playlist.PlaylistID, Playlist.CoverImageBytes);
                Playlist.LevelPackObject.CoverImage = Playlist.CoverArtSprite.PtrFrom(Playlist.LevelPackObject);
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
                if (Playlist.LevelPackObject.CoverImage != null)
                {
                    Playlist.CoverArtSprite = Playlist.LevelPackObject.CoverImage.Object;
                }
                else
                {
                    Playlist.CoverArtSprite = loader.LoadPackCover(Playlist.PlaylistID, null);
                }
                Playlist.LevelPackObject.CoverImage = Playlist.CoverArtSprite.PtrFrom(Playlist.LevelPackObject);
            }
        }

    }
}
