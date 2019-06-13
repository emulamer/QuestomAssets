using System;
using System.Collections.Generic;
using System.IO;
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
        
        public FolderFileProvider(string rootFolder, bool readOnly)
        {
            if (!Directory.Exists(rootFolder))
                throw new FileNotFoundException($"Root folder '{rootFolder}' does not exist!");
            _originalRoot = rootFolder;
            _rootFolder = rootFolder;
            _readonly = readOnly;
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
            return File.Exists(Path.Combine(_rootFolder, FwdToFS(filename)));
        }

        public List<string> FindFiles(string pattern)
        {
            List<string> fnames = new List<string>();

            foreach (var rawPath in Directory.EnumerateFiles(_rootFolder, "*", SearchOption.AllDirectories))
            {
                string filename = rawPath.Substring(_rootFolder.Length);
                if (filename.StartsWith(Path.DirectorySeparatorChar.ToString()))
                    filename = filename.Substring(1);
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
            return File.OpenRead(Path.Combine(_rootFolder, FwdToFS(filename)));
        }

        public byte[] Read(string filename)
        {
            return File.ReadAllBytes(Path.Combine(_rootFolder, FwdToFS(filename)));
        }

        public void Save()
        {
            //save is instant!
        }

        public void Write(string filename, byte[] data, bool overwrite = true, bool compressData = true)
        {
            CheckRO();
            if (!overwrite && FileExists(filename))
                throw new Exception("File already exists and overwrite is set to false.");

            using (var fs = File.Open(Path.Combine(_rootFolder, FwdToFS(filename)), FileMode.Create, FileAccess.ReadWrite))
                fs.Write(data, 0, data.Length);
        }

        public void WriteFile(string sourceFilename, string targetFilename, bool overwrite = true, bool compressData = true)
        {
            CheckRO();
            if (!overwrite && FileExists(targetFilename))
                throw new Exception("Target file already exists and overwrite is set to false.");

            File.Copy(sourceFilename, Path.Combine(_rootFolder, FwdToFS(targetFilename)));            
        }

        private string FwdToFS(string path)
        {
            return path.Replace('/', Path.DirectorySeparatorChar);
        }
        private string FSToFwd(string path)
        {
            return path.Replace(Path.DirectorySeparatorChar, '/');
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
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
