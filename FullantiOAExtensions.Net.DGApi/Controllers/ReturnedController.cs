using FullantiOAExtensions.Core.Database;
using FullantiOAExtensions.Core.Database.Entities;
using FullantiOAExtensions.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FullantiOAExtensions.Net.DGApi.Controllers
{
    /// <summary>
    /// 退货接口
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ReturnedController : ControllerBase
    {
        private readonly SyncDbContext syncDbContext;
        private readonly ConfigHelper configHelper;
        private readonly HttpUtil httpUtil;
        private readonly SecurityController securityController;
        private readonly EFCoreDbContext txDbContext;

        /// <summary>
        /// ioc
        /// </summary>
        /// <param name="syncDbContext"></param>
        /// <param name="configHelper"></param>
        /// <param name="httpUtil"></param>
        /// <param name="securityController"></param>
        public ReturnedController(SyncDbContext syncDbContext, ConfigHelper configHelper, HttpUtil httpUtil, SecurityController securityController)
        {
            this.syncDbContext = syncDbContext;
            this.configHelper = configHelper;
            this.httpUtil = httpUtil;
            this.securityController = securityController;
            string connectionString = configHelper.GetConnectionString("DG");
            var optionsBuilder = new DbContextOptionsBuilder<EFCoreDbContext>();
            optionsBuilder.UseSqlServer(connectionString!);
            txDbContext = new EFCoreDbContext(optionsBuilder.Options);
        }

        /// <summary>
        /// OA推送
        /// </summary>
        /// <param name="param">JSON:{ 'code': 'YB241210001', 'oauser': 'FD00646' }</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<dynamic> SendOA([FromBody] dynamic param)
        {
            if (param == null || param!.code == string.Empty || param!.oauser == string.Empty)
                return new { success = false, message = "参数错误" };
            try
            {
                var oaUser = await securityController.OAUser(param.oauser.ToString());
                MFTY order = await GetOrder(param.code.ToString());
                List<TFTY> details = await GetDetails(param.code.ToString());

                if (oaUser == null || order == null || details == null)
                    return new { success = false, message = "获取数据失败" };

                //构建表单
                var mainForm = new Dictionary<string, object>
                {
                    { "主体公司", "湖南富兰地工具股份有限公司" },
                    { "申请人", oaUser.bindingUser.id.ToString() },
                    { "申请部门", oaUser.bindingUser.departmentId.ToString() },
                    { "申请时间", DateTime.Now },
                    { "ERP外购退货单号", order.TY_NO },
                    { "ERP验收单号", order.TI_NO },
                    { "厂商", order.CUST }
                };
                var sonForm = new List<Dictionary<string, object>>();
                details!.ForEach(detail =>
                {
                    sonForm.Add(new Dictionary<string, object>
                    {
                        { "ERP入库单号", detail.RK_NO },
                        { "ERP采购单号", detail.BIL_NO },
                        { "品号", detail.PRD_NO },
                        { "品名", detail.PRD_NAME },
                        { "规格", detail.SPC },
                        { "仓库", detail.WH },
                        { "单位", detail.UT },
                        { "验货数量", detail.QTY_CHK },
                        { "合格数量", detail.QTY_OK },
                        { "不合格量", detail.QTY_FAIL },
                        { "不合格原因", detail.CAUSE }
                    });
                });
                //构建请求
                string formMain = "formmain_0235", formSon = "formson_0236", template = "WGTH";
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
                                "data", new Dictionary<string, object>
                                {
                                    { formMain, mainForm },
                                    { formSon, sonForm }
                                }
                            },
                            { "subject", "外购退货单" }
                        }
                    }
                };
                var requestHeader = new Dictionary<string, string> { { "token", oaUser.id.ToString() } };
                string postUrl = configHelper.GetAppSettings<string>("OA:OaUrl") + configHelper.GetAppSettings<string>("OA:FlowUrl");
                //提交请求
                var result = await httpUtil.PostAsync<dynamic>(postUrl, requestData, requestHeader);
                if (result.code != "0")
                    return new { success = false, result.message };
                var res = await syncDbContext.Database.ExecuteSqlInterpolatedAsync($"update SyncReturned set State=2 where Code={param.code.ToString()}");
                return res > 0 ? new { success = true, message = "推送成功" } : new { success = false, message = "推送失败" };
            }
            catch (Exception ex)
            {
                return new { success = false, message = ex.Message };
            }
        }

        /// <summary>
        /// 回调
        /// </summary>
        /// <param name="param">JSON:{ 'code':'YB241210001','auditor':'肖总','auditTime':'2024-12-10','state':'3' }</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<dynamic> Callback([FromBody] dynamic param)
        {
            if (param == null || param!.code == string.Empty || param!.state == string.Empty || param!.auditor == string.Empty || param!.auditTime == string.Empty)
                return new { success = false, message = "参数错误" };
            try
            {
                string state = param!.state == 3 ? "已审核" : "未通过";
                var success =
                    await txDbContext.Database.ExecuteSqlInterpolatedAsync($"update MF_TY_Z set OA_CHK={param!.auditor.ToString()},OA_TM={param.auditTime.ToString()},OA_STATE={state} where TY_NO={param.code.ToString()}") > 0 &&
                    await syncDbContext.Database.ExecuteSqlInterpolatedAsync($"update SyncReturned set State={int.Parse(param!.state.ToString())},Auditor={param.auditor.ToString()},AuditTime={param.auditTime.ToString()} where Code={param.code.ToString()}") > 0;
                return new { success, message = success ? "回调成功" : "回调失败" };
            }
            catch (Exception ex)
            {
                return new { success = false, message = ex.Message };
            }
        }

        /// <summary>
        /// 是否审核
        /// </summary>
        /// <param name="code">订单编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<dynamic> Audited(string code)
        {
            string sqlStr = "select a.TY_NO,a.TI_NO,a.CHK_MAN,b.NAME as CUST from MF_TY a left join CUST b on b.CUS_NO=a.CUS_NO where a.TY_ID='YB' and a.TY_NO=@code";
            var result = await txDbContext.MFTY.FromSqlRaw(sqlStr, new SqlParameter[] { new("@code", code) }).SingleOrDefaultAsync();
            if (result == null)
                return new { success = false, message = "订单不存在" };
            return new { success = true, audit = string.IsNullOrEmpty(result.CHK_MAN) ? false : true };
        }

        private async Task<MFTY> GetOrder(string code)
        {
            string sqlStr = "select a.TY_NO,a.TI_NO,a.CHK_MAN,b.NAME as CUST from MF_TY a left join CUST b on b.CUS_NO=a.CUS_NO where a.TY_ID='YB' and a.TY_NO=@code";
            var order = await txDbContext.MFTY.FromSqlRaw(sqlStr, new SqlParameter[] { new("@code", code) }).FirstOrDefaultAsync();
            return order;
        }

        private async Task<List<TFTY>> GetDetails(string code)
        {
            string sqlStr = "SELECT B.BIL_NO,B.RK_NO,B.PRD_NO,B.PRD_NAME,D.SPC,F.NAME AS WH,D.UT,C.QTY_CHK,C.QTY_OK,B.QTY_CHK AS QTY_FAIL,E.NAME AS CAUSE FROM dbo.MF_TY A " +
                "LEFT JOIN dbo.TF_TY B ON B.TY_ID=A.TY_ID AND B.TY_NO=A.TY_NO " +
                "LEFT JOIN dbo.TF_TY C ON C.TY_NO=B.TI_NO AND C.PRE_ITM=B.PRE_ITM " +
                "LEFT JOIN dbo.PRDT D ON D.PRD_NO=B.PRD_NO " +
                "LEFT JOIN dbo.SPC_LST E ON E.SPC_NO=B.SPC_NO " +
                "LEFT JOIN dbo.MY_WH F ON F.WH = B.WH " +
                "WHERE A.TY_ID='YB' AND C.TY_ID='TY' AND A.TY_NO=@code";
            var details = await txDbContext.TFTY.FromSqlRaw(sqlStr, new SqlParameter[] { new("@code", code) }).ToListAsync();
            return details;
        }
    }
}
