using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace QuestomAssets.BeatSaber
{
    class ImageUtils
    {
        public static byte[] LoadToRGBAndMipmap(Bitmap image, int targetWidth, int targetHeight, int maxMips, out int actualMips)
        {
            actualMips = 1;
            return LoadToRGBAndSize(image, targetWidth, targetHeight);
            using (MemoryStream msMips = new MemoryStream())
            {
                int currentWidth = targetWidth;
                int currentHeight = targetHeight;
                int mipCount = 0;
                while (currentWidth >= 1 && currentHeight >= 1 && mipCount < maxMips)
                {
                    byte[] mipData = LoadToRGBAndSize(image, targetWidth, targetHeight);
                    msMips.Write(mipData, 0, mipData.Length);
                    targetWidth /= 2;
                    targetHeight /= 2;
                    mipCount++;
                }
                actualMips = mipCount;
                return msMips.ToArray();
            }
        }

        public static byte[] LoadToRGBAndSize(Bitmap image, int width, int height)
        {
            byte[] imageBytes;
            using (MemoryStream msCover = new MemoryStream())
            {
                if (image.Width != width || image.Height != height)
                {
                    image = new Bitmap(image, new Size(width, height));
                }
                image.Save(msCover, System.Drawing.Imaging.ImageFormat.Bmp);

                imageBytes = new byte[msCover.Length - 54];
                byte[] msBytes = msCover.ToArray();
                Array.Copy(msBytes, 54, imageBytes, 0, imageBytes.Length);
            }
            for (int i = 0; i < imageBytes.Length - 2; i += 3)
            {
                byte hold = imageBytes[i];
                imageBytes[i] = imageBytes[i + 2];
                imageBytes[i + 2] = hold;
            }
            return imageBytes;
        }
    }
}
