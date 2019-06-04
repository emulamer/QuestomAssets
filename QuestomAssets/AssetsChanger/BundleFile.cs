using LZ4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LZMA = SevenZip.Compression.LZMA;

namespace QuestomAssets.AssetsChanger
{
    public class BundleFile
    {
        private class ProgressDummy : SevenZip.ICodeProgress
        {
            public void SetProgress(long inSize, long outSize)
            {
            }
            public static ProgressDummy Instance { get; } = new ProgressDummy();
        }
        public string PlayerVersion { get; set; }

        public string EngineVersion { get; set; }

        public Int64 BundleSize { get; set; }

        public List<DirectoryEntry> Entries { get; } = new List<DirectoryEntry>();
        public List<BlockInfo> BlockInfos { get; } = new List<BlockInfo>();

        public BundleFile(Stream stream)
        {
            using (var reader = new AssetsReader(stream, false))
            {
                Parse(reader);
            }
        }

        private UnityFSCompressionMode CompressionMode(UInt32 flags)
        {
            return (UnityFSCompressionMode)(flags & 0x3F);
        }

        private bool HasDirectoryInfo(UInt32 flags)
        {
            return (flags & 0x40) > 0;
        }

        private bool IsDirectoryAtEnd(UInt32 flags)
        {
            return (flags & 0x80) > 0;
        }

        private void Parse(AssetsReader reader)
        {
            //basic header stuff
            var signature = reader.ReadCStr();
            if (signature != "UnityFS")
                throw new NotSupportedException("Stream is not UnityFS");
            var fileVersion = reader.ReadBEInt32();
            if (fileVersion != 6)
                throw new NotSupportedException("File version is not supported");
            PlayerVersion = reader.ReadCStr();
            EngineVersion = reader.ReadCStr();
            BundleSize = reader.ReadBEInt64();

            //header info
            var compressedSize = reader.ReadBEInt32();
            var decompressedSize = reader.ReadBEInt32();
            var flags = reader.ReadBEUInt32();

            byte[] infoBytes;
            if (IsDirectoryAtEnd(flags))
            {
                var start = (int)reader.BaseStream.Position;
                reader.Seek((int)reader.BaseStream.Length - compressedSize);
                infoBytes = reader.ReadBytes(compressedSize);
                reader.Seek(start);
            }
            else
            {
                infoBytes = reader.ReadBytes(compressedSize);
            }

            Stream blockInfoStream = null;
            try
            {
                switch (CompressionMode(flags))
                {
                    case UnityFSCompressionMode.LZ4:
                    case UnityFSCompressionMode.LZ4HC:
                        blockInfoStream = new MemoryStream(LZ4.LZ4Codec.Decode(infoBytes, 0, infoBytes.Length, decompressedSize));
                        break;
                    case UnityFSCompressionMode.NoCompression:
                        blockInfoStream = new MemoryStream(infoBytes);
                        break;
                    case UnityFSCompressionMode.LZMA:
                        blockInfoStream = new MemoryStream(LZMADecode(infoBytes, decompressedSize));
                        break;
                }

                using (AssetsReader infoReader = new AssetsReader(blockInfoStream, false))
                    ParseDirectory(infoReader);
            }
            finally
            {
                if (blockInfoStream != null)
                {
                    blockInfoStream.Dispose();
                }
            }
            MemoryStream outputStream = new MemoryStream();
            foreach (var blockInfo in BlockInfos)
            {
                byte[] blockData = null;
                switch (blockInfo.CompressionMode)
                {
                    case UnityFSCompressionMode.LZ4:
                    case UnityFSCompressionMode.LZ4HC:
                        blockData = LZ4.LZ4Codec.Decode(reader.ReadBytes((int)blockInfo.CompressedSize), 0, (int)blockInfo.CompressedSize, (int)blockInfo.UncompressedSize);
                        break;
                    case UnityFSCompressionMode.NoCompression:
                        blockData = reader.ReadBytes((int)blockInfo.UncompressedSize);
                        break;
                    case UnityFSCompressionMode.LZMA:
                        blockData = LZMADecode(reader.BaseStream,(int)blockInfo.CompressedSize, (int)blockInfo.UncompressedSize);
                        break;
                }

                outputStream.Write(blockData, 0, blockData.Length);
            }
            using (outputStream)
            {
                foreach (var entry in Entries)
                {
                    outputStream.Seek(entry.Offset, SeekOrigin.Begin);
                    entry.Data = outputStream.ReadBytes((int)entry.Size);
                }

            }
        }

        private static byte[] LZMADecode(byte[] inputData, int uncompressedSize)
        {
            if (inputData.Length < 5)
                throw new ArgumentException("Input data is too short.");
            var decoder = new LZMA.Decoder();
            var properties = new byte[5];
            Array.Copy(inputData, 0,properties,0, 5);
            decoder.SetDecoderProperties(properties);
            using (var outLZMA = new MemoryStream())
                using (var inLZMA = new MemoryStream(inputData, 13, inputData.Length - 13))
                {
                    decoder.Code(inLZMA, outLZMA, inputData.Length-13, uncompressedSize, null);
                return outLZMA.ToArray();
                }
        }

        private static byte[] LZMADecode(Stream inStream, int compressedSize, int uncompressedSize)
        {
        
            var decoder = new LZMA.Decoder();
            var properties = new byte[5];
            if (inStream.Read(properties, 0, 5) != 5)
                throw new ArgumentException("Input data is too short.");
            decoder.SetDecoderProperties(properties);
            using (var outLZMA = new MemoryStream())
            {
                decoder.Code(inStream, outLZMA, compressedSize, uncompressedSize, null);
                return outLZMA.ToArray();
            }
        }

        private void ParseDirectory(AssetsReader reader)
        {
            //unknown?
            reader.ReadBytes(16);
            int numBlocks = reader.ReadBEInt32();

            for (int i = 0; i < numBlocks; i++)
                BlockInfos.Add(new BlockInfo(reader));

            int numEntries = reader.ReadBEInt32();
            for (int i = 0; i < numEntries; i++)
            {
                Entries.Add(new DirectoryEntry(reader));
            }
        }
    }

    public enum UnityFSCompressionMode : UInt32
    {
        NoCompression = 0,
        LZMA = 1,
        LZ4 = 2,
        LZ4HC = 3
    }

    public class DirectoryEntry
    {
        public Int64 Offset { get; set; }
        public Int64 Size { get; set; }
        private UInt32 _flags;
        public string Filename { get; set; }

        public byte[] Data { get; set; }

        public DirectoryEntry(AssetsReader reader)
        {
            Parse(reader);
        }

        private void Parse(AssetsReader reader)
        {
            Offset = reader.ReadBEInt64();
            Size = reader.ReadBEInt64();
            _flags = reader.ReadBEUInt32();
            Filename = reader.ReadCStr();
        }
    }
    public class BlockInfo
    {
        public UInt32 CompressedSize { get; set; }
        public UInt32 UncompressedSize { get; set; }
        private UInt16 _flags;


        public UnityFSCompressionMode CompressionMode
        {
            get
            {
                return (UnityFSCompressionMode)(_flags & 0x3F);
            }
            //I think this is right but it isn't needed now
            //set
            //{
            //    
            //    _flags = (_flags & ~(UInt32)0x3F) | ((UInt32)value & 0x3F);
            //}
        }

        public BlockInfo(AssetsReader reader)
        {
            Parse(reader);
        }

        private void Parse(AssetsReader reader)
        {
            UncompressedSize = reader.ReadBEUInt32();
            CompressedSize = reader.ReadBEUInt32();
            
            _flags = reader.ReadBEUInt16();
        }
    }


}