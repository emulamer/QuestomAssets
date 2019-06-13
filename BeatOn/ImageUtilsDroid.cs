using System;
using QuestomAssets.AssetsChanger;
using QuestomAssets.Utils;

namespace BeatOn
{
    public class ImageUtilsDroid : QuestomAssets.Utils.IImageUtils
    {
        public void AssignImageToTexture(byte[] imageData, Texture2DObject targetTexture, int targetWidth, int targetHeight, int targetMips = int.MaxValue, TextureConversionFormat format = TextureConversionFormat.Auto)
        {
            
        }

        public byte[] TextureToBitmap(Texture2DObject texture)
        {
            return new byte[1];
        }
    }
}
