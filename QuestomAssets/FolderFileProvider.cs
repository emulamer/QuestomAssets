using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using QuestomAssets.AssetsChanger;
using static QuestomAssets.ApkAssetsFileProvider;

namespace QuestomAssets
{
    public class FolderFileProvider : IAssetsFileProvider
    {
        private string _rootFolder;
        private string _originalRoot;
        private bool _readonly;
        private List<Stream> _writeStreamsToClose = new List<Stream>();
        private List<Stream> _readStreamsToClose = new List<Stream>();
        public bool UseCombinedStream { get; private set; }

        public FolderFileProvider(string rootFolder, bool readOnly, bool useCombinedStream = true)
        {
            //if (!Directory.Exists(rootFolder))
            //    throw new FileNotFoundException($"Root folder '{rootFolder}' does not exist!");
            _originalRoot = AddTrailSlash(rootFolder);
            _rootFolder = _originalRoot;
            _readonly = readOnly;
            UseCombinedStream = useCombinedStream;
        }

        private string AddTrailSlash(string path)
        {
            if (path.Length > 0 && !path.EndsWith(Path.DirectorySeparatorChar.ToString()))
                return path + Path.DirectorySeparatorChar;
            else
                return path;
        }

        private void CheckRO()
        {
            if (_readonly)
                throw new Exception("Folder is open in read only mode, cannot make changes!");
        }

        public void Delete(string filename)
        {
            CheckRO();
            File.Delete(Path.Combine(_rootFolder, FwdToFS(filename)));
        }

        public void DeleteFiles(string pattern)
        {
            CheckRO();
            foreach (var filename in FindFiles(pattern))
            {
                string deleteFile = Path.Combine(_rootFolder, FwdToFS(filename));
                if (File.Exists(deleteFile))
                {
                    File.Delete(deleteFile);
                }
            }
        }

        private static bool FilePatternMatch(string filename, string pattern)
        {
            Regex mask = new Regex(pattern.Replace(".", "[.]").Replace("*", ".*").Replace("?", "."));
            return mask.IsMatch(filename);
        }


        public bool FileExists(string filename)
        {
            return File.Exists(Path.Combine(_rootFolder, FwdToFS(filename))) || Directory.Exists(Path.Combine(_rootFolder, FwdToFS(filename)));
        }

        public void MkDir(string path)
        {
            Directory.CreateDirectory(Path.Combine(_rootFolder, FwdToFS(path)));
        }

        public void RmRfDir(string path)
        {
            Directory.Delete(Path.Combine(_rootFolder, FwdToFS(path)), true);
        }

        public List<string> FindFiles(string pattern)
        {
            List<string> fnames = new List<string>();
            pattern = FSToFwd(pattern);

            foreach (var rawPath in Directory.EnumerateFiles(_rootFolder, "*", SearchOption.AllDirectories))
            {
                string filename = rawPath;
                if (!filename.StartsWith(_rootFolder))
                    throw new Exception("Not sure why the found folder doesn't start with root path, check it out.");
                filename = filename.Substring(_rootFolder.Length);
                //if (filename.StartsWith(Path.DirectorySeparatorChar.ToString()))
                //    filename = filename.Substring(1);
                filename = FSToFwd(filename);
                if (FilePatternMatch(filename, pattern))
                {
                    fnames.Add(filename);
                }
            }
            return fnames;
        }

        public long GetFileSize(string filename)
        {
            return new FileInfo(Path.Combine(_rootFolder, FwdToFS(filename))).Length;
        }

        public Stream GetReadStream(string filename, bool bypassCache = false)
        {
            var readStream = File.OpenRead(Path.Combine(_rootFolder, FwdToFS(filename)));
            _readStreamsToClose.Add(readStream);
            return readStream;
        }

        public byte[] Read(string filename)
        {
            return File.ReadAllBytes(Path.Combine(_rootFolder, FwdToFS(filename)));
        }

        public void Save(string toFile = null)
        {
            //save is instant!  but we'll clean up the streams we made
            CloseWriteStreams();
        }

        private void CloseWriteStreams()
        {
            foreach (Stream s in _writeStreamsToClose.ToList())
            {
                try
                {
                    s.Close();
                    s.Dispose();
                }
                catch
                { }
                _writeStreamsToClose.Remove(s);
            }
        }

        private void CloseReadStreams()
        {
            foreach (Stream s in _readStreamsToClose.ToList())
            {
                try
                {
                    s.Close();
                    s.Dispose();
                }
                catch
                { }
                _readStreamsToClose.Remove(s);
            }
        }

        public void Write(string filename, byte[] data, bool overwrite = true, bool compressData = true)
        {
            CheckRO();
            if (!overwrite && FileExists(filename))
                throw new Exception("File already exists and overwrite is set to false.");
            else if (FileExists(filename))
                Delete(filename);

            using (var fs = File.Open(Path.Combine(_rootFolder, FwdToFS(filename)), FileMode.Create, FileAccess.ReadWrite))
                fs.Write(data, 0, data.Length);
        }

        public void WriteFile(string sourceFilename, string targetFilename, bool overwrite = true, bool compressData = true)
        {
            CheckRO();
            if (!overwrite && FileExists(Path.Combine(_rootFolder, FwdToFS(targetFilename))))
                throw new Exception("Target file already exists and overwrite is set to false.");
            else if (FileExists(targetFilename))
                Delete(targetFilename);

            File.Copy(sourceFilename, Path.Combine(_rootFolder, FwdToFS(targetFilename)), overwrite);
        }
        public Stream GetWriteStream(string filename)
        {
            CheckRO();
            if (FileExists(filename))
                Delete(filename);

            var stream = File.Open(Path.Combine(_rootFolder, FwdToFS(filename)), FileMode.Create, FileAccess.ReadWrite);
            _writeStreamsToClose.Add(stream);
            return stream;
        }

        private string FwdToFS(string path)
        {
            return path.Replace('/', Path.DirectorySeparatorChar);
        }
        private string FSToFwd(string path)
        {
            return path.Replace(Path.DirectorySeparatorChar, '/');
        }
        public string BasePath
        { get => _rootFolder; }



        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    CloseReadStreams();
                    CloseWriteStreams();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}