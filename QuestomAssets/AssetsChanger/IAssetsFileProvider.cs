using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public interface IAssetsFileProvider : IDisposable
    {
        List<string> FindFiles(string pattern);
        Stream GetReadStream(string filename, bool bypassCache = false);
        byte[] Read(string filename);
        void Write(string filename, byte[] data, bool overwrite = true, bool compressData = true);
        void WriteFile(string sourceFilename, string targetFilename, bool overwrite = true, bool compressData = true);
        void DeleteFiles(string pattern);
        void Delete(string filename);
        bool FileExists(string filename);
        //void WriteFromStream(string filename, Stream data, bool overwrite = true, bool compressData = true);
        long GetFileSize(string filename);
        void Save();
    }
}
