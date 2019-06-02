using Emulamer.Utils;
using System;
using System.IO;
using QuestomAssets.AssetsChanger;
using QuestomAssets.BeatSaber;
using System.Collections.Generic;

namespace QuestomAssets
{
    public class QustomAssetsEngine : IDisposable
    {
        private string _apkFilename;
        private Apkifier _apk;
        /// <summary>
        /// Create a new instance of the class and open the apk file
        /// </summary>
        /// <param name="apkFilename">The path to the Beat Saber APK file</param>
        /// <param name="pemCertificateData">The contents of the PEM certificate that will be used to sign the APK.  If omitted, a new self signed cert will be generated.</param>
        public QustomAssetsEngine(string apkFilename, string pemCertificateData = null)
        {
            _apkFilename = apkFilename;
            _apk = new Apkifier(apkFilename, true, pemCertificateData);
        }

        private Dictionary<string, AssetsFile> _openAssetsFiles = new Dictionary<string, AssetsFile>();

        private AssetsFile OpenFile(string assetsFilename)
        {
            if (_openAssetsFiles.ContainsKey(assetsFilename))
                return _openAssetsFiles[assetsFilename];
            AssetsFile assetsFile = new AssetsFile(_apk.Read(assetsFilename).ToStream(), BeatSaberConstants.GetAssetTypeMap());
            _openAssetsFiles.Add(assetsFilename, assetsFile);
            return assetsFile;
        }

        public BeatSaberQustomConfig GetCurrentConfig()
        {
            BeatSaberQustomConfig config = new BeatSaberQustomConfig();
            var file19 = OpenFile(BeatSaberConstants.KnownFiles.MainCollectionAssetsFullPath);
            var file17 = OpenFile(BeatSaberConstants.KnownFiles.SongsAssetsFullPath);
            var mainPack = GetMainLevelPack();
            foreach (var packPtr in mainPack.BeatmapLevelPacks)
            {
                if (file19.GetFilenameForFileID(packPtr.FileID) != BeatSaberConstants.KnownFiles.SongsAssetsFilename)
                    throw new NotImplementedException("Songs and packs are only supported in one file currently.");
                var pack = file17.GetAssetByID<BeatmapLevelPackObject>(packPtr.PathID);
                if (pack == null)
                {
                    Log.LogErr($"Level pack with path ID {packPtr} was not found in {BeatSaberConstants.KnownFiles.SongsAssetsFilename}!");
                    continue;
                }
                //TODO: cover art, ETC pack and all that
                var packModel = new BeatSaberPlaylist() { PlaylistName = pack.PackName, PlaylistID = pack.Name, LevelPackPathID = pack.ObjectInfo.ObjectID };
                //TODO: check file ref?
                var collection = file17.GetAssetByID<BeatmapLevelCollectionObject>(pack.BeatmapLevelCollection.PathID);
                if (collection == null)
                {
                    Log.LogErr($"Failed to find level pack collection object for playlist {pack.PackName}");
                    continue;
                }
                packModel.levelCollectionPathID = collection.ObjectInfo.ObjectID;

                foreach (var songPtr in collection.BeatmapLevels)
                {
                    var songObj = file17.GetAssetByID<BeatmapLevelDataObject>(songPtr.PathID);
                    if (songObj == null)
                    {
                        Log.LogErr($"Failed to find beatmap level data for playlist {pack.PackName} with path id {songPtr.PathID}!");
                        continue;
                    }

                    var songModel = new BeatSaberSong()
                    {
                        LevelAuthorName = songObj.LevelAuthorName,
                        LevelID = songObj.LevelID,
                        SongAuthorName = songObj.SongAuthorName,
                        SongName = songObj.SongName,
                        SongSubName = songObj.SongSubName,
                        CoverArtPtr = songObj.CoverImageTexture2D,
                        LevelPathID = songObj.ObjectInfo.ObjectID
                    };
                    //TODO: cover art here, easier but still a hassle

                }

            }

        }


        private MainLevelPackCollectionObject GetMainLevelPack()
        {
            var file19 = OpenFile(BeatSaberConstants.KnownFiles.MainCollectionAssetsFullPath);
            var mainLevelPack = file19.FindAsset<MainLevelPackCollectionObject>();
            if (mainLevelPack == null)
                throw new Exception("Unable to find the main level pack collection object!");
            return mainLevelPack;
        }

        public bool ApplyBeatmapSignaturePatch()
        {
            return Utils.Patcher.PatchBeatmapSigCheck(_apk);
        }



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
