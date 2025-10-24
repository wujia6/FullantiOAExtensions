using FullantiOAExtensions.Core.Database.Entities;
using FullantiOAExtensions.Core.Database;
using FullantiOAExtensions.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using System.Text;
using Newtonsoft.Json;
using System.Web;
using FullantiOAExtensions.Net.WebApi.Model;
using System.Reflection;
using System.Data;

namespace FullantiOAExtensions.Net.WebApi.Controllers
{
    /// <summary>
    /// 考勤接口
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KQController : ControllerBase
    {

        private readonly HttpUtil httpUtil;
        private readonly ConfigHelper configHelper;
        private readonly OaExtendDbContext dbContext;
        private static Dictionary<string, List<Attendance_details>> adir = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpUtil"></param>
        /// <param name="configHelper"></param>
        /// <param name="dbContext"></param>
        public KQController(HttpUtil httpUtil, ConfigHelper configHelper, OaExtendDbContext dbContext)
        {
            this.httpUtil = httpUtil;
            this.configHelper = configHelper;
            this.dbContext = dbContext;
        }

        /// <summary>
        /// 考勤查询
        /// </summary>
        /// <param name="Departments"></param>
        /// <param name="Name"></param>
        /// <param name="WorkNo"></param>      
        /// <param name="Month"></param>
        /// <param name="StatusChecker"></param>
        /// <param name="Create_WorkNo"></param>
        /// <param name="salary_type"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<dynamic> SearchKQ(string Create_WorkNo, string? Departments = null, string? Name = null, string? WorkNo = null, string? Month = null, string? StatusChecker = null, string? salary_type = null)
        {

            adir.Remove(Create_WorkNo);
            var sqlParams = new List<SqlParameter>();
            string strSql = "select * from Attendance_details where 1=1";
            if (!string.IsNullOrEmpty(Departments))
            {
                strSql += " and Departments = @Departments";
                sqlParams.Add(new SqlParameter("@Departments", Departments));
            }
            if (!string.IsNullOrEmpty(Name))
            {
                strSql += " and Name =@Name";
                sqlParams.Add(new SqlParameter("@Name", Name));
            }
            if (!string.IsNullOrEmpty(WorkNo))
            {
                strSql += " and WorkNo=@WorkNo";
                sqlParams.Add(new SqlParameter("@WorkNo", WorkNo));
            }
            if (!string.IsNullOrEmpty(Month))
            {
                strSql += " and Month=@Month";
                sqlParams.Add(new SqlParameter("@Month", Month));
            }
            if (!string.IsNullOrEmpty(StatusChecker))
            {
                strSql += " and StatusChecker=@StatusChecker";
                sqlParams.Add(new SqlParameter("@StatusChecker", StatusChecker));
            }
            if (!string.IsNullOrEmpty(Create_WorkNo))
            {
                strSql += " and Create_WorkNo=@Create_WorkNo";
                sqlParams.Add(new SqlParameter("@Create_WorkNo", Create_WorkNo));
            }
            if (!string.IsNullOrEmpty(salary_type))
            {
                strSql += " and salary_type=@salary_type";
                sqlParams.Add(new SqlParameter("@salary_type", salary_type));
            }
            var attendances = await dbContext.Attendance_Details.FromSqlRaw(strSql, sqlParams.ToArray()).ToListAsync();
            adir.Add(Create_WorkNo, attendances);
            return attendances;
        }

        /// <summary>
        /// 月度考情汇总明细导入
        /// </summary>
        /// <param name="formFile">excel</param>
        /// <param name="Create_WorkNo"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<dynamic> ImportKQ(IFormFile formFile, string Create_WorkNo)
        {
            if (formFile == null)
                return new { success = false, message = "上传文件不能为空" };

            string[] arr = { ".xls", ".xlsx" };
            if (!arr.Contains(Path.GetExtension(formFile.FileName)))
                return new { success = false, message = "只能是excel文件" };

            try
            {
                //读取excel
                using var stearm = formFile.OpenReadStream();
                var Attendance = stearm.Query<Attendance_details>(startCell: "A1").ToList();
                if (Attendance == null || Attendance.Count == 0)
                    return new { success = false, message = "上传数据不能为空" };


                string cleanedDateStr = Attendance[0].Day1.Substring(0, Attendance[0].Day1.IndexOf('('));
                // 解析日期
                DateTime date = DateTime.Parse(cleanedDateStr);

                string month = date.ToString("yyyy-M");
                int day = date.Day;
                if (day != 1)
                {
                    return new { success = false, message = "上传数据不是完整月份的数据，请重新导入！" };
                }
                //构建sqlvalues
                var fields = new StringBuilder();

                foreach (var x in Attendance)
                {
                    if (x is null || string.IsNullOrEmpty(x.Name) || x.Name.Equals("人员姓名"))
                    {
                        continue;
                    }
                    RemoveExistsKQ(x.WorkNo, month,Create_WorkNo);
                    x.CreateTime = DateTime.Now.ToString("yyyy-MM-dd");
                    x.Month = month;
                    x.StatusChecker = "未推送";
                    x.Create_WorkNo = Create_WorkNo;
                    fields.Append("('" + x.Name + "',");
                    fields.Append("'" + x.WorkNo + "',");
                    fields.Append("'" + x.DepartmentsLevel + "',");
                    fields.Append("'" + x.Departments + "',");
                    fields.Append("'" + x.Post + "',");
                    fields.Append("'" + x.AttendanceDays + "',");
                    fields.Append("'" + x.ActualAttendanceDays + "',");
                    fields.Append("'" + x.AbsenceDays + "',");
                    fields.Append("'" + x.BusinessTripDays + "',");
                    fields.Append("'" + x.StatutoryHolidayDays + "',");
                    fields.Append("'" + x.WorkInjuryDays + "',");
                    fields.Append("'" + x.MissingCardCount + "',");
                    fields.Append("'" + x.MakeUpCardCount + "',");
                    fields.Append("'" + x.LateMinutes + "',");
                    fields.Append("'" + x.EarlyLeaveMinutes + "',");
                    fields.Append("'" + x.CompensatoryLeaveDays + "',");
                    fields.Append("'" + x.AnnualLeaveDays + "',");
                    fields.Append("'" + x.SickLeaveDays + "',");
                    fields.Append("'" + x.PersonalLeaveDays + "',");
                    fields.Append("'" + x.MaternityLeaveDays + "',");
                    fields.Append("'" + x.MarriageLeaveDays + "',");
                    fields.Append("'" + x.FuneralLeaveDays + "',");
                    fields.Append("'" + x.FamilyVisitLeaveDays + "',");
                    fields.Append("'" + x.TotalLeaveDays + "',");
                    fields.Append("'" + x.Day1 + "',");
                    fields.Append("'" + x.Day2 + "',");
                    fields.Append("'" + x.Day3 + "',");
                    fields.Append("'" + x.Day4 + "',");
                    fields.Append("'" + x.Day5 + "',");
                    fields.Append("'" + x.Day6 + "',");
                    fields.Append("'" + x.Day7 + "',");
                    fields.Append("'" + x.Day8 + "',");
                    fields.Append("'" + x.Day9 + "',");
                    fields.Append("'" + x.Day10 + "',");
                    fields.Append("'" + x.Day11 + "',");
                    fields.Append("'" + x.Day12 + "',");
                    fields.Append("'" + x.Day13 + "',");
                    fields.Append("'" + x.Day14 + "',");
                    fields.Append("'" + x.Day15 + "',");
                    fields.Append("'" + x.Day16 + "',");
                    fields.Append("'" + x.Day17 + "',");
                    fields.Append("'" + x.Day18 + "',");
                    fields.Append("'" + x.Day19 + "',");
                    fields.Append("'" + x.Day20 + "',");
                    fields.Append("'" + x.Day21 + "',");
                    fields.Append("'" + x.Day22 + "',");
                    fields.Append("'" + x.Day23 + "',");
                    fields.Append("'" + x.Day24 + "',");
                    fields.Append("'" + x.Day25 + "',");
                    fields.Append("'" + x.Day26 + "',");
                    fields.Append("'" + x.Day27 + "',");
                    fields.Append("'" + x.Day28 + "',");
                    fields.Append("'" + x.Day29 + "',");
                    fields.Append("'" + x.Day30 + "',");
                    fields.Append("'" + x.Day31 + "',");
                    fields.Append("'" + x.Month + "',");
                    fields.Append("'" + x.OvertimeTotalDuration + "',");
                    fields.Append("'" + x.WeekdayOvertimeToPay + "',");
                    fields.Append("'" + x.HolidayOvertimeToPay + "',");
                    fields.Append("'" + x.HolidayToOvertimeFlag + "',");
                    fields.Append("'" + x.WeekdayOvertimeToCompensatoryLeave + "',");
                    fields.Append("'" + x.WeekendOvertimeToCompensatoryLeave + "',");
                    fields.Append("'" + x.CreateTime + "',");
                    fields.Append("'" + x.StatusChecker + "',");
                    fields.Append("'" + x.HolidayOvertimeToCompensatoryLeave + "',");
                    fields.Append("'" + x.Create_WorkNo + "',");
                    fields.Append("'" + x.salary_type + "',");
                    if (string.IsNullOrEmpty(x.Total_night_shift_days))
                    {
                        fields.Append("'0',");
                    }
                    else
                    {
                        fields.Append("'" + x.Total_night_shift_days + "',");
                    }
                    if (string.IsNullOrEmpty(x.Total_middle_shift_days))
                    {
                        fields.Append("'0'");
                    }
                    else
                    {
                        fields.Append("'" + x.Total_middle_shift_days + "'");
                    }

                    fields.Append("),");
                }
                string strSql = string.Format("insert into Attendance_details values{0}", fields.ToString().TrimEnd(','));
                int count = await dbContext.Database.ExecuteSqlRawAsync(strSql);
                return count > 0 ? new { success = true, message = "操作成功" } : new { success = false, message = "操作失败" };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new { success = false, code = 500, ex.Message };
            }
        }



        /// <summary>
        /// 月度加班明细导入
        /// </summary>
        /// <param name="formFile">excel</param>
        /// <param name="Create_WorkNo"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<dynamic> ImportKQNightDays(IFormFile formFile, string Create_Time)
        {
            if (formFile == null)
                return new { success = false, message = "上传文件不能为空" };

            string[] arr = { ".xls", ".xlsx" };
            if (!arr.Contains(Path.GetExtension(formFile.FileName)))
                return new { success = false, message = "只能是excel文件" };

            try
            {
                //读取excel
                using var stearm = formFile.OpenReadStream();
                var Attendance = stearm.Query<AttendanceNightDays>(startCell: "A1").ToList();
                if (Attendance == null || Attendance.Count == 0)
                    return new { success = false, message = "上传数据不能为空" };
                foreach (var x in Attendance)
                {
                    if (x.Name.Equals("人员姓名"))
                    {
                        continue;
                    }
                    string sqlstr = "select * from Attendance_details where Month=@month and WorkNo=@workNo";
                    //int result=await dbContext.Database.ExecuteSqlRawAsync(sqlstr);
                    var source = dbContext.Attendance_Details.FromSqlRaw(sqlstr, new List<SqlParameter>
                    {
                        new("@month",Create_Time),
                        new("@workNo", x.WorkNo)
                    }.ToArray()).FirstOrDefault();
                    if (source != null && source.Id > 0)
                    {
                        string sqlstr1 = string.Format("update Attendance_details set total_night_shift_days='{0}' where id={1}", x.total_night_shift_days.ToString(), source.Id);
                        int result = await dbContext.Database.ExecuteSqlRawAsync(sqlstr1);
                    }
                }
                return new { success = true, message = "操作成功" };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new { success = false, code = 500, ex.Message };
            }
        }

        /// <summary>
        /// 考情确认单导出
        /// </summary>
        /// <param name="Create_WorkNo"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ExportKQ(string Create_WorkNo)
        {
            var attendances = adir[Create_WorkNo];
            if (attendances == null || attendances.Count == 0)
                return BadRequest("导出数据不能为空");
            var memoryStream = new MemoryStream();
            memoryStream.SaveAs(attendances);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = $"考情确认单{DateTime.Now.Date:yyyy-MM-dd}.xlsx"
            };
        }

