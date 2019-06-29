using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public interface IFileProvider : IDisposable
    {
        List<string> FindFiles(string pattern);
        Stream GetReadStream(string filename, bool bypassCache = false);
        byte[] Read(string filename);
        void Write(string filename, byte[] data, bool overwrite = true, bool compressData = true);
        void WriteFile(string sourceFilename, string targetFilename, bool overwrite = true, bool compressData = true);
        void DeleteFiles(string pattern);
        void Delete(string filename);
        bool FileExists(string filename);
        void MkDir(string path);
        void RmRfDir(string path);
        //void WriteFromStream(string filename, Stream data, bool overwrite = true, bool compressData = true);
        long GetFileSize(string filename);
        void Save(string toFile = null);
        Stream GetWriteStream(string filename);
        bool UseCombinedStream { get; }

        /// <summary>
        /// Gets the name of the source folder or file.  e.g. in the case of a folder provider /sdcard/somedata/morethings/songdata, it would be "songdata", in the case of an apk file provider opening somesong.zip, it would be "somesong.zip"
        /// </summary>
        string SourceName { get; }

        bool DirectoryExists(string path);

    }
}