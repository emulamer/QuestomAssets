using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using QuestomAssets.AssetsChanger;
using QuestomAssets.Utils;
using QuestomAssets;

namespace BeatmapAssetMaker
{

    public class ImageUtilsWin : IImageUtils
    {
        public void AssignImageToTexture(byte[] imageData, Texture2DObject targetTexture, int targetWidth, int targetHeight, int targetMips = Int32.MaxValue, TextureConversionFormat format = TextureConversionFormat.Auto)
        {
            //right now "Auto" is always RGB24.  Probably will automatically decide based on image size, but doing ETC2 sucks at the moment
            Bitmap sourceImage = null;
            using (var ms = new MemoryStream(imageData))
            {
                sourceImage = new Bitmap(ms);
            }
            int mips;
            byte[] textureBytes;
            if (format == TextureConversionFormat.ETC2_RGB)
            {
                throw new NotImplementedException();
                //textureBytes = ConvertToETC2AndMipmap(sourceImage, targetWidth, targetHeight, targetMips, out mips);
                targetTexture.TextureFormat = Texture2DObject.TextureFormatType.ETC2_RGB;
            }
            else //always fall back to RGB24
            {
                textureBytes = ConvertToRGBAndMipmap(sourceImage, targetWidth, targetHeight, targetMips, out mips);
                targetTexture.TextureFormat = Texture2DObject.TextureFormatType.RGB24;
            }

            targetTexture.ForcedFallbackFormat = 4;
            targetTexture.DownscaleFallback = false;
            targetTexture.Width = targetWidth;
            targetTexture.Height = targetHeight;
            targetTexture.CompleteImageSize = textureBytes.Length;
            targetTexture.MipCount = mips;
            targetTexture.IsReadable = false;
            targetTexture.StreamingMipmaps = false;
            targetTexture.StreamingMipmapsPriority = 0;
            targetTexture.ImageCount = 1;
            targetTexture.TextureDimension = 2;
            targetTexture.TextureSettings = new GLTextureSettings()
            {
                FilterMode = 2,
                Aniso = 1,
                MipBias = -1,
                WrapU = 1,
                WrapV = 1,
                WrapW = 0
            };
            targetTexture.LightmapFormat = 6;
            targetTexture.ColorSpace = 1;
            targetTexture.ImageData = textureBytes;
            targetTexture.StreamData = new StreamingInfo()
            {
                offset = 0,
                size = 0,
                path = ""
            };
        }

        public byte[] TextureToPngBytes(Texture2DObject texture)
        {
            //does unity store mips of compressed textures individually, or does it compress the entire thing?
            switch (texture.TextureFormat)
            {
                case Texture2DObject.TextureFormatType.RGB24:
                    using (var ms = new MemoryStream())
                    {
                        ConvertFromRGB24ToBitmap(texture.ImageData, texture.Width, texture.Height, RotateFlipType.RotateNoneFlipXY).Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        return ms.ToArray();
                    }

                //not really sure if it can do all these or not
                case Texture2DObject.TextureFormatType.ETC2_RGB:
                case Texture2DObject.TextureFormatType.ETC_RGB4:
                case Texture2DObject.TextureFormatType.ETC2_RGBA8:
                case Texture2DObject.TextureFormatType.ETC2_RGBA1:
                    return null;// return ConvertETC2ToBitmap(texture.ImageData, texture.Width, texture.Height);
                default:
                    throw new NotImplementedException($"Texture type {texture.ToString()} isn't currently supported.");
            }
        }

        private static byte[] ConvertToRGBAndMipmap(Bitmap image, int targetWidth, int targetHeight, int maxMips, out int actualMips)
        {
            using (MemoryStream msMips = new MemoryStream())
            {
                int currentWidth = targetWidth;
                int currentHeight = targetHeight;
                int mipCount = 0;
                while (currentWidth >= 1 && currentHeight >= 1 && mipCount < maxMips)
                {
                    byte[] mipData = ConvertToRGBAndSize(image, currentWidth, currentHeight, RotateFlipType.Rotate180FlipX);
                    msMips.Write(mipData, 0, mipData.Length);
                    currentWidth /= 2;
                    currentHeight /= 2;
                    mipCount++;
                }
                actualMips = mipCount;
                return msMips.ToArray();
            }
        }

        private static byte[] FlipRB(byte[] rgb, int limit = Int32.MaxValue)
        {
            for (int i = 0; (i + 3) < rgb.Length && i < limit; i += 3)
            {
                byte temp = rgb[i];
                rgb[i] = rgb[i + 2];
                rgb[i + 2] = temp;
            }
            return rgb;
        }

        private static Bitmap ConvertFromRGB24ToBitmap(byte[] rgbData, int width, int height, RotateFlipType flipType)
        {
            Bitmap image = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            int imageBytesSize = width * height * 3;
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            System.Drawing.Imaging.BitmapData bitmapData =
                image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                image.PixelFormat);
            IntPtr bmpPtr = bitmapData.Scan0;
            FlipRB(rgbData, imageBytesSize);
            System.Runtime.InteropServices.Marshal.Copy(rgbData, 0, bmpPtr, imageBytesSize);
            image.UnlockBits(bitmapData);
            image.RotateFlip(flipType);
            return image;
        }

        private static byte[] ConvertToRGBAndSize(Bitmap image, int width, int height, RotateFlipType flipType)
        {

            Bitmap newImage = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(newImage);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            g.DrawImage(image, new Rectangle(0, 0, width, height));
            newImage.RotateFlip(flipType);
            image = newImage;

            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            System.Drawing.Imaging.BitmapData bitmapData =
                image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                image.PixelFormat);
            IntPtr bmpPtr = bitmapData.Scan0;
            byte[] imageBytes = new byte[Math.Abs(bitmapData.Stride) * image.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpPtr, imageBytes, 0, imageBytes.Length);
            image.UnlockBits(bitmapData);

            return FlipRB(imageBytes);
        }

    }
}