        /// <summary>
        /// 推送OA消息
        /// </summary>
        /// <param name="paramList">JSON集合</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<dynamic> SendOAKQ([FromBody] List<Attendance_details> paramList)
        {
            if (paramList == null || paramList.Count == 0)
                return new { success = false, message = "参数不能为空" };
            try
            {
                string Create_workNo = paramList[0].Create_WorkNo;
                string sqlStr1 = "select * from AutoSettings where Create_WorkNo=@Create_WorkNo";
                var sqlParams = new List<SqlParameter>
            {
                new ("@Create_WorkNo", Create_workNo)
            };
                var st = await dbContext.AutoSettings.FromSqlRaw(sqlStr1, sqlParams.ToArray()).FirstOrDefaultAsync();

                //生成消息体
                var codeList = paramList.Select(src => new { src.WorkNo, src.Month }).ToList();
                dynamic staff = await Staff(paramList[0].Create_WorkNo);
                //string codes = string.Empty;
                var messages = new List<Dictionary<string, object>>();
                codeList.ForEach(src =>
                {
                    long messageId = DateTime.Now.Ticks;
                    string code = EncryptHelper.DesEncrypt(src.WorkNo.Trim());
                    string htmlUrl = $"http://110.52.91.35:10020/OA/kaoqing_qr.html?code={HttpUtility.UrlEncode(code)}&month={src.Month}";
                    var msg = new Dictionary<string, object>
                    {
                        { "thirdpartyRegisterCode",3005 },
                        { "thirdpartyMessageId", messageId },
                        { "messageContent", "考勤确认：您的" + src.Month + "月份考勤汇总！如有疑问，可在考勤确认单发放后3个工作日内联系相关人员进行处理,系统将在本月"+st.Day+"日12:00:00后自动确认。" },
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
                    //codes += "'" + src.WorkNo + "',";
                });
                //发送请求
                string oaUrl = configHelper.GetAppSettings<string>("OA:OaUrl");
                string messageUrl = configHelper.GetAppSettings<string>("OA:SingleMessageUrl");
                int success = 0;
                var fails = new List<string>();
                foreach (var msg in messages)
                {
                    dynamic response = await httpUtil.PostAsync<dynamic>(oaUrl + messageUrl, msg, new Dictionary<string, string>
                    {
                        { "token", staff.token.ToString() }
                    });
                    string workNo = msg["thirdpartyReceiverId"].ToString()!;
                    string responseText = JsonConvert.SerializeObject(response);

                    if (responseText.Contains("true"))
                    {
                        string sqlStr = $"update Attendance_details set StatusChecker='已推送' where Month='{codeList.FirstOrDefault()!.Month}' and WorkNo='{workNo}'";
                        int res = dbContext.Database.ExecuteSqlRaw(sqlStr);
                        success++;
                    }
                    else
                    {
                        string sqlStr = $"update Attendance_details set StatusChecker='推送失败' where Month='{codeList.FirstOrDefault()!.Month}' and WorkNo='{workNo}'";
                        int res = dbContext.Database.ExecuteSqlRaw(sqlStr);
                        fails.Add(workNo);
                    }

                }
                return new { success, fails };
            }
            catch (Exception ex)
            {
                return new { success = false, code = 500, message = ex.Message };
            }
        }

        /// <summary>
        /// 考勤确认
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<dynamic> UpdateAttendanceState(int id)
        {
            string strSql = string.Format("update Attendance_details set StatusChecker='已确认' where id={0}", id);
            int count = await dbContext.Database.ExecuteSqlRawAsync(strSql);
            if (count > 0)
            {
                return new { success = true, message = "考勤确认成功" };
            }
            return new { success = false, message = "考勤确认失败" };
        }

        /// <summary>
        /// 查询个人考勤
        /// </summary>
        /// <param name="code">加密工号</param>
        /// <param name="month">月份</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<dynamic> GetAttendance(string code, string month)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(month))
                return new { success = false, message = "参数错误" };
            code = EncryptHelper.DesDecrypt(code);
            string sqlStr = "select * from Attendance_details where WorkNo=@code and Month=@month";
            var sqlParams = new List<SqlParameter>
            {
                new ("@code", code),
                new ("@month", month)
            };
            return await dbContext.Attendance_Details.FromSqlRaw(sqlStr, sqlParams.ToArray()).FirstOrDefaultAsync();

        }

