using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using QuestomAssets.AssetsChanger;

namespace QuestomAssets.Utils
{
    public class ImageUtils
    {
        public enum TextureConversionFormat
        {
            Auto = 0,
            RGB24 = 1,
            ETC2_RGB = 2
        }        

        public static void AssignImageToTexture(Bitmap sourceImage, AssetsChanger.Texture2DObject targetTexture, int targetWidth, int targetHeight, int targetMips = Int32.MaxValue, TextureConversionFormat format = TextureConversionFormat.Auto)
        {
            //right now "Auto" is always RGB24.  Probably will automatically decide based on image size, but doing ETC2 sucks at the moment

            int mips;
            byte[] textureBytes;
            if (format == TextureConversionFormat.ETC2_RGB)
            {
                textureBytes = ConvertToETC2AndMipmap(sourceImage, targetWidth, targetHeight, targetMips, out mips);
                targetTexture.TextureFormat = Texture2DObject.TextureFormatType.ETC2_RGB;
            }
            else //always fall back to RGB24
            {
                textureBytes = ConvertToRGBAndMipmap(sourceImage, targetWidth, targetHeight, targetMips, out mips);
                targetTexture.TextureFormat = AssetsChanger.Texture2DObject.TextureFormatType.RGB24;
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

        public static Bitmap TextureToBitmap(AssetsChanger.Texture2DObject texture)
        {
            //does unity store mips of compressed textures individually, or does it compress the entire thing?
            switch (texture.TextureFormat)
            {
                case AssetsChanger.Texture2DObject.TextureFormatType.RGB24:
                    return ConvertFromRGB24ToBitmap(texture.ImageData, texture.Width, texture.Height, RotateFlipType.RotateNoneFlipXY);

                //not really sure if it can do all these or not
                case AssetsChanger.Texture2DObject.TextureFormatType.ETC2_RGB:
                case AssetsChanger.Texture2DObject.TextureFormatType.ETC_RGB4:
                case AssetsChanger.Texture2DObject.TextureFormatType.ETC2_RGBA8:
                case AssetsChanger.Texture2DObject.TextureFormatType.ETC2_RGBA1:
                    return ConvertETC2ToBitmap(texture.ImageData, texture.Width, texture.Height);
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
            for (int i = 0; (i+3) < rgb.Length && i < limit; i +=3)
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
        #region ETC2
        private static void WriteRGBToPPMFile(string filename, byte[] imageBytes, int width, int height)
        {
            using (FileStream fs = File.Open(filename, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.ASCII, 1024, true))
                {
                    sw.Write("P6\n");
                    sw.Write(width.ToString()+"\n");
                    sw.Write(height.ToString()+"\n");
                    sw.Write("255\n");
                }
                fs.Write(imageBytes, 0, imageBytes.Length);
            }
        }

        private static byte[] ReadPPMFileToRGB(string filename, out int width, out int height, out int maxPixelValue)
        {
            //note: this doesn't cover comments
            byte[] rgb = null;
            using (var fs = File.OpenRead(filename))
            {
                var sb = new StringBuilder();
                sb.Append((char)fs.ReadByte());
                sb.Append((char)fs.ReadByte());
                    
                if (sb.ToString() != "P6")
                    throw new Exception("Converted file was not P6!");
                sb.Clear();
                char c;
                while (char.IsWhiteSpace((c = (char)fs.ReadByte())))
                { }
                sb.Append(c);
                while (!char.IsWhiteSpace((c = (char)fs.ReadByte())))
                {
                    sb.Append(c);
                }
                width = Convert.ToInt32(sb.ToString());
                sb.Clear();
                while (char.IsWhiteSpace((c = (char)fs.ReadByte())))
                { }
                sb.Append(c);
                while (!char.IsWhiteSpace((c = (char)fs.ReadByte())))
                {
                    sb.Append(c);
                }
                height = Convert.ToInt32(sb.ToString());
                sb.Clear();
                //don't care about number of colors, but might as well read it while I'm here
                while (char.IsWhiteSpace((c = (char)fs.ReadByte())))
                { }
                sb.Append(c);
                while (!char.IsWhiteSpace((c = (char)fs.ReadByte())))
                {
                    sb.Append(c);
                }
                maxPixelValue = Convert.ToInt32(sb.ToString());
                
                int dataSize = (int)(fs.Length - fs.Position);
                rgb = new byte[dataSize];
                fs.Read(rgb, 0, rgb.Length);
            }
            return rgb;
        }

        private static void WriteETCToPKMFile(string filename, byte[] rgb, int width, int height)
        {
            using (var fs = File.OpenWrite(filename))
            {
                using (var bw = new BinaryWriter(fs, Encoding.ASCII, true))
                {
                    bw.Write('P');
                    bw.Write('K');
                    bw.Write('M');
                    bw.Write(' ');
                    bw.Write('2');
                    bw.Write('0');
                    //for straight RGB, 01
                    bw.Write((byte)0);
                    bw.Write((byte)1);
                    byte[] word = BitConverter.GetBytes((ushort)width);
                    bw.Write(word[1]);
                    bw.Write(word[0]);
                    word = BitConverter.GetBytes((ushort)height);
                    bw.Write(word[1]);
                    bw.Write(word[0]);
                    //"active area" however you'd calculate that other than the image size?
                    word = BitConverter.GetBytes((ushort)width);
                    bw.Write(word[1]);
                    bw.Write(word[0]);
                    word = BitConverter.GetBytes((ushort)height);
                    bw.Write(word[1]);
                    bw.Write(word[0]);
                    bw.Write(rgb);
                }
            }
        }
        private static byte[] ReadPKMFileToRGB(string filename, out int width, out int height)
        {
            byte[] etc = null;
            using (var fs = File.OpenRead(filename))
            {
                using (var br = new BinaryReader(fs, Encoding.ASCII, true))
                {
                    var magic = Encoding.ASCII.GetString(br.ReadBytes(6));
                    if (!magic.StartsWith("PKM "))
                        throw new Exception("File was not in PKM format!");

                    byte[] bfr = br.ReadBytes(2);
                    Array.Reverse(bfr);
                    ushort texType = BitConverter.ToUInt16(bfr,0);
                    //this is the type
                    //hopefully is 02
                    if (texType != 1)
                        throw new NotSupportedException($"PKM texture type {texType} isn't supported yet!");
                    bfr = br.ReadBytes(2);
                    Array.Reverse(bfr);
                    width = BitConverter.ToUInt16(bfr, 0);
                    bfr = br.ReadBytes(2);
                    Array.Reverse(bfr);
                    height = BitConverter.ToUInt16(bfr, 0);
                    bfr = br.ReadBytes(2);
                    Array.Reverse(bfr);
                    ushort activeWidth = BitConverter.ToUInt16(bfr, 0);
                    bfr = br.ReadBytes(2);
                    Array.Reverse(bfr);
                    ushort activeHeight = BitConverter.ToUInt16(bfr, 0);
                    //hopefully won't need to do anything with active width/height unless alpha containing textures are needed
                    etc = new byte[fs.Length - fs.Position];
                    fs.Read(etc, 0, etc.Length);
                    return etc;
                }
            }

        }

        private static byte[] ConvertToETC2AndMipmap(Bitmap image, int targetWidth, int targetHeight, int maxMips, out int actualMips)
        {
            using (MemoryStream msMips = new MemoryStream())
            {
                int currentWidth = targetWidth;
                int currentHeight = targetHeight;
                int mipCount = 0;
                while (currentWidth >= 1 && currentHeight >= 1 && mipCount < maxMips)
                {
                    
                    byte[] mipData = ConvertToETC2AndSize(image, currentWidth, currentHeight);
                    msMips.Write(mipData, 0, mipData.Length);
                    currentWidth /= 2;
                    currentHeight /= 2;
                    mipCount++;
                }
                actualMips = mipCount;
                return msMips.ToArray();
            }
        }

        private static Bitmap ConvertETC2ToBitmap(byte[] etc1, int width, int height)
        {
            var tmp = Path.GetTempFileName();
            var srcFile = Path.Combine(Path.GetDirectoryName(tmp), Path.GetFileNameWithoutExtension(tmp) + ".pkm");
            File.Move(tmp, srcFile);
            tmp = Path.GetTempFileName();
            var dstFile = Path.Combine(Path.GetDirectoryName(tmp), Path.GetFileNameWithoutExtension(tmp) + ".ppm");
            File.Move(tmp, dstFile);
            WriteETCToPKMFile(srcFile, etc1, width, height);
            DecompressETC(srcFile, dstFile);
            if (!File.Exists(dstFile) || new FileInfo(dstFile).Length < 20)
                throw new Exception("Failed to convert ETC1 to Bitmap!");

            int maxPixelValue;
            byte[] rgb = ReadPPMFileToRGB(dstFile, out width, out height, out maxPixelValue);
            //who cares if deletes fail, disk space is cheap
            try
            {
                File.Delete(srcFile);
            }
            catch { }
            try
            {
                File.Delete(dstFile);
            }
            catch { }
            return ConvertFromRGB24ToBitmap(rgb, width, height, RotateFlipType.RotateNoneFlipY);
        }

        private static byte[] ConvertToETC2AndSize(Bitmap image, int targetWidth, int targetHeight)
        {
            byte[] rgb = ConvertToRGBAndSize(image, targetWidth, targetHeight, RotateFlipType.RotateNoneFlipY);
            var tmp = Path.GetTempFileName();
            var srcFile = Path.Combine(Path.GetDirectoryName(tmp), Path.GetFileNameWithoutExtension(tmp) + ".ppm");
            File.Move(tmp, srcFile);
            tmp = Path.GetTempFileName();
            var dstFile = Path.Combine(Path.GetDirectoryName(tmp), Path.GetFileNameWithoutExtension(tmp) + ".pkm");
            File.Move(tmp, dstFile);
            WriteRGBToPPMFile(srcFile, rgb, targetWidth, targetHeight);
            CompressETC(srcFile, dstFile, true, true, true, Codec.ETC2PACKAGE_RGB_NO_MIPMAPS);
            if (!File.Exists(dstFile) || new FileInfo(dstFile).Length < 20)
                throw new Exception("Failed to convert to ETC1!");
            int width;
            int height;
            byte[] etc1 = ReadPKMFileToRGB(dstFile, out width, out height);
            //who cares if deletes fail, disk space is cheap
            try
            {
                File.Delete(srcFile);
            }
            catch { }
            try
            {
                File.Delete(dstFile);
            }
            catch { }
            return etc1;
        }

        private static class Codec {
            public const string ETC2PACKAGE_RGB_NO_MIPMAPS = "RGB";
            public const string ETC2PACKAGE_RGBA_NO_MIPMAPS = "RGBA";
            public const string ETC2PACKAGE_RGBA1_NO_MIPMAPS = "RGBA1";
            public const string ETC2PACKAGE_R_NO_MIPMAPS = "R";
            public const string ETC2PACKAGE_RG_NO_MIPMAPS = "RG";
            public const string ETC2PACKAGE_R_SIGNED_NO_MIPMAPS = "R_signed";
            public const string ETC2PACKAGE_RG_SIGNED_NO_MIPMAPS = "RG_signed";
            public const string ETC2PACKAGE_sRGB_NO_MIPMAPS = "sRGB";
            public const string ETC2PACKAGE_sRGBA_NO_MIPMAPS = "sRGBA";
            public const string ETC2PACKAGE_sRGBA1_NO_MIPMAPS = "sRGBA1";
        }        		

        private static void CompressETC(string srcFile, string dstFile, bool fast, bool perceptual, bool etc2, string format)
        {
            ProcessStartInfo psi = new ProcessStartInfo(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "etcpack.exe"), $"\"{srcFile}\" \"{dstFile}\" -s {(fast?"fast":"slow")} -e {(perceptual?"perceptual":"nonperceptual")} -c {(etc2?"etc2":"etc1")} -f {(etc2?format:"RGB")}");
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = true;
            Process p = Process.Start(psi);
            StringBuilder sb = new StringBuilder();
            p.Start();
            while (!p.HasExited)
            {
                sb.Append(p.StandardOutput.ReadToEnd());
                System.Threading.Thread.Sleep(100);
            }
            if (p.ExitCode != 0)
                throw new Exception($"etcpack exited with non zero: {sb.ToString()}");
        }

        private static void DecompressETC(string srcFile, string dstFile)
        {
            
            ProcessStartInfo psi = new ProcessStartInfo(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "etcpack.exe"), $"\"{srcFile}\" \"{dstFile}\"");
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = true;
            Process p = Process.Start(psi);
            StringBuilder sb = new StringBuilder();
            p.Start();
            while (!p.HasExited)
            {
                sb.Append(p.StandardOutput.ReadToEnd());
                System.Threading.Thread.Sleep(100);
            }
            if (p.ExitCode != 0)
                throw new Exception($"etcpack exited with non zero: {sb.ToString()}");
        }

        #endregion
    }
}
