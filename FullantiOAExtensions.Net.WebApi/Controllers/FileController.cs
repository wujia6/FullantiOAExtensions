using FullantiOAExtensions.Core.Utils;
using Microsoft.AspNetCore.Mvc;

namespace FullantiOAExtensions.Net.WebApi.Controllers
{
    /// <summary>
    /// 文件接口
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment hostingEnvironment;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        public FileController(IWebHostEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="formFiles"></param>
        /// <returns></returns>
        [HttpPost]
        public dynamic UploadFile(IFormFileCollection formFiles)
        {
            try
            {
                //获取上传文件
                //IFormFileCollection formFiles = HttpContext.Request.Form.Files;
                //是否有上传文件
                if (formFiles == null || formFiles.Count == 0)
                {
                    return new HttpResult(false, "请选择上传文件", null);
                }

                string saveUrl = Path.Combine(hostingEnvironment.ContentRootPath, "Upload");

                var returnFiles = new List<dynamic>();
                foreach (var file in formFiles!)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string fullFileName = Path.Combine(saveUrl, fileName);
                    //文件目录是否存在
                    if (!Directory.Exists(saveUrl))
                        Directory.CreateDirectory(saveUrl);
                    //将流写入文件
                    using (var stream = file.OpenReadStream())
                    {
                        // 把 Stream 转换成 byte[]
                        var bytes = new byte[stream.Length];
                        stream.Read(bytes, 0, bytes.Length);
                        // 设置当前流的位置为流的开始
                        stream.Seek(0, SeekOrigin.Begin);
                        // 把 byte[] 写入文件
                        var fs = new FileStream(fullFileName, FileMode.Create);
                        var bw = new BinaryWriter(fs);
                        bw.Write(bytes);
                        bw.Close();
                        fs.Close();
                    }
                    returnFiles.Add(new { name = fileName, url = fullFileName });
                }
                return new HttpResult(true, "上传成功", returnFiles);
            }
            catch (Exception)
            {
                return new HttpResult(false, "服务器内部错误，请联系管理员或稍后再试", null);
            }
        }

        /// <summary>
        /// 文件上传（异步）
        /// </summary>
        /// <param name="formFiles">文件集合</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<dynamic> UploadAsync(IFormFileCollection formFiles)
        {
            HttpResult result = default!;
            await Task.Run(() =>
            {
                try
                {
                    //获取上传文件
                    //IFormFileCollection formFiles = HttpContext.Request.Form.Files;
                    //是否有上传文件
                    if (formFiles == null || formFiles.Count == 0)
                    {
                        result = new HttpResult(false, "请选择上传图片", null);
                        return;
                    }

                    string saveUrl = Path.Combine(hostingEnvironment.ContentRootPath, "Upload");

                    var returnFiles = new List<dynamic>();
                    foreach (var file in formFiles!)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string fullFileName = Path.Combine(saveUrl, fileName);
                        //文件目录是否存在
                        if (!Directory.Exists(saveUrl))
                            Directory.CreateDirectory(saveUrl);
                        //将流写入文件
                        using (var stream = file.OpenReadStream())
                        {
                            // 把 Stream 转换成 byte[]
                            var bytes = new byte[stream.Length];
                            stream.Read(bytes, 0, bytes.Length);
                            // 设置当前流的位置为流的开始
                            stream.Seek(0, SeekOrigin.Begin);
                            // 把 byte[] 写入文件
                            var fs = new FileStream(fullFileName, FileMode.Create);
                            var bw = new BinaryWriter(fs);
                            bw.Write(bytes);
                            bw.Close();
                            fs.Close();
                        }
                        returnFiles.Add(new { name = fileName, url = fullFileName });
                    }
                    result = new HttpResult(true, "上传成功", returnFiles);
                }
                catch (Exception)
                {
                    result = new HttpResult(false, "服务器内部错误，请联系管理员或稍后再试", null);
                }
            });
            return result;
        }
    }
}
