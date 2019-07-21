using QuestomAssets;
using QuestomAssets.AssetsChanger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using adb = SharpAdbClient;

namespace Assplorer
{
    public class AdbFileProvider : IFileProvider, IDisposable
    {

        public IEnumerable<string> GetDevices()
        {
            var devices = adb.AdbClient.Instance.GetDevices();

            foreach (var device in devices)
            {
                yield return device.Name;
            }
        }

        public void SetDevice(string deviceName)
        {
            var devices = adb.AdbClient.Instance.GetDevices();

            foreach (var device in devices)
            {
                if (device.Name == deviceName)
                {
                    _device = device;
                    break;
                }
            }
            throw new Exception("Device was not found.");
        }

        public void Connect()
        {
            _service = new adb.SyncService(new adb.AdbSocket(new IPEndPoint(IPAddress.Loopback, adb.AdbClient.AdbServerPort)), Device);
        }

        public void Disconnect()
        {
            if (_service != null)
            {
                _service.Dispose();
                _service = null;
            }
        }
        private adb.SyncService _service;
        private SharpAdbClient.AdbServer _server;
        private adb.DeviceData _device;
        private adb.DeviceData Device
        {
            get
            {
                if (_device == null)
                {
                    var devices = adb.AdbClient.Instance.GetDevices();
                    if (devices.Count > 1)
                    {
                        throw new Exception("More than one device is connected, SetDevice must be called!");
                    }
                    if (devices.Count < 1)
                    {
                        throw new Exception("No devices are connected.");
                    }
                    _device = devices.First();
                }
                return _device;
            }
        }

        private string _rootPath;
        public AdbFileProvider(string rootPath)
        {
            try
            {
                _server = new SharpAdbClient.AdbServer();
                _server.StartServer(@"adb.exe", restartServerIfNewer: false);
                _rootPath = rootPath;
            }
            catch (Exception ex)
            {
                Log.LogErr("Exception initializing AdbFileProvider", ex);
                throw;
            }
        }

        private static bool FilePatternMatch(string filename, string pattern)
        {
            Regex mask = new Regex(pattern.Replace(".", "[.]").Replace("*", ".*").Replace("?", "."));
            return mask.IsMatch(filename);
        }

        public List<string> FindFiles(string pattern)
        {
            List<string> files = new List<string>();

            foreach (var f in _service.GetDirectoryListing(_rootPath))
            {
                if (FilePatternMatch(f.Path.Substring(_rootPath.Length), pattern))
                    files.Add(f.Path);
            }
            
            return files;
        }

        public Stream GetReadStream(string filename, bool bypassCache = false)
        {
            MemoryStream ms = new MemoryStream();
            _service.Pull(filename, ms, null, System.Threading.CancellationToken.None);
            return ms;
        }

        public byte[] Read(string filename)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                _service.Pull(filename, ms, null, System.Threading.CancellationToken.None);
                return ms.ToArray();
            }
        }

        public void Write(string filename, byte[] data, bool overwrite = true, bool compressData = true)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                _service.Push(ms, filename, 444, DateTime.Now, null, System.Threading.CancellationToken.None);
            }
        }

        public void WriteFile(string sourceFilename, string targetFilename, bool overwrite = true, bool compressData = true)
        {
            throw new NotImplementedException();
        }

        public void DeleteFiles(string pattern)
        {
            throw new NotImplementedException();
        }

        public void Delete(string filename)
        {
            throw new NotImplementedException();
        }

        public bool FileExists(string filename)
        {
            throw new NotImplementedException();
        }

        public void MkDir(string path, bool recursive = false)
        {
            var receiver = new adb.ConsoleOutputReceiver();
            if (recursive)
            {
                var dir = "/";
                
                foreach (string dirname in path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    dir = dir.CombineFwdSlash(dirname);
                    adb.AdbClient.Instance.ExecuteRemoteCommand($"mkdir \"{dir}\"", _device, receiver);
                }
            }
            
        }

        public void RmRfDir(string path)
        {
            throw new NotImplementedException();
        }

        public long GetFileSize(string filename)
        {
            throw new NotImplementedException();
        }

        public void Save(string toFile = null)
        {
            throw new NotImplementedException();
        }

        public Stream GetWriteStream(string filename)
        {
            throw new NotImplementedException();
        }

        public bool DirectoryExists(string path)
        {
            throw new NotImplementedException();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        public bool UseCombinedStream => throw new NotImplementedException();

        public string SourceName => throw new NotImplementedException();

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AdbFileProvider()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
