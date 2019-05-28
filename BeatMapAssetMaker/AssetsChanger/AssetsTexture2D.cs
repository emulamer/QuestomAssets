using System;
using System.Collections.Generic;
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
            base.Parse(reader);
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
