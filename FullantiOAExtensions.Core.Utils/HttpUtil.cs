using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace FullantiOAExtensions.Core.Utils
{
    /// <summary>
    /// http请求工具类
    /// </summary>
    public class HttpUtil
    {
        private readonly IHttpClientFactory httpClientFactory;

        public HttpUtil(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<TBody> GetAsync<TBody>(string url, Dictionary<string, string> headerDict = null)
        {
            return await SendAsync<TBody>("application/json;charset=UTF-8", HttpMethod.Get, url, null, headerDict);
        }

        public async Task<TBody> PostAsync<TBody>(string url, dynamic requestData, Dictionary<string, string> headerDict = null)
        {
            return await SendAsync<TBody>("application/json", HttpMethod.Post, url, requestData, headerDict);
        }

        public async Task<TBody> PutAsync<TBody>(string url, dynamic requestData, Dictionary<string, string> headerDict = null)
        {
            return await SendAsync<TBody>("application/json", HttpMethod.Put, url, requestData, headerDict);
        }

        public async Task<TBody> DeleteAsync<TBody>(string url, Dictionary<string, string> headerDict = null)
        {
            return await SendAsync<TBody>("application/json", HttpMethod.Delete, url, null, headerDict);
        }

        public static async Task<TBody> PostFormAsync<TBody>(HttpClient _client, string url, Dictionary<string, string> param)
        {
            try
            {
                using (var multipartFormDataContent = new FormUrlEncodedContent(param))
                {
                    Console.WriteLine(JsonConvert.SerializeObject(param));
                    var result = await _client.PostAsync(url, multipartFormDataContent).Result.Content.ReadAsStringAsync();
                    var resp = JsonConvert.DeserializeObject<TBody>(result);
                    return resp;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TBody"></typeparam>
        /// <param name="ContentType"></param>
        /// <param name="method"></param>
        /// <param name="url"></param>
        /// <param name="requestData"></param>
        /// <param name="headerDict"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<TBody> SendAsync<TBody>(string ContentType, HttpMethod method, string url, dynamic requestData, Dictionary<string, string> headerDict = null)
        {
            string content = "";
            if (requestData != null)
            {
                content = JsonConvert.SerializeObject(requestData, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            }
            try
            {
                var client = httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Clear();
                using (var message = new HttpRequestMessage(method, url))
                {
                    if (headerDict != null)
                    {
                        foreach (var d in headerDict)
                        {
                            message.Headers.Add(d.Key, d.Value);
                        }
                    }
                    using (HttpContent httpContent = new StringContent(content, Encoding.UTF8))
                    {
                        httpContent.Headers.ContentType = MediaTypeHeaderValue.Parse(ContentType);
                        message.Content = httpContent;
                        var httpResponseMessage = await client.SendAsync(message);
                        string json = await httpResponseMessage.Content.ReadAsStringAsync();
                        var resp = JsonConvert.DeserializeObject<TBody>(json);
                        return resp;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
