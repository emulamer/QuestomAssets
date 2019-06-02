using Emulamer.Utils;
using System;
using System.IO;
using QuestomAssets.AssetsChanger;
using QuestomAssets.BeatSaber;
using System.Collections.Generic;
using System.Linq;

namespace QuestomAssets
{
    public class QustomAssetsEngine : IDisposable
    {
        private string _apkFilename;
        private Apkifier _apk;
        private bool _readOnly;

        /// <summary>
        /// Create a new instance of the class and open the apk file
        /// </summary>
        /// <param name="apkFilename">The path to the Beat Saber APK file</param>
        /// <param name="pemCertificateData">The contents of the PEM certificate that will be used to sign the APK.  If omitted, a new self signed cert will be generated.</param>
        public QustomAssetsEngine(string apkFilename, string pemCertificateData = null, bool readOnly = false)
        {
            _readOnly = readOnly;
            _apkFilename = apkFilename;
            _apk = new Apkifier(apkFilename, !readOnly, readOnly?null:pemCertificateData, readOnly);
        }

        private Dictionary<string, AssetsFile> _openAssetsFiles = new Dictionary<string, AssetsFile>();

        private AssetsFile OpenFile(string assetsFilename)
        {
            if (_openAssetsFiles.ContainsKey(assetsFilename))
                return _openAssetsFiles[assetsFilename];
            AssetsFile assetsFile = new AssetsFile(_apk.ReadCombinedAssets(assetsFilename), BeatSaberConstants.GetAssetTypeMap());
            _openAssetsFiles.Add(assetsFilename, assetsFile);
            return assetsFile;
        }

        public BeatSaberQustomConfig GetCurrentConfig()
        {
            BeatSaberQustomConfig config = new BeatSaberQustomConfig();
            var file19 = OpenFile(BeatSaberConstants.KnownFiles.FullMainCollectionAssetsPath);
            var file17 = OpenFile(BeatSaberConstants.KnownFiles.FullSongsAssetsPath);
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
                //don't include stock packs in this
                if (BeatSaberConstants.KnownLevelPackNames.Contains(pack.Name))
                    continue;
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

                    packModel.SongList.Add(songModel);
                }

                config.Playlists.Add(packModel);
            }
            return config;
        }

        public void UpdateConfig(BeatSaberQustomConfig config)
        {
            if (_readOnly)
                throw new InvalidOperationException("Cannot update in read only mode.");
            UpdateKnownObjects();
            throw new NotImplementedException();
        }


        private MainLevelPackCollectionObject GetMainLevelPack()
        {
            var file19 = OpenFile(BeatSaberConstants.KnownFiles.FullMainCollectionAssetsPath);
            var mainLevelPack = file19.FindAsset<MainLevelPackCollectionObject>();
            if (mainLevelPack == null)
                throw new Exception("Unable to find the main level pack collection object!");
            return mainLevelPack;
        }

        public bool ApplyBeatmapSignaturePatch()
        {
            return Utils.Patcher.PatchBeatmapSigCheck(_apk);
        }

        //this is crap, I need to load all files and resolve file pointers properly
        private void UpdateKnownObjects()
        {
            var songsFile = OpenFile(BeatSaber.BeatSaberConstants.KnownFiles.FullSongsAssetsPath);
            if (!songsFile.Metadata.ExternalFiles.Any(x => x.FileName == BeatSaberConstants.KnownFiles.File19))
            {
                songsFile.Metadata.ExternalFiles.Add(new ExternalFile()
                {
                    FileName = BeatSaberConstants.KnownFiles.File19,
                    AssetName = "",
                    ID = Guid.Empty,
                    Type = 0
                });
            }
            int file19 = songsFile.GetFileIDForFilename(BeatSaberConstants.KnownFiles.File19);
            KnownObjects.File17.MonstercatEnvironment = new PPtr(file19, KnownObjects.File17.MonstercatEnvironment.PathID);
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
