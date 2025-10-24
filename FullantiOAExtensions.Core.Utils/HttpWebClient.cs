using System.Dynamic;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace FullantiOAExtensions.Core.Utils
{
    /// <summary>
    /// http请求工具类
    /// </summary>
    public class HttpWebClient
    {
        private readonly IHttpClientFactory httpClientFactory;

        public HttpWebClient(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Build HttpClient
        /// </summary>
        /// <param name="timeoutSecond"></param>
        /// <returns></returns>
        private HttpClient BuildHttpClient(Dictionary<string, string> requestHeaders, int? timeoutSecond)
        {
            var httpClient = httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Clear();   //为了使客户端不受最后一个请求的影响，清除DefaultRequestHeaders
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (requestHeaders != null)
            {
                foreach (var headerItem in requestHeaders)
                {
                    if (!httpClient.DefaultRequestHeaders.Contains(headerItem.Key))
                        httpClient.DefaultRequestHeaders.Add(headerItem.Key, headerItem.Value);
                }
            }
            if (timeoutSecond.HasValue)
                httpClient.Timeout = TimeSpan.FromSeconds(timeoutSecond.Value);
            return httpClient;
        }

        /// <summary>
        /// Generate HttpRequestMessage
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestBodys"></param>
        /// <param name="method"></param>
        /// <param name="requestHeaders"></param>
        /// <returns></returns>
        private HttpRequestMessage GenerateHttpRequestMessage(string url, string requestBodys, HttpMethod method, Dictionary<string, string> requestHeaders)
        {
            var request = new HttpRequestMessage(method, url);
            if (!string.IsNullOrEmpty(requestBodys))
            {
                request.Content = new StringContent(requestBodys);
            }
            if (requestHeaders != null)
            {
                foreach (var header in requestHeaders)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            return request;
        }

        /// <summary>
        ///  Generate StringContent
        /// </summary>
        /// <param name="requestBodys"></param>
        /// <param name="requestHeaders"></param>
        /// <returns></returns>
        private StringContent GenerateStringContent(string requestBodys, Dictionary<string, string>? requestHeaders = null)
        {
            var content = new StringContent(requestBodys, Encoding.UTF8);
            if (requestHeaders != null)
            {
                foreach (var headerItem in requestHeaders)
                {
                    content.Headers.Add(headerItem.Key, headerItem.Value);
                }
            }
            return content;
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestHeaders"></param>
        /// <param name="timeoutSecond"></param>
        /// <returns></returns>
        public async Task<HttpResult> GetAsync(string url, Dictionary<string, string>? requestHeaders = null, int timeoutSecond = 180)
        {
            try
            {
                var client = BuildHttpClient(requestHeaders!, timeoutSecond);
                var response = await client.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<dynamic>(responseContent);
                return new HttpResult(response.IsSuccessStatusCode, response.IsSuccessStatusCode ? "请求成功" : "请求失败", data!);
            }
            catch (Exception ex)
            {
                throw new Exception($"HttpGet:{url} Error", ex);
            }
        }

        /// <summary>
        /// Post
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestBodys"></param>
        /// <param name="requestHeaders"></param>
        /// <param name="timeoutSecond"></param>
        /// <returns></returns>
        public async Task<HttpResult> PostAsync(string url, string requestBodys, Dictionary<string, string>? requestHeaders = null, int timeoutSecond = 180)
        {
            try
            {
                var client = BuildHttpClient(requestHeaders!, timeoutSecond);
                var requestContent = GenerateStringContent(requestBodys, requestHeaders);
                var response = await client.PostAsync(url, requestContent);
                string responseContent = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<dynamic>(responseContent);
                return new HttpResult(response.IsSuccessStatusCode, response.IsSuccessStatusCode?"请求成功":"请求失败", data!);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Post Form
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="formData"></param>
        /// <param name="requestHeaders"></param>
        /// <param name="timeoutSecond"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<T> PostFormAsync<T>(string url, Dictionary<string,string> formData, Dictionary<string, string>? requestHeaders = null, int timeoutSecond = 180)
        {
            try
            {
                var client = BuildHttpClient(requestHeaders!, timeoutSecond);
                using (var formDataContent = new FormUrlEncodedContent(formData))
                {
                    var result = await client.PostAsync(url, formDataContent).Result.Content.ReadAsStringAsync();
                    var resp = JsonConvert.DeserializeObject<T>(result);
                    return resp;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"HttpPost:{url} Error", ex);
            }
        }

        /// <summary>
        /// Put
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestBodys"></param>
        /// <param name="requestHeaders"></param>
        /// <param name="timeoutSecond"></param>
        /// <returns></returns>
        public async Task<HttpResult> PutAsync(string url, string requestBodys, Dictionary<string, string>? requestHeaders = null, int timeoutSecond = 180)
        {
            try
            {
                var client = BuildHttpClient(requestHeaders, timeoutSecond);
                var requestContent = GenerateStringContent(requestBodys, requestHeaders);
                var response = await client.PutAsync(url, requestContent);
                var responseContent = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<dynamic>(responseContent);
                return new HttpResult(response.IsSuccessStatusCode, response.IsSuccessStatusCode ? "请求成功" : "请求失败", data!);
            }
            catch (Exception ex)
            {
                throw new Exception($"HttpPut:{url} Error", ex);
            }
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestHeaders"></param>
        /// <param name="timeoutSecond"></param>
        /// <returns></returns>
        public async Task<HttpResult> DeleteAsync(string url, Dictionary<string, string>? requestHeaders = null, int timeoutSecond = 180)
        {
            try
            {
                var client = BuildHttpClient(requestHeaders!, timeoutSecond);
                var response = await client.DeleteAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<dynamic>(responseContent);
                return new HttpResult(response.IsSuccessStatusCode, response.IsSuccessStatusCode ? "请求成功" : "请求失败", data!);
            }
            catch (Exception ex)
            {
                throw new Exception($"HttpDelete:{url} Error", ex);
            }
        }
    }
}
