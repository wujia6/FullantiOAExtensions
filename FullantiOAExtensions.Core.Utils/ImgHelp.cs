using System.Drawing;
using System.Drawing.Imaging;

namespace FullantiOAExtensions.Core.Utils
{
    /// <summary>
    /// 转换工具类
    /// </summary>
    public class ImgHelp
    {
        /// <summary>
        /// 将Base64字符串转换为Image对象
        /// </summary>
        /// <param name="base64Str">base64字符串</param>
        /// <returns></returns>
        public static Bitmap Base64ToImage(string base64Str)
        {
            Bitmap bitmap;
            try
            {
                byte[] arr = Convert.FromBase64String(base64Str);
                var ms = new MemoryStream(arr);
                var bmp = new Bitmap(ms);
                ms.Close();
                bitmap = bmp;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return bitmap;
        }

        /// <summary>
        /// 将Base64字符串转换为图片并保存到本地
        /// </summary>
        /// <param name="base64Str">base64字符串</param>
        /// <param name="savePath">图片保存地址，如：/Content/Images/10000.png</param>
        /// <returns></returns>
        public static bool SaveBase64ToLocation(string base64Str, string savePath, string fileName)
        {
            var ret = true;
            try
            {
                var bitmap = Base64ToImage(base64Str);
                if (bitmap != null)
                {
                    //图片后缀格式
                    var suffix = fileName[(fileName.LastIndexOf('.') + 1)..].ToLower();
                    var suffixName = suffix == "png" ? ImageFormat.Png :
                        suffix == "jpg" || suffix == "jpeg" ? ImageFormat.Jpeg :
                        suffix == "bmp" ? ImageFormat.Bmp :
                        suffix == "gif" ? ImageFormat.Gif : ImageFormat.Jpeg;

                    //这里复制一份对图像进行保存，否则会出现“GDI+ 中发生一般性错误”的错误提示
                    var bmpNew = new Bitmap(bitmap);
                    string fileFullName = Path.Combine(savePath, fileName);
                    bmpNew.Save(fileFullName, suffixName);
                    bmpNew.Dispose();
                    bitmap.Dispose();
                }
                else
                {
                    ret = false;
                }
            }
            catch (Exception ex)
            {
                ret = false;
            }

            return ret;
        }
    }
}