        /// <summary>
        /// 删除指定考情确认记录
        /// </summary>
        /// <param name="workNo"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        private void RemoveExistsKQ(string workNo, string month,string createWokNo)
        {
            var sqlParams = new List<SqlParameter>
            {
                new ("@workNo", workNo),
                new ("@month",month),
                new ("@Create_WorkNo",createWokNo)
            };
            var model = dbContext.Attendance_Details.FromSqlRaw("select * from Attendance_details where WorkNo=@workNo  and month=@month and Create_WorkNo=@Create_WorkNo", sqlParams.ToArray()).FirstOrDefault();
            if (model != null)
                dbContext.Database.ExecuteSqlRaw("delete Attendance_details where WorkNo=@workNo and Month=@month and Create_WorkNo=@Create_WorkNo", sqlParams.ToArray());
        }


        /// <summary>
        /// 判断该工号的自动确认参数是否存在
        /// </summary>
        /// <param name="WorkNo"></param>
        /// <returns></returns>
        private bool isExitesWorkNo(string WorkNo)
        {

            string sqlstr = "select * from AutoSettings where Create_WorkNo=@Create_WorkNo";
            var source = dbContext.AutoSettings.FromSqlRaw(sqlstr, new List<SqlParameter>
                    {
                        new("@Create_WorkNo",WorkNo)
                    }.ToArray()).FirstOrDefault();

            if (source != null && source.id> 0)
            {
                return true;
            }
            return false;

        }

