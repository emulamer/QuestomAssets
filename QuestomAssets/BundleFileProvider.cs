using QuestomAssets.AssetsChanger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace QuestomAssets
{
    public class BundleFileProvider : IAssetsFileProvider
    {
        private FileStream _fileStream;
        private BundleFile _bundleFile;

        public BundleFileProvider(string bundleFile, bool readOnly = true)
        {
            _fileStream = File.Open(bundleFile, FileMode.Open, readOnly ? FileAccess.Read : FileAccess.ReadWrite);
            _bundleFile = new BundleFile(_fileStream);
        }

        private static bool FilePatternMatch(string filename, string pattern)
        {
            Regex mask = new Regex(pattern.Replace(".", "[.]").Replace("*", ".*").Replace("?", "."));
            return mask.IsMatch(filename);
        }

        public bool FileExists(string filename)
        {
            return _bundleFile.Entries.Any(x => x.Filename.ToLower() == filename.ToLower());
        }

        public List<string> FindFiles(string pattern)
        {
            List<DirectoryEntry> found = new List<DirectoryEntry>();
            foreach (var entry in _bundleFile.Entries)
            {
                if (FilePatternMatch(entry.Filename.ToLower(), pattern.ToLower()))
                    found.Add(entry);
            }
            return found.Select(x => x.Filename).ToList();
        }

        private DirectoryEntry GetEntry(string filename)
        {
            var entry = _bundleFile.Entries.FirstOrDefault(x => x.Filename.ToLower() == filename.ToLower());
            if (entry == null)
                throw new FileNotFoundException($"{filename} was not found in the bundle.");
            return entry;
        }

        public long GetFileSize(string filename)
        {
            return GetEntry(filename).Size;
        }

        public Stream GetReadStream(string filename, bool bypassCache = false)
        {
            return new MemoryStream(GetEntry(filename).Data);
        }

        public byte[] Read(string filename)
        {
            return GetEntry(filename).Data;
        }

        #region "Not implemented things"

        public void Delete(string filename)
        {
            throw new NotImplementedException();
        }

        public void DeleteFiles(string pattern)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Write(string filename, byte[] data, bool overwrite = true, bool compressData = true)
        {
            throw new NotImplementedException();
        }

        public void WriteFile(string sourceFilename, string targetFilename, bool overwrite = true, bool compressData = true)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_fileStream != null)
                    {
                        _fileStream.Dispose();
                        _fileStream = null;
                    }
                    _bundleFile = null;
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
