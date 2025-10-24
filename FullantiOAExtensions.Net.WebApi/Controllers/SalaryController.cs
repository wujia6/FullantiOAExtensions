using FullantiOAExtensions.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using MiniExcelLibs;
using FullantiOAExtensions.Core.Database.Entities;
using FullantiOAExtensions.Core.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Text;
using FullantiOAExtensions.Net.WebApi.Model;
using Newtonsoft.Json;
using System.Web;

namespace FullantiOAExtensions.Net.WebApi.Controllers
{
    /// <summary>
    /// 工资接口
    /// 
    /// 湖南：FH00035
    /// 东莞：FD00363
    /// 山西：FS00005
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SalaryController : ControllerBase
    {
        private readonly IWebHostEnvironment environment;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly HttpUtil httpUtil;
        private readonly ConfigHelper configHelper;
        private readonly OaExtendDbContext dbContext;

        private static Dictionary<string, List<Salary>> dics = new();

        /// <summary>
        /// ioc
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="configHelper"></param>
        /// <param name="httpUtil"></param>
        /// <param name="dbContext"></param>
        public SalaryController(
            IWebHostEnvironment environment, 
            IHttpContextAccessor httpContextAccessor, 
            ConfigHelper configHelper, 
            HttpUtil httpUtil, 
            OaExtendDbContext dbContext)
        {
            this.environment = environment;
            this.httpContextAccessor = httpContextAccessor;
            this.configHelper = configHelper;
            this.httpUtil = httpUtil;
            this.dbContext = dbContext;
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="account"></param>
        /// <param name="department"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<dynamic> Search(string account, string? department = null, string? month = null)
        {
            dics.Remove(account);
            var sqlParams = new List<SqlParameter>();
            string strSql = "select * from Salary where 1=1";
            if (!account.Equals("sysadmin"))
            {
                strSql += " and Createor=@account";
                sqlParams.Add(new SqlParameter("@account", account));
            }
            if (!string.IsNullOrEmpty(department))
            {
                strSql += " and Department=@department";
                sqlParams.Add(new SqlParameter("@department", department));
            }
            if (!string.IsNullOrEmpty(month))
            {
                strSql += " and Month=@month";
                sqlParams.Add(new SqlParameter("@month", month));
            }
            var salaries = await dbContext.Salaries.FromSqlRaw(strSql, sqlParams.ToArray()).ToListAsync();
            dics.Add(account, salaries);
            return salaries;
        }

        /// <summary>
        /// 日期范围搜索
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="department">部门</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<dynamic> Search2(string account, string? department = null, string? startDate = null, string? endDate = null)
        {
            dics.Remove(account);
            var sqlParams = new List<SqlParameter>();
            string strSql = "select * from Salary where 1=1";
            if (!account.Equals("sysadmin"))
            {
                strSql += " and Createor=@account";
                sqlParams.Add(new SqlParameter("@account", account));
            }
            if (!string.IsNullOrEmpty(department))
            {
                strSql += " and Department=@department";
                sqlParams.Add(new SqlParameter("@department", department));
            }
            if(!string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate))
            {
                strSql += " and Month=@month";
                sqlParams.Add(new SqlParameter("@month", startDate));
            }
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                strSql += " and convert(date,Month+'-01') between convert(date,@startDate) and eomonth(convert(date,@endDate))";
                sqlParams.Add(new SqlParameter("@startDate", startDate + "-01"));
                sqlParams.Add(new SqlParameter("@endDate", endDate + "-01"));
            }
            var salaries = await dbContext.Salaries.FromSqlRaw(strSql, sqlParams.ToArray()).ToListAsync();
            dics.Add(account, salaries);
            return salaries;
        }

