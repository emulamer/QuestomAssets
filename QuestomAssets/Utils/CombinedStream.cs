using QuestomAssets.AssetsChanger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuestomAssets.Utils
{
    public class CombinedStream : Stream
    {
        private int _totalLength;
        private IFileProvider _fileProvider;
        private class SplitFile
        {
            public SplitFile(Stream s)
            {
                Stream = s;
            }
            public string Filename { get; set; }
            private Stream Stream { get; set; }
            public int StartOffset { get; set; }
            public int Length
            {
                get
                {
                    return (int)Stream.Length;
                }
            }

            public bool CanSeekAbsolute(int position)
            {
                int relativePos = position - StartOffset;
                //check if the absolute position is within this file
                if (relativePos >= Stream.Length)
                    return false;

                return true;
            }

            public void SeekAbsolute(int position)
            {
                int relativePos = position - StartOffset;
                //check if the absolute position is within this file
                if (relativePos >= Stream.Length)
                    throw new ArgumentOutOfRangeException();

                Stream.Seek(relativePos, SeekOrigin.Begin);
            }

            public int Read(byte[] buffer, int offset, int length)
            {
                int readLen = length;
                if (readLen > (Stream.Length - Stream.Position))
                {
                    readLen = (int)(Stream.Length - Stream.Position);
                }
                return Stream.Read(buffer, offset, readLen);
            }
            public void DisposeStream()
            {
                Stream.Close();
                Stream.Dispose();
                Stream = null;
            }
        }
        private List<SplitFile> _files = new List<SplitFile>();

        public CombinedStream(List<string> orderedSplitFiles, IFileProvider provider)
        {
            _fileProvider = provider;
            InitFromFiles(orderedSplitFiles);
        }

        private void InitFromFiles(List<string> orderedSplitFiles)
        {
            _totalLength = 0;
            for (int i = 0; i < orderedSplitFiles.Count; i++)
            {
                var sf = new SplitFile(_fileProvider.GetReadStream(orderedSplitFiles[i]))
                {
                    Filename = orderedSplitFiles[i],
                    StartOffset = (int)_totalLength
                };
                _totalLength += sf.Length;
                _files.Add(sf);
            }
        }

        // gets the split file for the current stream's position
        private SplitFile FileAtPosition
        {
            get
            {
                for (int i = 0; i < _files.Count; i++)
                {
                    if (_files[i].CanSeekAbsolute((int)Position))
                        return _files[i];
                }
                return null;
            }
        }

        private long _absolutePosition;

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length => _totalLength;

        public override long Position
        {
            get => _absolutePosition;
            set
            {
                if (_absolutePosition > _totalLength)
                    throw new ArgumentOutOfRangeException();

                _absolutePosition = value;
                var file = FileAtPosition;
                if (file == null && _absolutePosition != _totalLength)
                    throw new Exception("Tried to seek past end of file!");
                if (file != null)
                    file.SeekAbsolute((int)_absolutePosition);
            }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalRead = 0;

            while (totalRead < count)
            {
                int curRead = FileAtPosition.Read(buffer, totalRead, count - totalRead);
                Position += curRead;
                totalRead += curRead;
            }
            return totalRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            Stream s;
            var newPos = 0;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    newPos = (int)offset;
                    break;
                case SeekOrigin.Current:
                    newPos = (int)_absolutePosition + (int)offset;
                    break;
                case SeekOrigin.End:
                    newPos = _totalLength + (int)offset;
                    break;
            }

            if (newPos < 0 || newPos > _totalLength - 1)
                throw new ArgumentOutOfRangeException();

            Position = newPos;
            return Position;
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        private bool _isClosed = false;
        public override void Close()
        {
            if (_isClosed)
                return;

            _isClosed = true;
            foreach (var file in _files.ToList())
            {
                file.DisposeStream();
                _files.Remove(file);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected override void Dispose(bool disposing)
        {            
            if (!disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        Close();
                    }
                    catch { }
                }

                disposedValue = true;
            }
            base.Dispose(disposing);
        }

        #endregion


    }
}
