using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets
{
    //Maybe will end up needing this
    //class SplitFile
    //{
    //    public string FileName { get; set; }
    //    public long Offset { get; set; }
    //    public long Length { get; set; }
    //}

    //public class SplitStream : Stream
    //{
    //    private Apkifier _apk;
    //    private string _filename;
    //    private long _position;
    //    private List<SplitFile> _files = new List<SplitFile>();
    //    private long PosToCurrentStream
    //    {
    //        get
    //        {
    //            return _position - _currentFile.Offset;
    //        }
    //    }
    //    private SplitFile _currentFile;
    //    private Stream _currentStream;
    //    private long _length;
    //    public override long Length => _length;

    //    public SplitStream(Apkifier apk, string filename)
    //    {
    //        _apk = apk;
    //        _filename = filename;
    //        GetFiles(filename);
    //    }

    //    private void GetFiles(string filename)
    //    {
    //        string actualName = _apk.CorrectAssetFilename(_filename);


    //        if (actualName.ToLower().EndsWith("split0"))
    //        {
    //            long startsAt = 0;
    //            foreach (var split in _apk.FindFiles(actualName.Replace(".split0", ".split*"))
    //                .OrderBy(x => Convert.ToInt32(x.Split(new string[] { ".split" },
    //                StringSplitOptions.None).Last())))
    //            {
    //                var splitFile = new SplitFile()
    //                {
    //                    FileName = split,
    //                    Length = _apk.GetFileSize(split),
    //                    Offset = startsAt
    //                };
    //                startsAt += splitFile.Length;
    //            }
    //            _length = startsAt;
    //        }
    //        else
    //        {
    //            _files.Add(new SplitFile()
    //            {
    //                FileName = actualName,
    //                Offset = 0,
    //                Length = _apk.GetFileSize(actualName)
    //            });
    //        }
    //    }



    //    public override bool CanRead => true;

    //    public override bool CanSeek => true;

    //    public override bool CanWrite => !_apk.IsReadOnly;

    //    public override long Position => _currentStream.Position + _currentFile.Offset;

    //    public override void Flush()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    const int BFR_SIZE = 1024;
    //    public override int Read(byte[] buffer, int offset, int count)
    //    {
    //        byte[] bfr = new byte[BFR_SIZE];
    //        int readIndex = 0;
    //        while (readIndex +)
    //            if (count + PosToCurrentStream <= _currentFile.Length)
    //            {
    //                //single easy read
    //                _currentStream.Read(bfr, )
    //            }
    //            else
    //            {
    //                //hard split read
    //            }
    //        if ()
    //            throw new NotImplementedException();
    //    }

    //    public override long Seek(long offset, SeekOrigin origin)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override void SetLength(long value)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override void Write(byte[] buffer, int offset, int count)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
    //private Stream _baseStream;
    //public Stream BaseStream
    //{
    //    get
    //    {
    //        if (_baseStream.)
    //        }
    //}
}
