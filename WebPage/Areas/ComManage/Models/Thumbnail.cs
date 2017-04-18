using Common;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace WebPage.Areas.ComManage.Models
{
    /// <summary>
    /// 縮略圖構造類
    /// </summary>
    public class Thumbnail
    {
        private Image srcImage;
        private string srcFileName;

        /// <summary>
        /// 創建
        /// </summary>
        /// <param name="FileName">原始圖片路徑</param>
        public bool SetImage(string FileName)
        {
            srcFileName = Utils.GetMapPath(FileName);
            try
            {
                srcImage = Image.FromFile(srcFileName);
            }
            catch
            {
                return false;
            }
            return true;

        }

        /// <summary>
        /// 回檔
        /// </summary>
        /// <returns></returns>
        public bool ThumbnailCallback()
        {
            return false;
        }

        /// <summary>
        /// 生成縮略圖,返回縮略圖的Image對象
        /// </summary>
        /// <param name="Width">縮略圖寬度</param>
        /// <param name="Height">縮略圖高度</param>
        /// <returns>縮略圖的Image對象</returns>
        public Image GetImage(int Width, int Height)
        {
            Image img;
            Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(ThumbnailCallback);
            img = srcImage.GetThumbnailImage(Width, Height, callb, IntPtr.Zero);
            return img;
        }

        /// <summary>
        /// 保存縮略圖
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        public void SaveThumbnailImage(int Width, int Height)
        {
            switch (Path.GetExtension(srcFileName).ToLower())
            {
                case ".png":
                    SaveImage(Width, Height, ImageFormat.Png);
                    break;
                case ".gif":
                    SaveImage(Width, Height, ImageFormat.Gif);
                    break;
                case ".bmp":
                    SaveImage(Width, Height, ImageFormat.Bmp);
                    break;
                default:
                    SaveImage(Width, Height, ImageFormat.Jpeg);
                    break;
            }
        }

        /// <summary>
        /// 生成縮略圖並保存
        /// </summary>
        /// <param name="Width">縮略圖的寬度</param>
        /// <param name="Height">縮略圖的高度</param>
        /// <param name="imgformat">保存的圖像格式</param>
        /// <returns>縮略圖的Image對象</returns>
        public void SaveImage(int Width, int Height, ImageFormat imgformat)
        {
            if (imgformat != ImageFormat.Gif && (srcImage.Width > Width) || (srcImage.Height > Height))
            {
                Image img;
                Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                img = srcImage.GetThumbnailImage(Width, Height, callb, IntPtr.Zero);
                srcImage.Dispose();
                img.Save(srcFileName, imgformat);
                img.Dispose();
            }
        }

        #region Helper

        /// <summary>
        /// 保存圖片
        /// </summary>
        /// <param name="image">Image 對象</param>
        /// <param name="savePath">保存路徑</param>
        /// <param name="ici">指定格式的編解碼參數</param>
        private static void SaveImage(Image image, string savePath, ImageCodecInfo ici)
        {
            //設置 原圖片 物件的 EncoderParameters 物件
            EncoderParameters parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(Encoder.Quality, ((long)100));
            image.Save(savePath, ici, parameters);
            parameters.Dispose();
        }

        /// <summary>
        /// 獲取圖像編碼解碼器的所有相關資訊
        /// </summary>
        /// <param name="mimeType">包含編碼解碼器的多用途網際郵件擴充協議 (MIME) 類型的字串</param>
        /// <returns>返回圖像編碼解碼器的所有相關資訊</returns>
        private static ImageCodecInfo GetCodecInfo(string mimeType)
        {
            ImageCodecInfo[] CodecInfo = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in CodecInfo)
            {
                if (ici.MimeType == mimeType)
                    return ici;
            }
            return null;
        }

        /// <summary>
        /// 計算新尺寸
        /// </summary>
        /// <param name="width">原始寬度</param>
        /// <param name="height">原始高度</param>
        /// <param name="maxWidth">最大新寬度</param>
        /// <param name="maxHeight">最大新高度</param>
        /// <returns></returns>
        private static Size ResizeImage(int width, int height, int maxWidth, int maxHeight)
        {
            //此次2012-02-05修改過=================
            if (maxWidth <= 0)
                maxWidth = width;
            if (maxHeight <= 0)
                maxHeight = height;
            //以上2012-02-05修改過=================
            decimal MAX_WIDTH = (decimal)maxWidth;
            decimal MAX_HEIGHT = (decimal)maxHeight;
            decimal ASPECT_RATIO = MAX_WIDTH / MAX_HEIGHT;

            int newWidth, newHeight;
            decimal originalWidth = (decimal)width;
            decimal originalHeight = (decimal)height;

            if (originalWidth > MAX_WIDTH || originalHeight > MAX_HEIGHT)
            {
                decimal factor;
                // determine the largest factor 
                if (originalWidth / originalHeight > ASPECT_RATIO)
                {
                    factor = originalWidth / MAX_WIDTH;
                    newWidth = Convert.ToInt32(originalWidth / factor);
                    newHeight = Convert.ToInt32(originalHeight / factor);
                }
                else
                {
                    factor = originalHeight / MAX_HEIGHT;
                    newWidth = Convert.ToInt32(originalWidth / factor);
                    newHeight = Convert.ToInt32(originalHeight / factor);
                }
            }
            else
            {
                newWidth = width;
                newHeight = height;
            }
            return new Size(newWidth, newHeight);
        }

        /// <summary>
        /// 得到圖片格式
        /// </summary>
        /// <param name="name">檔案名稱</param>
        /// <returns></returns>
        public static ImageFormat GetFormat(string name)
        {
            string ext = name.Substring(name.LastIndexOf(".") + 1);
            switch (ext.ToLower())
            {
                case "jpg":
                case "jpeg":
                    return ImageFormat.Jpeg;
                case "bmp":
                    return ImageFormat.Bmp;
                case "png":
                    return ImageFormat.Png;
                case "gif":
                    return ImageFormat.Gif;
                default:
                    return ImageFormat.Jpeg;
            }
        }
        #endregion

        /// <summary>
        /// 製作小正方形
        /// </summary>
        /// <param name="image">圖片物件</param>
        /// <param name="newFileName">新地址</param>
        /// <param name="newSize">長度或寬度</param>
        public static void MakeSquareImage(Image image, string newFileName, int newSize)
        {
            int i = 0;
            int width = image.Width;
            int height = image.Height;
            if (width > height)
                i = height;
            else
                i = width;

            Bitmap b = new Bitmap(newSize, newSize);

            try
            {
                Graphics g = Graphics.FromImage(b);
                //設置高品質插值法
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //設置高品質,低速度呈現平滑程度
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                //清除整個繪圖面並以透明背景色填充
                g.Clear(Color.Transparent);
                if (width < height)
                    g.DrawImage(image, new Rectangle(0, 0, newSize, newSize), new Rectangle(0, (height - width) / 2, width, width), GraphicsUnit.Pixel);
                else
                    g.DrawImage(image, new Rectangle(0, 0, newSize, newSize), new Rectangle((width - height) / 2, 0, height, height), GraphicsUnit.Pixel);

                SaveImage(b, newFileName, GetCodecInfo("image/" + GetFormat(newFileName).ToString().ToLower()));
            }
            finally
            {
                image.Dispose();
                b.Dispose();
            }
        }

        /// <summary>
        /// 製作小正方形
        /// </summary>
        /// <param name="fileName">圖片檔案名</param>
        /// <param name="newFileName">新地址</param>
        /// <param name="newSize">長度或寬度</param>
        public static void MakeSquareImage(string fileName, string newFileName, int newSize)
        {
            MakeSquareImage(Image.FromFile(fileName), newFileName, newSize);
        }

        /// <summary>
        /// 製作遠程小正方形
        /// </summary>
        /// <param name="url">圖片url</param>
        /// <param name="newFileName">新地址</param>
        /// <param name="newSize">長度或寬度</param>
        public static void MakeRemoteSquareImage(string url, string newFileName, int newSize)
        {
            Stream stream = GetRemoteImage(url);
            if (stream == null)
                return;
            Image original = Image.FromStream(stream);
            stream.Close();
            MakeSquareImage(original, newFileName, newSize);
        }

        /// <summary>
        /// 製作縮略圖
        /// </summary>
        /// <param name="original">圖片物件</param>
        /// <param name="newFileName">新圖路徑</param>
        /// <param name="maxWidth">最大寬度</param>
        /// <param name="maxHeight">最大高度</param>
        public static void MakeThumbnailImage(Image original, string newFileName, int maxWidth, int maxHeight)
        {
            Size _newSize = ResizeImage(original.Width, original.Height, maxWidth, maxHeight);

            using (Image displayImage = new Bitmap(original, _newSize))
            {
                try
                {
                    if (original.Width > maxWidth)
                    {
                        displayImage.Save(newFileName, original.RawFormat);
                    }
                }
                finally
                {
                    original.Dispose();
                }
            }
        }

        /// <summary>
        /// 製作縮略圖
        /// </summary>
        /// <param name="fileName">檔案名</param>
        /// <param name="newFileName">新圖路徑</param>
        /// <param name="maxWidth">最大寬度</param>
        /// <param name="maxHeight">最大高度</param>
        public static void MakeThumbnailImage(string fileName, string newFileName, int maxWidth, int maxHeight)
        {
            //2012-02-05修改過，支持替換
            byte[] imageBytes = File.ReadAllBytes(fileName);
            Image img = Image.FromStream(new System.IO.MemoryStream(imageBytes));
            if (img.Width > maxWidth)
            {
                MakeThumbnailImage(img, newFileName, maxWidth, maxHeight);
            }
            //原文
            //MakeThumbnailImage(Image.FromFile(fileName), newFileName, maxWidth, maxHeight);
        }

        #region 2012-2-19 新增生成圖片縮略圖方法
        /// <summary>
        /// 生成縮略圖
        /// </summary>
        /// <param name="fileName">源圖路徑（絕對路徑）</param>
        /// <param name="newFileName">縮略圖路徑（絕對路徑）</param>
        /// <param name="width">縮略圖寬度</param>
        /// <param name="height">縮略圖高度</param>
        /// <param name="mode">生成縮略圖的方式</param>    
        public static bool MakeThumbnailImage(string fileName, string newFileName, int width, int height, string mode)
        {
            Image originalImage = Image.FromFile(fileName);
            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            if (ow > towidth)
            {
                switch (mode)
                {
                    case "HW"://指定高寬縮放（可能變形）                
                        break;
                    case "W"://指定寬，高按比例                    
                        toheight = originalImage.Height * width / originalImage.Width;
                        break;
                    case "H"://指定高，寬按比例
                        towidth = originalImage.Width * height / originalImage.Height;
                        break;
                    case "Cut"://指定高寬裁減（不變形）                
                        if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                        {
                            oh = originalImage.Height;
                            ow = originalImage.Height * towidth / toheight;
                            y = 0;
                            x = (originalImage.Width - ow) / 2;
                        }
                        else
                        {
                            ow = originalImage.Width;
                            oh = originalImage.Width * height / towidth;
                            x = 0;
                            y = (originalImage.Height - oh) / 2;
                        }
                        break;
                    default:
                        break;
                }

                //新建一個bmp圖片
                Bitmap b = new Bitmap(towidth, toheight);
                try
                {
                    //新建一個畫板
                    Graphics g = Graphics.FromImage(b);
                    //設置高品質插值法
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    //設置高品質,低速度呈現平滑程度
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    //清空畫布並以透明背景色填充
                    g.Clear(Color.Transparent);
                    //在指定位置並且按指定大小繪製原圖片的指定部分
                    g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight), new Rectangle(x, y, ow, oh), GraphicsUnit.Pixel);

                    SaveImage(b, newFileName, GetCodecInfo("image/" + GetFormat(newFileName).ToString().ToLower()));
                }
                catch (System.Exception e)
                {
                    throw e;
                }
                finally
                {
                    originalImage.Dispose();
                    b.Dispose();
                }
                return true;
            }
            else
            {
                originalImage.Dispose();
                return false;
            }
        }
        #endregion

        #region 2012-10-30 新增圖片裁剪方法
        /// <summary>
        /// 裁剪圖片並保存
        /// </summary>
        /// <param name="fileName">源圖路徑（絕對路徑）</param>
        /// <param name="newFileName">縮略圖路徑（絕對路徑）</param>
        /// <param name="maxWidth">縮略圖寬度</param>
        /// <param name="maxHeight">縮略圖高度</param>
        /// <param name="cropWidth">裁剪寬度</param>
        /// <param name="cropHeight">裁剪高度</param>
        /// <param name="X">X軸</param>
        /// <param name="Y">Y軸</param>
        public static bool MakeThumbnailImage(string fileName, string newFileName, int maxWidth, int maxHeight, int cropWidth, int cropHeight, int X, int Y)
        {
            byte[] imageBytes = File.ReadAllBytes(fileName);
            Image originalImage = Image.FromStream(new System.IO.MemoryStream(imageBytes));
            Bitmap b = new Bitmap(cropWidth, cropHeight);
            try
            {
                using (Graphics g = Graphics.FromImage(b))
                {
                    //設置高品質插值法
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    //設置高品質,低速度呈現平滑程度
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    //清空畫布並以透明背景色填充
                    g.Clear(Color.Transparent);
                    //在指定位置並且按指定大小繪製原圖片的指定部分
                    g.DrawImage(originalImage, new Rectangle(0, 0, cropWidth, cropWidth), X, Y, cropWidth, cropHeight, GraphicsUnit.Pixel);
                    Image displayImage = new Bitmap(b, maxWidth, maxHeight);
                    SaveImage(displayImage, newFileName, GetCodecInfo("image/" + GetFormat(newFileName).ToString().ToLower()));
                    return true;
                }
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                b.Dispose();
            }
        }
        #endregion

        /// <summary>
        /// 製作遠程縮略圖
        /// </summary>
        /// <param name="url">圖片URL</param>
        /// <param name="newFileName">新圖路徑</param>
        /// <param name="maxWidth">最大寬度</param>
        /// <param name="maxHeight">最大高度</param>
        public static void MakeRemoteThumbnailImage(string url, string newFileName, int maxWidth, int maxHeight)
        {
            Stream stream = GetRemoteImage(url);
            if (stream == null)
                return;
            Image original = Image.FromStream(stream);
            stream.Close();
            MakeThumbnailImage(original, newFileName, maxWidth, maxHeight);
        }

        /// <summary>
        /// 獲取圖片流
        /// </summary>
        /// <param name="url">圖片URL</param>
        /// <returns></returns>
        private static Stream GetRemoteImage(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            request.ContentLength = 0;
            request.Timeout = 20000;
            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
                return response.GetResponseStream();
            }
            catch
            {
                return null;
            }
        }
    }
    /// <summary>
    /// 浮水印構造類
    /// </summary>
    public class WaterMark
    {
        /// <summary>
        /// 圖片浮水印
        /// </summary>
        /// <param name="imgPath">伺服器圖片相對路徑</param>
        /// <param name="filename">保存檔案名</param>
        /// <param name="watermarkFilename">浮水印檔相對路徑</param>
        /// <param name="watermarkStatus">圖片浮水印位置 0=不使用 1=左上 2=中上 3=右上 4=左中  9=右下</param>
        /// <param name="quality">附加浮水印圖片品質,0-100</param>
        /// <param name="watermarkTransparency">浮水印的透明度 1--10 10為不透明</param>
        public static void AddImageSignPic(string imgPath, string filename, string watermarkFilename, int watermarkStatus, int quality, int watermarkTransparency)
        {
            if (!File.Exists(Utils.GetMapPath(imgPath)))
                return;
            byte[] _ImageBytes = File.ReadAllBytes(Utils.GetMapPath(imgPath));
            Image img = Image.FromStream(new System.IO.MemoryStream(_ImageBytes));
            filename = Utils.GetMapPath(filename);

            if (watermarkFilename.StartsWith("/") == false)
                watermarkFilename = "/" + watermarkFilename;
            watermarkFilename = Utils.GetMapPath(watermarkFilename);
            if (!File.Exists(watermarkFilename))
                return;
            Graphics g = Graphics.FromImage(img);
            //設置高品質插值法
            //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //設置高品質,低速度呈現平滑程度
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Image watermark = new Bitmap(watermarkFilename);

            if (watermark.Height >= img.Height || watermark.Width >= img.Width)
                return;

            ImageAttributes imageAttributes = new ImageAttributes();
            ColorMap colorMap = new ColorMap();

            colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
            colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
            ColorMap[] remapTable = { colorMap };

            imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

            float transparency = 0.5F;
            if (watermarkTransparency >= 1 && watermarkTransparency <= 10)
                transparency = (watermarkTransparency / 10.0F);


            float[][] colorMatrixElements = {
                                                new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                                new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                                                new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                                                new float[] {0.0f,  0.0f,  0.0f,  transparency, 0.0f},
                                                new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                                            };

            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            int xpos = 0;
            int ypos = 0;

            switch (watermarkStatus)
            {
                case 1:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)(img.Height * (float).01);
                    break;
                case 2:
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)(img.Height * (float).01);
                    break;
                case 3:
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)(img.Height * (float).01);
                    break;
                case 4:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 5:
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 6:
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 7:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
                case 8:
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
                case 9:
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
            }

            g.DrawImage(watermark, new Rectangle(xpos, ypos, watermark.Width, watermark.Height), 0, 0, watermark.Width, watermark.Height, GraphicsUnit.Pixel, imageAttributes);

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType.IndexOf("jpeg") > -1)
                    ici = codec;
            }
            EncoderParameters encoderParams = new EncoderParameters();
            long[] qualityParam = new long[1];
            if (quality < 0 || quality > 100)
                quality = 80;

            qualityParam[0] = quality;

            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityParam);
            encoderParams.Param[0] = encoderParam;

            if (ici != null)
                img.Save(filename, ici, encoderParams);
            else
                img.Save(filename);

            g.Dispose();
            img.Dispose();
            watermark.Dispose();
            imageAttributes.Dispose();
        }

        /// <summary>
        /// 文字浮水印
        /// </summary>
        /// <param name="imgPath">伺服器圖片相對路徑</param>
        /// <param name="filename">保存檔案名</param>
        /// <param name="watermarkText">浮水印文字</param>
        /// <param name="watermarkStatus">圖片浮水印位置 0=不使用 1=左上 2=中上 3=右上 4=左中  9=右下</param>
        /// <param name="quality">附加浮水印圖片品質,0-100</param>
        /// <param name="fontname">字體</param>
        /// <param name="fontsize">字體大小</param>
        public static void AddImageSignText(string imgPath, string filename, string watermarkText, int watermarkStatus, int quality, string fontname, int fontsize)
        {
            byte[] _ImageBytes = File.ReadAllBytes(Utils.GetMapPath(imgPath));
            Image img = Image.FromStream(new System.IO.MemoryStream(_ImageBytes));
            filename = Utils.GetMapPath(filename);

            Graphics g = Graphics.FromImage(img);
            Font drawFont = new Font(fontname, fontsize, FontStyle.Regular, GraphicsUnit.Pixel);
            SizeF crSize;
            crSize = g.MeasureString(watermarkText, drawFont);

            float xpos = 0;
            float ypos = 0;

            switch (watermarkStatus)
            {
                case 1:
                    xpos = (float)img.Width * (float).01;
                    ypos = (float)img.Height * (float).01;
                    break;
                case 2:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = (float)img.Height * (float).01;
                    break;
                case 3:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = (float)img.Height * (float).01;
                    break;
                case 4:
                    xpos = (float)img.Width * (float).01;
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case 5:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case 6:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case 7:
                    xpos = (float)img.Width * (float).01;
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
                case 8:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
                case 9:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
            }

            g.DrawString(watermarkText, drawFont, new SolidBrush(Color.White), xpos + 1, ypos + 1);
            g.DrawString(watermarkText, drawFont, new SolidBrush(Color.Yellow), xpos, ypos);

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType.IndexOf("jpeg") > -1)
                    ici = codec;
            }
            EncoderParameters encoderParams = new EncoderParameters();
            long[] qualityParam = new long[1];
            if (quality < 0 || quality > 100)
                quality = 80;

            qualityParam[0] = quality;

            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityParam);
            encoderParams.Param[0] = encoderParam;

            if (ici != null)
                img.Save(filename, ici, encoderParams);
            else
                img.Save(filename);

            g.Dispose();
            img.Dispose();
        }
    }
}