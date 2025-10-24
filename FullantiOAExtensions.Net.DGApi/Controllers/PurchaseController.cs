using FullantiOAExtensions.Core.Database;
using FullantiOAExtensions.Core.Database.Entities;
using FullantiOAExtensions.Core.Utils;
using FullantiOAExtensions.Net.DGApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FullantiOAExtensions.Net.DGApi.Controllers
{
    /// <summary>
    /// 采购接口
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PurchaseController : ControllerBase
    {
        private readonly string formMain = "formmain_0078", formSon = "formson_0079", template = "CGD";
        private readonly SecurityController securityController;
        private readonly ConfigHelper configHelper;
        private readonly HttpUtil httpUtil;
        private readonly SyncDbContext syncDbContext;

        private EFCoreDbContext? efCoreDbContext;

        /// <summary>
        /// ioc
        /// </summary>
        /// <param name="securityController"></param>
        /// <param name="configHelper"></param>
        /// <param name="httpUtil"></param>
        /// <param name="syncDbContext"></param>
        public PurchaseController(SecurityController securityController, ConfigHelper configHelper, HttpUtil httpUtil, SyncDbContext syncDbContext)
        {
            this.securityController = securityController;
            this.configHelper = configHelper;
            this.httpUtil = httpUtil;
            this.syncDbContext = syncDbContext;
        }

        /// <summary>
        /// 获取审核状态
        /// </summary>
        /// <param name="orderNo">单号</param>
        /// <param name="source">数据源</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<dynamic> Audited(string orderNo, string source)
        {
            FormattableString sqlstr = $"select a.os_no as No,a.os_dd as CreateDate,b.cus_nm as CustomerName,a.usr as CreateUser,b.oa_src as Source,b.oa_cls as Classify,chk_man as Auditor from mf_pos a left join mf_pos_z b on a.os_no=b.os_no where a.os_id='PO' and a.os_no={orderNo}";
            var serviceDbContext = CreateDbContext(source);
            var mfpos = await serviceDbContext.MFPOS.FromSqlInterpolated(sqlstr).ToListAsync();
            var single = mfpos.FirstOrDefault();
            if ((single is null))
                throw new ArgumentNullException(nameof(single));
            return !string.IsNullOrEmpty(single.Auditor); 
        }

        /// <summary>
        /// OA推送采购单
        /// </summary>
        /// <param name="param">参数对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<dynamic> PushOA([FromBody] OAParam param)
        {
            if (param is null || string.IsNullOrEmpty(param.OrderNo) || string.IsNullOrEmpty(param.Source))
                return new { success = false, message = "参数错误" };
            try
            {
                efCoreDbContext = CreateDbContext(param.Source);

                if (efCoreDbContext is null)
                    return new { success = false, message = "创建业务数据源失败" };

                var order = await GetOrder(param.OrderNo);
                var details = await GetOrderDetails(param.OrderNo);
                var acc = await GetOAUser(order.No, order.Source);
                var oaUser = await securityController.OAUser(acc);

                if (order is null || details is null || details.Count == 0 || oaUser is null)
                    return new { success = false, message = "订单数据，或OA账号异常" };

                //表单构建
                string company = string.Empty;
                string account = oaUser.bindingUser.loginName.ToString();
                if (param.Source.Contains("HN")) company = "湖南富石数控刀具有限公司";
                else if (param.Source.Contains("DG")) company = "湖南富兰地工具股份有限公司";
                else if (param.Source.Contains("JS")) company = "江苏富兰地工具有限公司";
                else company = "山西富兰地工具有限公司";
                var totalMoney = Math.Round(details.Sum(x => x.Total), 2);
                var purchaseOrder = new Dictionary<string, object>
                {
                    { "主体公司", "name|" + company },
                    { "ERP采购单号", param.OrderNo },
                    { "申请人", oaUser.bindingUser.id.ToString() },
                    { "申请部门", oaUser.bindingUser.departmentId.ToString() },
                    { "申请时间", DateTime.Now },
                    { "客户", order.CustomerName },
                    { "采购类型", "name|" + order.Classify },
                    { "合计", totalMoney }
                };
                var purchaseOrderDetails = new List<Dictionary<string, object>>();
                foreach (var detail in details)
                {
                    var dic = new Dictionary<string, object>
                    {
                        { "商品编码", detail.ProductNo },
                        { "厂商名称", order.CustomerName },
                        { "商品名称", detail.ProductName },
                        { "规格型号", detail.Specification },
                        { "单位", "支" },
                        { "材质", "-" },
                        { "数量", detail.Quantity },
                        { "单价", detail.Price },
                        { "受订价格", detail.ReceivePrice.HasValue ? detail.ReceivePrice.Value : string.Empty },
                        { "总价", detail.Quantity * detail.Price },
                        { "备注", detail.Remark ?? string.Empty }
                    };
                    purchaseOrderDetails.Add(dic);
                }
                //请求构建
                var requestData = new Dictionary<string, object>
                {
                    { "appName", "collaboration" },
                    {
                        "data",
                        new Dictionary<string, object>
                        {
                            { "templateCode", template },
                            { "draft", "0" },
                            {
                                "data",
                                new Dictionary<string, object>
                                {
                                    { formMain, purchaseOrder },
                                    { formSon, purchaseOrderDetails }
                                }
                            },
                            { "subject", "采购单" }
                        }
                    }
                };
                var requestHeader = new Dictionary<string, string> { { "token", oaUser.id.ToString() } };
                string postUrl = configHelper.GetAppSettings<string>("OA:OaUrl") + configHelper.GetAppSettings<string>("OA:FlowUrl");

                //提交请求
                var result = await httpUtil.PostAsync<dynamic>(postUrl, requestData, requestHeader);
                if (result.code != "0")
                    return new { success = false, result.message };
                var res = await syncDbContext.Database.ExecuteSqlInterpolatedAsync($"update Sync set State=2 where Code={param.OrderNo} and Source={param.Source}");
                return res > 0 ? new { success = true, message = "推送成功" } : new { success = false, message = "推送失败" };
            }
            catch (Exception ex)
            {
                return new { code = 500, message = "服务器内部错误", error = ex.Message };
            }
        }

        /// <summary>
        /// OA回调采购单
        /// </summary>
        /// <param name="param">参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<dynamic> Callback([FromBody] CallbackParam param)
        {
            var dbContext = CreateDbContext(param.Source);
            if (dbContext is null)
                return new { success = false, message = "数据源创建失败" };
            var result = await dbContext.Database.ExecuteSqlInterpolatedAsync($"update MF_POS_Z set OA_STATE={param.State},OA_CHK={param.OaUser},OA_TM={param.CreateTime} where OS_NO={param.OrderNo}");
            int state = param.State.Equals("通过") ? 3 : 4;
            var res = await syncDbContext.Database.ExecuteSqlInterpolatedAsync($"update Sync set State={state},Auditor={param.OaUser},AuditTime={param.CreateTime} where Code={param.OrderNo} and Source={param.Source}");
            return result > 0 && res > 0 ? new { success = true, message = "回调成功" } : new { success = false, message = "回调失败" };
        }

        /// <summary>
        /// 创建业务数据库
        /// </summary>
        /// <param name="source">数据源</param>
        /// <returns></returns>
        private EFCoreDbContext CreateDbContext(string source)
        {
            try
            {
                var connectionString = source switch
                {
                    "DG" => configHelper.GetConnectionString("DG"),
                    "SX" => configHelper.GetConnectionString("SX"),
                    "JS" => configHelper.GetConnectionString("JS"),
                    _ => configHelper.GetConnectionString("HN"),
                };
                var optionsBuilder = new DbContextOptionsBuilder<EFCoreDbContext>();
                optionsBuilder.UseSqlServer(connectionString!);
                return new EFCoreDbContext(optionsBuilder.Options);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        private async Task<string> GetOAUser(string orderNo, string source)
        {
            var sqlParams = new List<SqlParameter>
            {
                new("@code", orderNo),
                new("@src", source)
            };
            var sync = await syncDbContext.Syncs.FromSqlRaw("select * from Sync where Code=@code and Source=@src", sqlParams.ToArray()).FirstOrDefaultAsync();
            return sync.Operator;
        }

        /// <summary>
        /// 获取天心采购单
        /// </summary>
        /// <param name="orderNo">订单编号</param>
        /// <returns></returns>
        private async Task<MFPOS> GetOrder(string orderNo)
        {
            FormattableString sqlstr = $"select a.os_no as No,a.os_dd as CreateDate,b.cus_nm as CustomerName,a.usr as CreateUser,b.oa_src as Source,b.oa_cls as Classify,chk_man as Auditor from mf_pos a left join mf_pos_z b on a.os_no=b.os_no where a.os_id='PO' and a.os_no={orderNo}";
            var mfpos = await efCoreDbContext.MFPOS.FromSqlInterpolated(sqlstr).ToListAsync();
            return mfpos.FirstOrDefault()!;
        }

        /// <summary>
        /// 获取天心采购单明细
        /// </summary>
        /// <param name="orderNo">订单编号</param>
        /// <returns></returns>
        private async Task<List<TFPOS>> GetOrderDetails(string orderNo)
        {
            FormattableString sqlstr = $"select a.prd_no as ProductNo,a.prd_name as ProductName,b.spc as Specification,a.qty as Quantity,a.up as Price,c.so_up as ReceivePrice,a.rem as Remark from tf_pos a left join prdt b on a.prd_no=b.prd_no left join tf_pos_z c on c.os_no=a.os_no and c.itm=a.itm where a.os_no={orderNo}";
            return await efCoreDbContext.TFPOS.FromSqlInterpolated(sqlstr).ToListAsync();
        }
    }
}
