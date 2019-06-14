using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public sealed class Texture2DObject : AssetsObject, IHaveName
    {
        public Texture2DObject(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        //public Texture2DObject(IObjectInfo<AssetsObject> objectInfo) : base(objectInfo)
        //{ }

        public Texture2DObject(AssetsFile assetsFile) : base(assetsFile, AssetsConstants.ClassID.Texture2DClassID)
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
            TextureFormat = (TextureFormatType)reader.ReadInt32();
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
        protected override void WriteObject(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.Write(Name);
            writer.Write(ForcedFallbackFormat);
            writer.Write(DownscaleFallback);
            writer.AlignTo(4);
            writer.Write(Width);
            writer.Write(Height);
            writer.Write(CompleteImageSize);
            writer.Write((int)TextureFormat);
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
        public TextureFormatType TextureFormat { get; set; }
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

        public enum TextureFormatType : int
        {
            Alpha8 = 1,
            ARGB4444,
            RGB24,
            RGBA32,
            ARGB32,
            RGB565 = 7,
            R16 = 9,
            DXT1,
            DXT5 = 12,
            RGBA4444,
            BGRA32,
            RHalf,
            RGHalf,
            RGBAHalf,
            RFloat,
            RGFloat,
            RGBAFloat,
            YUY2,
            RGB9e5Float,
            BC4 = 26,
            BC5,
            BC6H = 24,
            BC7,
            DXT1Crunched = 28,
            DXT5Crunched,
            PVRTC_RGB2,
            PVRTC_RGBA2,
            PVRTC_RGB4,
            PVRTC_RGBA4,
            ETC_RGB4,
            ATC_RGB4,
            ATC_RGBA8,
            EAC_R = 41,
            EAC_R_SIGNED,
            EAC_RG,
            EAC_RG_SIGNED,
            ETC2_RGB,
            ETC2_RGBA1,
            ETC2_RGBA8,
            ASTC_RGB_4x4,
            ASTC_RGB_5x5,
            ASTC_RGB_6x6,
            ASTC_RGB_8x8,
            ASTC_RGB_10x10,
            ASTC_RGB_12x12,
            ASTC_RGBA_4x4,
            ASTC_RGBA_5x5,
            ASTC_RGBA_6x6,
            ASTC_RGBA_8x8,
            ASTC_RGBA_10x10,
            ASTC_RGBA_12x12,
            ETC_RGB4_3DS,
            ETC_RGBA8_3DS,
            RG16,
            R8,
            ETC_RGB4Crunched,
            ETC2_RGBA8Crunched,
        }

    }

    
}
