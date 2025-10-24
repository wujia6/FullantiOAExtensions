using FullantiOAExtensions.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FullantiOAExtensions.Net.DGApi.Controllers
{
    /// <summary>
    /// 安全接口
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SecurityController : ControllerBase
    {
        private readonly HttpUtil httpUtil;
        private readonly ConfigHelper configHelper;

        /// <summary>
        /// ioc
        /// </summary>
        /// <param name="httpUtil"></param>
        /// <param name="configHelper"></param>
        public SecurityController(HttpUtil httpUtil, ConfigHelper configHelper)
        {
            this.httpUtil = httpUtil;
            this.configHelper = configHelper;
        }

        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="loginName">登录名（OA账号）</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<dynamic> TokenAsync(string loginName)
        {
            string oaUrl = configHelper.GetAppSettings<string>("OA:OaUrl");
            string tokenUrl = configHelper.GetAppSettings<string>("OA:TokenUrl");
            string restName = configHelper.GetAppSettings<string>("OA:RestName");
            string password = configHelper.GetAppSettings<string>("OA:Password");
            string postUrl = oaUrl + tokenUrl.Replace("@restName", restName).Replace("@password", password).Replace("@loginName", loginName);
            dynamic responseContent = await httpUtil.GetAsync<dynamic>(postUrl);
            return responseContent.id;
        }

        /// <summary>
        /// 获取OA用户
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<dynamic> OAUser(string loginName)
        {
            string oaUrl = configHelper.GetAppSettings<string>("OA:OaUrl");
            string tokenUrl = configHelper.GetAppSettings<string>("OA:TokenUrl");
            string restName = configHelper.GetAppSettings<string>("OA:RestName");
            string password = configHelper.GetAppSettings<string>("OA:Password");
            string postUrl = oaUrl + tokenUrl.Replace("@restName", restName).Replace("@password", password).Replace("@loginName", loginName);
            return await httpUtil.GetAsync<dynamic>(postUrl, new Dictionary<string, string> 
            { 
                { "Accept", "application/json" } 
            });
        }
    }
}
