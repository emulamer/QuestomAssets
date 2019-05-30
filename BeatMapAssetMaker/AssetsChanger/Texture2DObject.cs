using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BeatmapAssetMaker.AssetsChanger
{
    public sealed class Texture2DObject : AssetsObject
    {
        public Texture2DObject(ObjectInfo objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        public Texture2DObject(ObjectInfo objectInfo) : base(objectInfo)
        { }

        public Texture2DObject(AssetsMetadata metadata) : base(metadata, AssetsConstants.ClassID.Texture2DClassID)
        { }

        protected override void Parse(AssetsReader reader)
        {
            base.Parse(reader);
            Name = reader.ReadString();
            ForcedFallbackFormat = reader.ReadInt32();
            DownscaleFallback = reader.ReadBoolean();
            reader.AlignTo(4);
            Width = reader.ReadInt32();
            Height = reader.ReadInt32();
            CompleteImageSize = reader.ReadInt32();
            TextureFormat = reader.ReadInt32();
            MipCount = reader.ReadInt32();
            IsReadable = reader.ReadBoolean();
            StreamingMipmaps = reader.ReadBoolean();
            reader.AlignTo(4);
            StreamingMipmapsPriority = reader.ReadInt32();
            ImageCount = reader.ReadInt32();
            TextureDimension = reader.ReadInt32();
            TextureSettings = new GLTextureSettings(reader);
            LightmapFormat = reader.ReadInt32();
            ColorSpace = reader.ReadInt32();
            int imageDataSize = reader.ReadInt32();
            ImageData = reader.ReadBytes(imageDataSize);
            reader.AlignTo(4);
            StreamData = new StreamingInfo(reader);
        }
        public override void Write(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.Write(Name);
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


        public override byte[] Data
        {
            get
            {
                throw new InvalidOperationException("Data cannot be accessed from this class.");
            }
            set
            {
                throw new InvalidOperationException("Data cannot be accessed from this class.");
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
        public GLTextureSettings TextureSettings { get; set; }  
        public Int32 LightmapFormat { get; set; }
        public Int32 ColorSpace { get; set; }
        public byte[] ImageData { get; set; }
        public StreamingInfo StreamData { get; set; }

    }
}
