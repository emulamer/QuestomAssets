using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using QuestomAssets.AssetsChanger;
using QuestomAssets.Utils;

namespace QuestomAssets
{
    public static class Extensions
    {
        public static string ToBase64PNG(this Texture2DObject texture)
        {
            var image = ImageUtils.TextureToBitmap(texture);
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static string RemoveSuffix(this string value, string suffix)
        {
            if (!value.EndsWith(suffix))
                return value;
            //don't remove it if that's all there is
            if (value == suffix)
                return value;

            return value.Substring(0, value.Length - suffix.Length);
        }

    }
}