        /// <summary>
        /// 修改自动确认参数
        /// </summary>
        /// <param name="param">JSON对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<dynamic> UpdateAutoSettings([FromBody] AutoSettings param)
        {
            int count = 0;
            if (param == null || string.IsNullOrEmpty(param.Create_WorkNo) || param.Day <= 0) return new { success = false, message = "参数错误" };
            bool result = isExitesWorkNo(param.Create_WorkNo);
            if (result)
            {
                string sqlstr = "update AutoSettings set AutoEnsure=@AutoEnsure,Day=@Day where Create_WorkNo=@Create_WorkNo";
                SqlParameter[] sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("@AutoEnsure", param.AutoEnsure);
                sqlParameters[1] = new SqlParameter("@Day", param.Day);
                sqlParameters[2] = new SqlParameter("@Create_WorkNo", param.Create_WorkNo);
                count = await dbContext.Database.ExecuteSqlRawAsync(sqlstr, sqlParameters.ToArray());

            }
            else
            {
                string sqlstr = "insert into AutoSettings(AutoEnsure,Day,Create_WorkNo)values(@AutoEnsure,@Day,@Create_WorkNo)";
                SqlParameter[] sqlParameters = new SqlParameter[3];
                sqlParameters[0] = new SqlParameter("@AutoEnsure", param.AutoEnsure);
                sqlParameters[1] = new SqlParameter("@Day", param.Day);
                sqlParameters[2] = new SqlParameter("@Create_WorkNo", param.Create_WorkNo);
                count = await dbContext.Database.ExecuteSqlRawAsync(sqlstr, sqlParameters.ToArray());

            }
            return count > 0 ? new { success = true, message = "编辑成功" } : new { success = false, message = "编辑失败" };

        }

