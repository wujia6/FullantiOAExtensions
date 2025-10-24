namespace FullantiOAExtensions.Core.Utils
{
    public class HttpResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public dynamic Data { get; set; }

        public HttpResult(bool success, string message, dynamic data)
        {
            Success = success;
            Message = message;
            Data = data;
        }
    }
}
