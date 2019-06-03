using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class AlignedStream
{
    private Stream _Stream;
    private int _ctr;
    public AlignedStream(Stream stream)
    {
        _Stream = stream;
    }

    public void Write(string str, bool align=true)
    {
        if (string.IsNullOrEmpty(str))// == null)
        {
            _Stream.Write(new byte[] { 0, 0, 0, 0 }, 0, 4);
            Ct(4);
        }
        else
        {
            byte[] strBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(str);
            _Stream.Write(BitConverter.GetBytes(strBytes.Length), 0, 4);
            _Stream.Write(strBytes, 0, strBytes.Length);
            //_Stream.WriteByte(0);
            Ct(4 + strBytes.Length );
        }
        if (align)
        {
            AlignTo(4);
        }
    }
    public void AlignTo(byte numBytes)
    {
        int num = numBytes - (_ctr % numBytes);
        if (num > 0 && num < numBytes)
        {
            byte[] b = new byte[num];
            _Stream.Write(b, 0, b.Length);
            Ct(num);
        }
    }
    public void Write(UPtr p)
    {
        this.Write(p.FileID);
        this.Write(p.PathID);
    }
    public void Write(int i)
    {
        _Stream.Write(BitConverter.GetBytes(i), 0, 4);
        Ct(4);
    }
    public void Write(float f)
    {
        _Stream.Write(BitConverter.GetBytes(f), 0, 4);
        Ct(4);
    }
    public void Write(Int64 i)
    {
        _Stream.Write(BitConverter.GetBytes(i), 0, 8);
        Ct(8);
    }
    public void Write(byte[] buffer, int offset, int count)
    {
        _Stream.Write(buffer, offset, count);
        Ct(count);
    }
    public void WriteByte(byte b)
    {
        _Stream.WriteByte(b);
        Ct(1);
    }
    public void Write(byte[] data)
    {
        this.Write(data.Length);
        this.Write(data, 0, data.Length);
    }

    private void Ct(int num)
    {
        _ctr += num;
    }

}