        /// <summary>
        /// 查询登陆账号的自动确认参数
        /// </summary>
        /// <param name="WorkNo">登陆工号</param>       
        /// <returns></returns>
        [HttpGet]
        public async Task<dynamic> GetAutoSettings(string WorkNo)
        {
            if (string.IsNullOrEmpty(WorkNo))
                return new { success = false, message = "参数错误" };           
            string sqlStr = "select * from AutoSettings where Create_WorkNo=@Create_WorkNo";
            var sqlParams = new List<SqlParameter>
            {
                new ("@Create_WorkNo", WorkNo)
            };
            return await dbContext.AutoSettings.FromSqlRaw(sqlStr, sqlParams.ToArray()).FirstOrDefaultAsync();

        }      


        /// <summary>
        /// 修改考勤记录
        /// </summary>
        /// <param name="param">JSON对象</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<dynamic> UpdateAttendanceDetails([FromBody] Attendance_details param)
        {
            if (param == null || param.Id < 0) return new { success = false, message = "参数错误" };

            StringBuilder strSqlBuilder = new StringBuilder("update Attendance_details set ");
            List<SqlParameter> sqlParams = new List<SqlParameter>();

            // 获取Attendance_details类的所有属性
            PropertyInfo[] properties = typeof(Attendance_details).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                // 跳过Id属性，因为它是用于定位记录的，不参与值是否改变的判断
                if (property.Name == "Id") continue;

                // 获取属性的值
                object value = property.GetValue(param);

                // 根据属性类型判断默认值情况并决定是否添加到更新语句和参数列表
                if (property.PropertyType == typeof(string) && !string.IsNullOrEmpty((string)value))
                {
                    var PName = property.Name;
                    if (PName == "StatusChecker" && value.ToString().Equals("已确认"))
                    {
                        strSqlBuilder.Append($"{property.Name}=@{property.Name},");
                        sqlParams.Add(new($"@{property.Name}", "已推送"));
                    }
                    else
                    {
                        strSqlBuilder.Append($"{property.Name}=@{property.Name},");
                        sqlParams.Add(new($"@{property.Name}", value));
                    }

                }
                else if (property.PropertyType.IsValueType &&
                         (value != null && !EqualityComparer<object>.Default.Equals(value, Activator.CreateInstance(property.PropertyType))))
                {
                    strSqlBuilder.Append($"{property.Name}=@{property.Name},");
                    sqlParams.Add(new($"@{property.Name}", value));
                }
            }

            // 判断是否有要更新的字段添加到了SQL语句中，如果没有则返回参数错误提示
            if (strSqlBuilder.Length <= "update Attendance_details set ".Length)
            {
                return new { success = false, message = "没有要更新的有效字段，请检查传入参数" };
            }

            // 去除最后多余的逗号
            strSqlBuilder.Remove(strSqlBuilder.Length - 1, 1);

            // 添加条件语句用于定位要更新的记录
            strSqlBuilder.Append(" where id=@id");
            sqlParams.Add(new("@id", param.Id));

            string strSql = strSqlBuilder.ToString();

            int count = await dbContext.Database.ExecuteSqlRawAsync(strSql, sqlParams.ToArray());
            return count > 0 ? new { success = true, message = "修改成功" } : new { success = false, message = "修改失败" };
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


    }
}
