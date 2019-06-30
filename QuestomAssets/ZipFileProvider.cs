using Ionic.Zip;
using QuestomAssets.AssetsChanger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace QuestomAssets
{
    public class ZipFileProvider : IFileProvider
    {
        public string SourceName
        {
            get
            {
                return ApkFilename.GetFilenameFwdSlash();
            }
        }
        private Dictionary<string, string> _tempFiles = new Dictionary<string, string>();
        public bool ReadOnly { get; private set; }
        public string ApkFilename { get; private set; }
        public FileCacheMode CacheMode { get; set; }
        private List<Stream> _streamsToClose = new List<Stream>();

        public bool IsOpen
        {
            get
            {
                return _zipFile != null;
            }
        }

        public bool DirectoryExists(string path)
        {
            return FindFiles(path.TrimEnd('/') + "/*").Any();
        }

        public bool UseCombinedStream { get; private set; }
        private ZipFile _zipFile;
        string _tempFolder = null;
        public ZipFileProvider(string apkFilename, FileCacheMode cacheMode, bool readOnly = false, string tempFolder = null, bool useCombinedStream = false)
        {
            _zipFile = new ZipFile(apkFilename, System.Text.Encoding.UTF8);
            UseCombinedStream = useCombinedStream;
            ApkFilename = apkFilename;
            ReadOnly = readOnly;
            CacheMode = cacheMode;
            _tempFolder = tempFolder;
            if (tempFolder != null)
                _zipFile.TempFileFolder = tempFolder;
        }

        public ZipFileProvider(Stream zipFileStream, string zipFilename, FileCacheMode cacheMode, bool readOnly = false, string tempFolder = null, bool useCombinedStream = false)
        {
            _zipFile = ZipFile.Read(zipFileStream, new ReadOptions() { Encoding = System.Text.Encoding.UTF8 });
            ApkFilename = zipFilename;
            UseCombinedStream = useCombinedStream;
            ReadOnly = readOnly;
            CacheMode = cacheMode;
            _tempFolder = tempFolder;
            if (tempFolder != null)
                _zipFile.TempFileFolder = tempFolder;
        }

        //public ZipFileProvider(ZipFile zipFile, FileCacheMode cacheMode, bool readOnly = false, string tempFolder = null, bool useCombinedStream = false)
        //{
        //    _zipFile = zipFile;
        //    UseCombinedStream = useCombinedStream;
        //    ApkFilename = "";
        //    ReadOnly = readOnly;
        //    CacheMode = cacheMode;
        //    _tempFolder = tempFolder;
        //    if (tempFolder != null)
        //        _zipFile.TempFileFolder = tempFolder;
        //}

        private void CheckRO()
        {
            if (ReadOnly)
                throw new NotSupportedException("Cannot modify a read only file.");
        }
        private static bool FilePatternMatch(string filename, string pattern)
        {
            Regex mask = new Regex(pattern.Replace(".", "[.]").Replace("*", ".*").Replace("?", "."));
            return mask.IsMatch(filename);
        }

        public void MkDir(string path)
        {
            //not really relevant for a zip file
        }

        public void DeleteFiles(string pattern)
        {
            CheckRO();
            var toDelete = new List<ZipEntry>();
            foreach (var e in _zipFile.Entries)
            {
                if (FilePatternMatch(e.FileName, pattern))
                    toDelete.Add(e);
            }

            _zipFile.RemoveEntries(toDelete);
        }

        public bool FileExists(string filename)
        {
            return _zipFile.Entries.Any(x => x.FileName.ToLower() == filename.ToLower());
        }

        public void Rename(string fromfile, string tofile)
        {
            foreach (var e in _zipFile.Entries.ToList())
            {
                if (e.FileName == fromfile)
                    e.FileName = tofile;
            }
        }

        public List<string> FindFiles(string pattern)
        {
            var found = new List<string>();
            foreach (var e in _zipFile.EntryFileNames)
            {
                if (FilePatternMatch(e, pattern))
                    found.Add(e);
            }
            return found;
        }

        private Stream GetCacheStream(string filename, Stream stream)
        {
            Stream outStream = null;
            switch (CacheMode)
            {
                case FileCacheMode.None:
                    outStream = stream;
                    break;
                case FileCacheMode.Memory:
                    outStream = new MemoryStream();
                    stream.CopyTo(outStream);
                    stream.Close();
                    stream.Dispose();
                    outStream.Seek(0, SeekOrigin.Begin);
                    break;
                case FileCacheMode.TempFiles:
                    var tempFile = Path.GetTempFileName();
                    _tempFiles.Add(filename, tempFile);
                    outStream = new FileStream(tempFile, FileMode.Create, FileAccess.ReadWrite);
                    stream.CopyTo(outStream);
                    outStream.Seek(0, SeekOrigin.Begin);
                    stream.Close();
                    stream.Dispose();
                    break;
            }
            return outStream;
        }

        public Stream GetReadStream(string filename, bool bypassCache = false)
        {
            var entry = _zipFile.Entries.FirstOrDefault(x => x.FileName.ToLower() == filename.ToLower());
            if (entry == null)
                throw new FileNotFoundException($"An entry named {filename} was not found in the APK.");

            if (bypassCache)
                return entry.OpenReader();

            return GetCacheStream(filename, entry.OpenReader());
        }

        public byte[] Read(string filename)
        {
            using (var reader = GetReadStream(filename))
            {
                return reader.ReadBytes((int)reader.Length);
            }
        }

        public void RmRfDir(string path)
        {
            DeleteFiles(path + "*");
        }

        public void Write(string filename, byte[] data, bool overwrite = true, bool compressData = true)
        {
            CheckRO();
            var entry = _zipFile.Entries.FirstOrDefault(x => x.FileName == filename);
            if (entry != null && !overwrite)
                throw new Exception($"An entry named {filename} already exists and overwrite is false");
            if (entry != null)
                _zipFile.RemoveEntry(entry);
            _zipFile.AddEntry(filename, data);
            _zipFile.CompressionLevel = compressData ? Ionic.Zlib.CompressionLevel.Default : Ionic.Zlib.CompressionLevel.None;
        }
        public void WriteFile(string sourceFilename, string targetFilename, bool overwrite = true, bool compressData = true)
        {
            CheckRO();
            var entry = _zipFile.Entries.FirstOrDefault(x => x.FileName == targetFilename);
            if (entry != null && !overwrite)
                throw new Exception($"An entry named {targetFilename} already exists and overwrite is false");
            if (entry != null)
                _zipFile.RemoveEntry(entry);
            FileStream fs = File.OpenRead(sourceFilename);
            _streamsToClose.Add(fs);
            _zipFile.AddEntry(targetFilename, fs);
            _zipFile.CompressionLevel = compressData ? Ionic.Zlib.CompressionLevel.Default : Ionic.Zlib.CompressionLevel.None;
        }

        /// <summary>
        /// Queues a stream to be written to a file.  Stream will not actually be read until .Save() is called
        /// </summary>
        public void QueueWriteStream(string filename, Stream streamToWrite, bool overwrite = true, bool compressData = true)
        {
            CheckRO();
            var entry = _zipFile.Entries.FirstOrDefault(x => x.FileName == filename);
            if (entry != null && !overwrite)
                throw new Exception($"An entry named {filename} already exists and overwrite is false");
            if (entry != null)
                _zipFile.RemoveEntry(entry);

            _zipFile.AddEntry(filename, streamToWrite);
            _zipFile.CompressionLevel = compressData ? Ionic.Zlib.CompressionLevel.Default : Ionic.Zlib.CompressionLevel.None;
        }

        //todo: untested if this will work right
        public Stream GetWriteStream(string filename)
        {
            CheckRO();
            var entry = _zipFile.Entries.FirstOrDefault(x => x.FileName == filename);
            if (entry != null)
                _zipFile.RemoveEntry(entry);
            var stream = new MemoryStream();
            _zipFile.AddEntry(filename, stream);
            _zipFile.CompressionLevel =Ionic.Zlib.CompressionLevel.Default;
            _streamsToClose.Add(stream);            
            return stream;
        }

        public void Delete(string filename)
        {
            CheckRO();
            var entry = _zipFile.Entries.FirstOrDefault(x => x.FileName == filename);
            if (entry == null)
                throw new FileNotFoundException($"An entry named {filename} was not found in the APK.");
            _zipFile.RemoveEntry(entry);
        }

        //public void WriteFromStream(string filename, Stream data, bool overwrite = true, bool compressData = true)
        //{
        //    CheckRO();
        //    var entry = _zipFile.Entries.FirstOrDefault(x => x.FileName == filename);
        //    if (entry != null && !overwrite)
        //        throw new Exception($"An entry named {filename} already exists and overwrite is false");
        //    if (entry != null)
        //        _zipFile.RemoveEntry(entry);

        //    _zipFile.AddEntry(filename, data);
        //}

        public long GetFileSize(string filename)
        {
            var entry = _zipFile.Entries.FirstOrDefault(x => x.FileName == filename);
            if (entry == null)
                throw new FileNotFoundException($"An entry named {filename} was not found in the APK.");
            return entry.UncompressedSize;
        }

        public void Save(string toFile = null)
        {
            CheckRO();
            foreach (var stream in _streamsToClose)
            {
                stream.Seek(0, SeekOrigin.Begin);
                
            }
            if (toFile != null)
                _zipFile.Save(toFile);
            else
            {
                //this is a workaround for something screwy with mono and/or the android filesystem where some race condition
                //  is causing the normal Save() to fail internally
                // I am NOT changing zip libraries AGAIN
                try
                {
                    _zipFile.Save();
                }
                catch (NullReferenceException nre)
                {
                    var st = nre.StackTrace;

                }
            }
            _streamsToClose.ForEach(x => x.Dispose());
            _streamsToClose.Clear();
        }

        private void CleanupTempFiles()
        {
            foreach (string tempfile in _tempFiles.Keys)
            {
                try
                {
                    File.Delete(_tempFiles[tempfile]);
                }
                catch (Exception ex)
                {
                    Log.LogErr($"Unable to delete temp file!", ex);
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_zipFile != null)
                    {
                        _zipFile.Dispose();
                        _zipFile = null;
                    }
                    CleanupTempFiles();
                    //Xamarin seems to be lazy about cleaning up open files.  This seems to fix it
                    GC.Collect();
                }
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

    }
}