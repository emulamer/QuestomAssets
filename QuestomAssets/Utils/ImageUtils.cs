using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace QuestomAssets.Utils
{
    public class ImageUtils
    {
        public static byte[] LoadToRGBAndMipmap(Bitmap image, int targetWidth, int targetHeight, int maxMips, out int actualMips)
        {
            using (MemoryStream msMips = new MemoryStream())
            {
                int currentWidth = targetWidth;
                int currentHeight = targetHeight;
                int mipCount = 0;
                while (currentWidth >= 1 && currentHeight >= 1 && mipCount < maxMips)
                {
                    byte[] mipData = LoadToRGBAndSize(image, currentWidth, currentHeight);
                    msMips.Write(mipData, 0, mipData.Length);
                    currentWidth /= 2;
                    currentHeight /= 2;
                    mipCount++;
                }
                actualMips = mipCount;
                return msMips.ToArray();
            }
        }

        public static Bitmap TextureToBitmap(AssetsChanger.Texture2DObject texture)
        {
            //does unity store mips of compressed textures individually, or does it compress the entire thing?
            switch (texture.TextureFormat)
            {
                case AssetsChanger.Texture2DObject.TextureFormatType.RGB24:
                    return ConvertFromRGB24Texture(texture);
                //case AssetsChanger.Texture2DObject.TextureFormatType.ETC2_RGB:
                default:
                    throw new NotImplementedException($"Texture type {texture.ToString()} isn't currently supported.");
            }
        }

        
        private static Bitmap ConvertFromRGB24Texture(AssetsChanger.Texture2DObject texture)
        {
            Bitmap image = new Bitmap(texture.Width, texture.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            System.Drawing.Imaging.BitmapData bitmapData =
                image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                image.PixelFormat);
            IntPtr bmpPtr = bitmapData.Scan0;
            byte[] imageBytes = new byte[Math.Abs(bitmapData.Stride) * image.Height];
            System.Runtime.InteropServices.Marshal.Copy(texture.ImageData, 0, bmpPtr, imageBytes.Length);
            image.UnlockBits(bitmapData);
            return image;
        }

        public static byte[] LoadToRGBAndSize(Bitmap image, int width, int height)
        {

            Bitmap newImage = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(newImage);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            g.DrawImage(image,new Rectangle(0, 0, width, height));
            newImage.RotateFlip(RotateFlipType.RotateNoneFlipXY);
            image = newImage;

            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            System.Drawing.Imaging.BitmapData bitmapData =
                image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                image.PixelFormat);
            IntPtr bmpPtr = bitmapData.Scan0;
            byte[] imageBytes = new byte[Math.Abs(bitmapData.Stride) * image.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpPtr, imageBytes, 0, imageBytes.Length);
            image.UnlockBits(bitmapData);            
            return imageBytes;
        }

        //public static byte[] ConvertToETC2
    }
}
