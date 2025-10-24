using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace FullantiOAExtensions.Core.Utils
{
    public class HttpFile
    {
        private const string FILE_BOUNDARY = "---------------------------7d4a6d158c9";
        //private readonly IHttpClientFactory clientFactory;

        //public HttpFile(IHttpClientFactory httpClientFactory)
        //{
        //    this.clientFactory = httpClientFactory;
        //}

        private byte[] GetStartData(FileInfo file)
        {
            var sb = new StringBuilder();
            sb.Append("--" + FILE_BOUNDARY + "\r\n");
            sb.Append("Content-Disposition: form-data; name=\"1\"; filename=\"" + file.Name + "\"\r\n");
            sb.Append("Content-Type: msoffice\r\n\r\n");
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        //public async Task<long> UploadFileAsync(FileInfo uploadFile, string url)
        //{
        //    try
        //    {
        //        var client = clientFactory.CreateClient();
        //        client.DefaultRequestHeaders.Clear();

        //        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + FILE_BOUNDARY);

        //        using var formData = new MultipartFormDataContent();
        //        byte[] startData = GetStartData(uploadFile);
        //        formData.Add(new ByteArrayContent(startData), "file", uploadFile.Name);

        //        using var fs = new FileStream(uploadFile.FullName, FileMode.Open, FileAccess.Read);
        //        formData.Add(new StreamContent(fs), "file");

        //        HttpResponseMessage response = await client.PostAsync(url, formData);
        //        string responseContent = await response.Content.ReadAsStringAsync();

        //        if (!string.IsNullOrEmpty(responseContent))
        //        {
        //            Console.WriteLine("附件上传成功！！ID:" + responseContent);

        //            dynamic jsonObject = JsonConvert.DeserializeObject(responseContent);
        //            dynamic atts = jsonObject.attrs;
        //            List<Dictionary<string, object>> lists = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(atts.ToString());

        //            foreach (var item in lists)
        //            {
        //                return Convert.ToInt64(item["fileUrl"]);
        //            }
        //        }
        //        else
        //            Console.WriteLine("附件上传失败！！");
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("附件上传失败！！错误信息：" + e.Message);
        //    }
        //    return 0L;
        //}

        public long Upload(FileInfo uploadFile, string url)
        {
            using var fs = new FileStream(uploadFile.FullName, FileMode.Open);

            var hc = (HttpWebRequest)WebRequest.Create(url);
            hc.Method = "POST";
            hc.ContentType = "multipart/form-data; boundary=" + FILE_BOUNDARY;

            using var requestStream = hc.GetRequestStream();
            byte[] startData = GetStartData(uploadFile);
            requestStream.Write(startData, 0, startData.Length);

            byte[] buffer = new byte[8192];
            int bytesRead;
            while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) != 0)
            {
                requestStream.Write(buffer, 0, bytesRead);
            }

            byte[] endBoundary = Encoding.UTF8.GetBytes("\r\n--" + FILE_BOUNDARY + "--\r\n");
            requestStream.Write(endBoundary, 0, endBoundary.Length);

            using var response = hc.GetResponse().GetResponseStream();
            using var reader = new StreamReader(response);
            string jsonResponse = reader.ReadToEnd();

            if (!string.IsNullOrEmpty(jsonResponse))
            {
                var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);
                if (jsonObject != null && jsonObject.ContainsKey("atts"))
                {
                    var atts = JsonConvert.DeserializeObject<dynamic>(jsonObject["atts"].ToString()!);
                    return Convert.ToInt64(atts[0].fileUrl);
                    //var lists = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(atts!);
                    //foreach (var json in lists!)
                    //{
                    //    return Convert.ToInt64(json["fileUrl"]);
                    //}
                }
            }
            else
            {
                Console.WriteLine("File upload failed!!");
            }
            return 0;
        }
    }
}