        /// <summary>
        /// 查询个人薪资
        /// </summary>
        /// <param name="code">加密工号</param>
        /// <param name="month">月份</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<dynamic> Personal(string code, string month)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(month))
                return new { success = false, message = "参数错误" };
            code = EncryptHelper.DesDecrypt(code);
            string sqlStr = "select * from Salary where WorkNo=@code and Month=@month";
            var sqlParams = new List<SqlParameter>
            {
                new ("@code", code),
                new ("@month", month)
            };
            return await dbContext.Salaries.FromSqlRaw(sqlStr, sqlParams.ToArray()).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="param">参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<dynamic> Login([FromBody] dynamic param)
        {
            if (param == null) return new { success = false, message = "参数错误" };
            string strSql = "select * from SalaryManager where Account=@account and Password=@password";
            //string account = EncryptHelper.DesEncrypt(param.account.ToString());
            var sqlParams = new List<SqlParameter>
            {
                new("@account", param.account.ToString()),
                new("@password", EncryptHelper.DesEncrypt(param.password.ToString()))
            };
            var users = await dbContext.SalaryManager.FromSqlRaw(strSql, sqlParams.ToArray()).ToListAsync();
            if (!users.Any())
                return new { success = false, message = "错误的账号或密码" };
            return users.FirstOrDefault()!.Account;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="param">JSON对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<dynamic> Register([FromBody] SalaryManager param)
        {
            if (param == null) return new { success = false, message = "参数错误" };
            string sql = $"select * from SalaryManager where Account=@account";
            var sqlParam = new List<SqlParameter>
            {
                new("@account", param.Account),
            };
            var exists = dbContext.SalaryManager.FromSqlRaw(sql, sqlParam.ToArray()).FirstOrDefault();
            if (exists != null)
                return new { success = false, message = "用户已存在" };
            string strSql = "insert into SalaryManager(Account,Password,Name) values(@account,@password,@name)";
            var sqlParams = new List<SqlParameter>
            {
                new("@account", param.Account),
                new("@password", EncryptHelper.DesEncrypt(param.Password)),
                new("@name", param.Name)
            };
            int count = await dbContext.Database.ExecuteSqlRawAsync(strSql, sqlParams.ToArray());
            return count > 0 ? new { success = true, message = "注册成功" } : new { success = false, message = "注册失败" };
        }

        /// <summary>
        /// 密码变更
        /// </summary>
        /// <param name="manager">{ "account": "string", "password": "string", "pwdNew": "string" }</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<dynamic> Password_Change([FromBody] dynamic manager)
        {
            string account = manager.account.ToString();
            string password = manager.password.ToString();
            string pwdNew = manager.pwdNew.ToString();

            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(pwdNew))
                return new { success = false, message = "参数错误" };

            string sql = "select * from SalaryManager where Account=@account and Password=@password";
            var sqlParam = new List<SqlParameter>
            {
                new("@account", account),
                new("@password", EncryptHelper.DesEncrypt(password))
            };
            var mgr = dbContext.SalaryManager.FromSqlRaw(sql, sqlParam.ToArray()).FirstOrDefault();

            if (mgr == null)
                return new { success = false, message = "用户不存在" };

            string sqlStr = "update SalaryManager set Password=@password where Account=@account";
            var sqlParams = new List<SqlParameter>
            {
                new("@password", EncryptHelper.DesEncrypt(pwdNew)),
                new("@account", account)
            };
            int count = await dbContext.Database.ExecuteSqlRawAsync(sqlStr, sqlParams.ToArray());
            return count > 0 ? new { success = true, message = "修改成功" } : new { success = false, message = "修改失败" };
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="param">JSON集合</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<dynamic> Remove([FromBody] List<Salary> param)
        {
            if (param == null) return new { success = false, message = "参数错误" };
            var ids = param.Select(src => src.Id).ToList();
            var strIds = new StringBuilder();
            ids.ForEach(src => strIds.Append(src).Append(','));
            strIds.Remove(strIds.Length - 1, 1);
            string strSql = $"delete Salary where Id in({strIds})";
            var count = await dbContext.Database.ExecuteSqlRawAsync(strSql);
            return new { success = count > 0, message = count > 0 ? "删除成功" : "删除失败" };
        }

        /// <summary>
        /// 推送OA(作废)
        /// </summary>
        /// <param name="paramList">JSON集合</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<dynamic> PushOA([FromBody] List<Salary> paramList)
        {
            if (paramList == null || paramList.Count == 0)
                return new { success = false, message = "参数不能为空" };

            var salaries = paramList.ConvertAll<PersonalSalary>(x => new PersonalSalary
            {
                WorkNo = x.WorkNo,
                Name = x.Name,
                Department = x.Department,
                Company = x.Company,
                Month = x.Month,
                //IdCard = x.IdCard,
                //DutyLevel = x.DutyLevel,
                Group = x.Group,
                Duty = x.Duty,
                JoinTime = x.JoinTime,
                Type = x.Type,
                AttendanceDays = x.AttendanceDays,
                RequireAttendanceDays = x.RequireAttendanceDays,
                AbsenceDays = x.AbsenceDays,
                WorkTime1 = x.WorkTime1,
                WorkTime2 = x.WorkTime2,
                WorkTime3 = x.WorkTime3,
                NightDays = x.NightDays,
                SynthesisSalary = x.SynthesisSalary,
                BasicSalary = x.BasicSalary,
                PositionSalary = x.PositionSalary,
                ConfidentialitySalary = x.ConfidentialitySalary,
                PerformanceSalary = x.PerformanceSalary,
                PerformanceAward = x.PerformanceAward,
                PerformanceDeduct = x.PerformanceDeduct,
                SenioritySubsidy = x.SenioritySubsidy,
                HousingSubsidy = x.HousingSubsidy,
                PhoneChargeSubsidy = x.PhoneChargeSubsidy,
                NightSubsidy = x.NightSubsidy,
                PositionSubsidy = x.PositionSubsidy,
                OtherSubsidy = x.OtherSubsidy,
                WelfareTotal = x.WelfareTotal,
                GrowthAward = x.GrowthAward,
                ProductionAward = x.ProductionAward,
                QualityAward = x.QualityAward,
                OtherAward = x.OtherAward,
                AwardTotal = x.AwardTotal,
                Enhanced = x.Enhanced,
                OverTimePay = x.OverTimePay,
                AbsenceDeduction = x.AbsenceDeduction,
                BudgetSalary = x.BudgetSalary,
                SocialSecurity = x.SocialSecurity,
                ProvidentFund = x.ProvidentFund,
                PersionalSocialSecurity = x.PersionalSocialSecurity,
                PersionalProvidentFund = x.PersionalProvidentFund,
                Utilities = x.Utilities,
                RepairAttendance = x.RepairAttendance,
                PersionalTax = x.PersionalTax,
                Other = x.Other,
                DeductionTotal = x.DeductionTotal,
                PracticalSalary = x.PracticalSalary
            });
            //此处添加5空对象，对应excel表头部分
            salaries.Insert(0, new PersonalSalary());
            salaries.Insert(0, new PersonalSalary());
            salaries.Insert(0, new PersonalSalary());
            salaries.Insert(0, new PersonalSalary());
            salaries.Insert(0, new PersonalSalary());

            try
            {
                //写入csv文件
                string fileName = Guid.NewGuid().ToString();
                string filePath = Path.Combine(environment.ContentRootPath, "Files", $"{fileName}.csv");
                MiniExcel.SaveAs(filePath, salaries, printHeader:false);

                //创建请求体
                using var client = new HttpClient();
                using var form = new MultipartFormDataContent();
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                form.Add(fileContent, "uploadedInputStream", fileName);

                //发送POST请求，读取响应内容
                string oaUrl = configHelper.GetAppSettings<string>("OA:OaUrl");
                string companyId = configHelper.GetAppSettings<string>("OA:CompanyId");
                dynamic staff = await Staff("FH00035");
                string url = $"{oaUrl}/hr/wagestrip?state=1&token={staff.token.ToString()}&accountId={companyId}&createUserId={staff.id.ToString()}";
                var response = await client.PostAsync(url, form);
                string responseContent = await response.Content.ReadAsStringAsync();
                return new { code = response.StatusCode, content = responseContent };
            }
            catch (Exception ex)
            {
                return new { success = false, code = 500, message = ex.Message };
            }
        }

        /// <summary>
        /// 推送OA消息
        /// </summary>
        /// <param name="param">JSON对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<dynamic> SendOA([FromBody] SendParam param)
        {
            if (param.Salaries == null || param.Salaries.Count == 0 || string.IsNullOrEmpty(param.Account))
                return new { success = false, message = "参数错误" };
            try
            {
                //生成消息体
                var codeList = param.Salaries.Select(src => new { src.WorkNo, src.Month }).ToList();
                dynamic staff = await Staff(param.Account);
                string codes = string.Empty;
                var messages = new List<Dictionary<string, object>>();
                codeList.ForEach(src =>
                {
                    long messageId = DateTime.Now.Ticks;
                    string code = EncryptHelper.DesEncrypt(src.WorkNo);
                    string htmlUrl = $"{configHelper.GetAppSettings<string>("ExteriorServerUrl")}/OA/gzt.html?code={HttpUtility.UrlEncode(code)}&month={src.Month}";
                    var msg = new Dictionary<string, object>
                    {
                        { "thirdpartyRegisterCode",3004 },
                        { "thirdpartyMessageId", messageId },
                        { "messageContent", "您的" + src.Month + "月份薪资条明细，请注意保密！如有疑问，可在薪资条发放后5个工作日内联系相关人员进行处理" },
                        { "thirdpartySenderId", staff.loginName.ToString() },
                        { "thirdpartyReceiverId", src.WorkNo },
                        { "creation_date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
                        { "messageH5URL", htmlUrl },
                        { "messageURL", htmlUrl },
                        { "noneBindingSender", staff.loginName.ToString() },
                        { "noneBindingReceiver", src.WorkNo },
                        { "messageType", 1 }
                    };
                    messages.Add(msg);
                    codes += "'"+src.WorkNo + "',";
                });
                //发送请求
                string oaUrl = configHelper.GetAppSettings<string>("OA:OaUrl");
                string messageUrl = configHelper.GetAppSettings<string>("OA:SingleMessageUrl");
                //dynamic response = await httpUtil.PostAsync<dynamic>(oaUrl + messageUrl, new { messages }, new Dictionary<string, string>
                //{
                //    { "token", staff.token.ToString() }
                //});
                //string responseText = JsonConvert.SerializeObject(response);
                //if (responseText.Contains("true"))
                //{
                //    string sqlStr = $"update Salary set Sended='1' where Month='{codeList.FirstOrDefault()!.Month}' and WorkNo in ({codes.TrimEnd(',')})";
                //    int res = dbContext.Database.ExecuteSqlRaw(sqlStr);
                //}
                //return responseText;
                int success = 0;
                var fails = new List<string>();
                foreach(var msg in messages)
                {
                    dynamic response = await httpUtil.PostAsync<dynamic>(oaUrl + messageUrl, msg, new Dictionary<string, string>
                    {
                        { "token", staff.token.ToString() }
                    });
                    string workNo = msg["thirdpartyReceiverId"].ToString()!;
                    string responseText = JsonConvert.SerializeObject(response);
                    if (responseText.Contains("true"))
                    {
                        string sqlStr = $"update Salary set Sended='1' where Month='{codeList.FirstOrDefault()!.Month}' and WorkNo='{workNo}'";
                        int res = dbContext.Database.ExecuteSqlRaw(sqlStr);
                        success++;
                    }
                    else
                        fails.Add(workNo);
                }
                return new { success, fails };
            }
            catch (Exception ex)
            {
                return new { success = false, code = 500, message = ex.Message };
            }
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="formFile"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<dynamic> Import(IFormFile formFile, string account)
        {
            if (formFile == null || string.IsNullOrEmpty(account))
                return new { success = false, message = "参数错误" };

            string[] arr = { ".xls", ".xlsx" };
            if (!arr.Contains(Path.GetExtension(formFile.FileName)))
                return new { success = false, message = "只能是excel文件" };
            
            try
            {
                //读取excel
                using var stearm = formFile.OpenReadStream();
                var salarys = stearm.Query<Salary>(startCell: "A1").ToList();
                if (salarys == null || salarys.Count == 0)
                    return new { success = false, message = "上传数据不能为空" };

                string oaUrl = configHelper.GetAppSettings<string>("OA:OaUrl");
                string companyId = configHelper.GetAppSettings<string>("OA:CompanyId");
                dynamic staff = await Staff(account);

                //获取员工
                var headers = new Dictionary<string, string> { { "token", staff.token.ToString() } };
                var responseJson = await httpUtil.GetAsync<List<dynamic>>(oaUrl + "/orgMembers/" + companyId, headers);
                var personals = responseJson.Select(src => new { src.code, src.orgDepartmentId, src.orgPostName }).ToList();

                //构建sqlvalues
                var fields = new StringBuilder();
                var fails = new List<string>();
                salarys.ForEach(x =>
                {
                    RemoveExists(x.WorkNo, x.Month);
                    var person = personals.SingleOrDefault(p => p.code == x.WorkNo);
                    if (person != null)
                    {
                        x.Department = FullDepartment(person.orgDepartmentId.ToString(), staff.token.ToString());
                        x.Duty = person.orgPostName;
                        x.Createor = account;
                        fields.Append("('" + x.WorkNo + "',");
                        fields.Append("'" + x.Name + "',");
                        fields.Append("'" + x.IdCard + "',");
                        fields.Append("'" + x.Department + "',");
                        fields.Append("'" + x.Company + "',");
                        fields.Append("'" + x.Month + "',");
                        fields.Append("'" + x.Group + "',");
                        fields.Append("'" + x.Duty + "',");
                        fields.Append("'" + x.JoinTime + "',");
                        fields.Append("'" + x.Type + "',");
                        fields.Append("'" + x.AttendanceDays + "',");
                        fields.Append("'" + x.RequireAttendanceDays + "',");
                        fields.Append("'" + x.AbsenceDays + "',");
                        fields.Append("'" + x.WorkTime1 + "',");
                        fields.Append("'" + x.WorkTime2 + "',");
                        fields.Append("'" + x.WorkTime3 + "',");
                        fields.Append("'" + x.NightDays + "',");
                        fields.Append("'" + x.SynthesisSalary + "',");
                        fields.Append("'" + x.BasicSalary + "',");
                        fields.Append("'" + x.PositionSalary + "',");
                        fields.Append("'" + x.ConfidentialitySalary + "',");
                        fields.Append("'" + x.PerformanceSalary + "',");
                        fields.Append("'" + x.PerformanceAward + "',");
                        fields.Append("'" + x.PerformanceDeduct + "',");
                        fields.Append("'" + x.SenioritySubsidy + "',");
                        fields.Append("'" + x.HousingSubsidy + "',");
                        fields.Append("'" + x.PhoneChargeSubsidy + "',");
                        fields.Append("'" + x.NightSubsidy + "',");
                        fields.Append("'" + x.PositionSubsidy + "',");
                        fields.Append("'" + x.OtherSubsidy + "',");
                        fields.Append("'" + x.WelfareTotal + "',");
                        fields.Append("'" + x.GrowthAward + "',");
                        fields.Append("'" + x.ProductionAward + "',");
                        fields.Append("'" + x.QualityAward + "',");
                        fields.Append("'" + x.OtherAward + "',");
                        fields.Append("'" + x.AwardTotal + "',");
                        fields.Append("'" + x.Enhanced + "',");
                        fields.Append("'" + x.OverTimePay + "',");
                        fields.Append("'" + x.AbsenceDeduction + "',");
                        fields.Append("'" + x.BudgetSalary + "',");
                        fields.Append("'" + x.SocialSecurity + "',");
                        fields.Append("'" + x.ProvidentFund + "',");
                        fields.Append("'" + x.PersionalSocialSecurity + "',");
                        fields.Append("'" + x.PersionalProvidentFund + "',");
                        fields.Append("'" + x.Utilities + "',");
                        fields.Append("'" + x.RepairAttendance + "',");
                        fields.Append("'" + x.PersionalTax + "',");
                        fields.Append("'" + x.Other + "',");
                        fields.Append("'" + x.DeductionTotal + "',");
                        fields.Append("'" + x.PracticalSalary + "',");
                        fields.Append("'" + x.Createor + "',");
                        fields.Append("'" + 0 + "'");
                        fields.Append("),");
                    }
                    else
                        fails.Add(x.WorkNo);
                });
                string strSql = string.Format("insert into Salary values{0}", fields.ToString().TrimEnd(','));
                int count = await dbContext.Database.ExecuteSqlRawAsync(strSql);
                return count > 0 ? new { success = true, message = "操作成功", fails } : new { success = false, message = "操作失败" };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new { success = false, code = 500, ex.Message };
            }
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Export(string account)
        {
            var salaries = dics[account];
            if (salaries == null || salaries.Count == 0)
                return BadRequest("导出数据不能为空");

            var personalSalaries = new List<PersonalSalary>();
            salaries.ForEach(x =>
            {
                personalSalaries.Add(new PersonalSalary
                {
                    WorkNo = x.WorkNo,
                    IdCard = x.IdCard,
                    Name = x.Name,
                    Department = x.Department,
                    Company = x.Company,
                    Month = x.Month,
                    Group = x.Group,
                    Duty = x.Duty,
                    JoinTime = x.JoinTime,
                    Type = x.Type,
                    AttendanceDays = x.AttendanceDays,
                    RequireAttendanceDays = x.RequireAttendanceDays,
                    AbsenceDays = x.AbsenceDays,
                    WorkTime1 = x.WorkTime1,
                    WorkTime2 = x.WorkTime2,
                    WorkTime3 = x.WorkTime3,
                    NightDays = x.NightDays,
                    SynthesisSalary = x.SynthesisSalary,
                    BasicSalary = x.BasicSalary,
                    PositionSalary = x.PositionSalary,
                    ConfidentialitySalary = x.ConfidentialitySalary,
                    PerformanceSalary = x.PerformanceSalary,
                    PerformanceAward = x.PerformanceAward,
                    PerformanceDeduct = x.PerformanceDeduct,
                    SenioritySubsidy = x.SenioritySubsidy,
                    HousingSubsidy = x.HousingSubsidy,
                    PhoneChargeSubsidy = x.PhoneChargeSubsidy,
                    NightSubsidy = x.NightSubsidy,
                    PositionSubsidy = x.PositionSubsidy,
                    OtherSubsidy = x.OtherSubsidy,
                    WelfareTotal = x.WelfareTotal,
                    GrowthAward = x.GrowthAward,
                    ProductionAward = x.ProductionAward,
                    QualityAward = x.QualityAward,
                    OtherAward = x.OtherAward,
                    AwardTotal = x.AwardTotal,
                    Enhanced = x.Enhanced,
                    OverTimePay = x.OverTimePay,
                    AbsenceDeduction = x.AbsenceDeduction,
                    BudgetSalary = x.BudgetSalary,
                    SocialSecurity = x.SocialSecurity,
                    ProvidentFund = x.ProvidentFund,
                    PersionalSocialSecurity = x.PersionalSocialSecurity,
                    PersionalProvidentFund = x.PersionalProvidentFund,
                    Utilities = x.Utilities,
                    RepairAttendance = x.RepairAttendance,
                    PersionalTax = x.PersionalTax,
                    Other = x.Other,
                    DeductionTotal = x.DeductionTotal,
                    PracticalSalary = x.PracticalSalary
                });
            });
            dics.Remove(account);
            var memoryStream = new MemoryStream();
            memoryStream.SaveAs(personalSalaries);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") 
            { 
                FileDownloadName = $"工资条{DateTime.Now.Date:yyyy-MM-dd}.xlsx"
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        private async Task<dynamic> Staff(string loginName)
        {
            string oaUrl = configHelper.GetAppSettings<string>("OA:OaUrl");
            string tokenUrl = configHelper.GetAppSettings<string>("OA:TokenUrl");
            string restName = configHelper.GetAppSettings<string>("OA:RestName");
            string password = configHelper.GetAppSettings<string>("OA:Password");
            string postUrl = oaUrl + tokenUrl.Replace("@restName", restName).Replace("@password", password).Replace("@loginName", loginName);
            dynamic responseContent = await httpUtil.GetAsync<dynamic>(postUrl, new Dictionary<string, string> 
            {
                { "Accept", "application/json" }
            });
            //var identity = new ClaimsIdentity(new List<Claim>
            //{
            //    new("userId", responseContent.bindingUser.id),
            //    new("userCode", responseContent.bindingUser.loginName),
            //    new("userName", responseContent.bindingUser.name),
            //    new("userToken", responseContent.id)
            //});
            //httpContextAccessor.HttpContext!.User = new ClaimsPrincipal(identity);
            return new
            {
                responseContent.bindingUser.id,
                responseContent.bindingUser.loginName,
                responseContent.bindingUser.name,
                token = responseContent.id
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private string FullDepartment(string departmentId, string token)
        {
            string departmentUrl = configHelper.GetAppSettings<string>("OA:DepartmentUrl");
            var department = httpUtil.GetAsync<dynamic>(departmentUrl + departmentId, new Dictionary<string, string>
            {
                { "token",token }
            }).Result;
            return department.wholeName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        private void RemoveExists(string code, string month)
        {
            var sqlParams = new List<SqlParameter>
            {
                new ("@code", code),
                new ("@month", month)
            };
            var model = dbContext.Salaries.FromSqlRaw("select * from Salary where WorkNo=@code and Month=@month", sqlParams.ToArray()).FirstOrDefault();
            if (model != null)
                dbContext.Database.ExecuteSqlRaw("delete Salary where WorkNo=@code and Month=@month", sqlParams.ToArray());
        }
    }
}
