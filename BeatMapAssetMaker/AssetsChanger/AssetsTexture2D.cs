using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BeatmapAssetMaker.AssetsChanger
{
    public class AssetsTexture2D : AssetsObject
    {
        public AssetsTexture2D(AssetsObjectInfo objectInfo, AssetsReader reader) : base(objectInfo, reader)
        { }

        public AssetsTexture2D(AssetsObjectInfo objectInfo) : base(objectInfo)
        { }


        protected override void Parse(AssetsReader reader)
        {
            Name = reader.ReadString();
            reader.AlignToObjectData(4);
            ForcedFallbackFormat = reader.ReadInt32();
            DownscaleFallback = reader.ReadBoolean();
            reader.AlignToObjectData(4);
            Width = reader.ReadInt32();
            Height = reader.ReadInt32();
            CompleteImageSize = reader.ReadInt32();
            TextureFormat = reader.ReadInt32();
            MipCount = reader.ReadInt32();
            IsReadable = reader.ReadBoolean();
            StreamingMipmaps = reader.ReadBoolean();
            reader.AlignToObjectData(4);
            StreamingMipmapsPriority = reader.ReadInt32();
            ImageCount = reader.ReadInt32();
            TextureDimension = reader.ReadInt32();
            TextureSettings = new AssetsGLTextureSettings(reader);
            LightmapFormat = reader.ReadInt32();
            ColorSpace = reader.ReadInt32();
            int imageDataSize = reader.ReadInt32();
            ImageData = reader.ReadBytes(imageDataSize);
            reader.AlignToObjectData(4);
            StreamData = new AssetsStreamingInfo(reader);
        }

        private byte[] Serialize()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (AssetsWriter writer = new AssetsWriter(ms))
                {
                    writer.Write(Name);
                    writer.AlignTo(4);
                    writer.Write(ForcedFallbackFormat);
                    writer.Write(DownscaleFallback);
                    writer.AlignTo(4);
                    writer.Write(Width);
                    writer.Write(Height);
                    writer.Write(CompleteImageSize);
                    writer.Write(TextureFormat);
                    writer.Write(MipCount);
                    writer.Write(IsReadable);
                    writer.Write(StreamingMipmaps);
                    writer.AlignTo(4);
                    writer.Write(StreamingMipmapsPriority);
                    writer.Write(ImageCount);
                    writer.Write(TextureDimension);
                    TextureSettings.Write(writer);
                    writer.Write(LightmapFormat);
                    writer.Write(ColorSpace);
                    writer.Write(ImageData.Length);
                    writer.Write(ImageData);
                    writer.AlignTo(4);
                    StreamData.Write(writer);
                }
                return ms.ToArray();
            }
        }

        public override byte[] Data
        {
            get
            {
                return Serialize();
            }
            set
            {
                using (MemoryStream ms = new MemoryStream(value))
                {
                    using (AssetsReader reader = new AssetsReader(ms))
                    {
                        Parse(reader);
                    }
                }
            }
        }


        public string Name { get; set; }
        public Int32 ForcedFallbackFormat { get; set; }
        public bool DownscaleFallback { get; set; }
        public Int32 Width { get; set; }
        public Int32 Height { get; set; }
        public Int32 CompleteImageSize { get; set; }
        public Int32 TextureFormat { get; set; }
        public Int32 MipCount { get; set; }
        public bool IsReadable { get; set; }
        public bool StreamingMipmaps { get; set; }
        public Int32 StreamingMipmapsPriority { get; set; }
        public Int32 ImageCount { get; set; }
        public Int32 TextureDimension { get; set; }
        public AssetsGLTextureSettings TextureSettings { get; set; }  
        public Int32 LightmapFormat { get; set; }
        public Int32 ColorSpace { get; set; }
        public byte[] ImageData { get; set; }
        public AssetsStreamingInfo StreamData { get; set; }

    }
}
