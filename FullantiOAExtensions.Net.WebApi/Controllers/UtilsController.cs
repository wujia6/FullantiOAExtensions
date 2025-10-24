using FullantiOAExtensions.Core.Utils;
using Microsoft.AspNetCore.Mvc;

namespace FullantiOAExtensions.Net.WebApi.Controllers
{
    /// <summary>
    /// 工具接口
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UtilsController : ControllerBase
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="param">JSON对象</param>
        /// <returns></returns>
        [HttpPost]
        public string DesEncrypt([FromBody] dynamic param)
        {
            string inputStr = param.input.ToString();
            return EncryptHelper.DesEncrypt(inputStr);
        }
    }
}
