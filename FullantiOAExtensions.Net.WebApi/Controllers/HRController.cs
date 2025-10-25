using System.ComponentModel;
using FullantiOAExtensions.Core.Models.HR;
using FullantiOAExtensions.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace FullantiOAExtensions.Net.WebApi.Controllers
{
    /// <summary>
    /// 人事接口
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class HRController : ControllerBase
    {
        private readonly HttpUtil httpUtil;
        private readonly HttpFile httpFile;
        private readonly IWebHostEnvironment webHostEnvironment;

        private readonly string groupId = "670869647114347";
        private readonly string oaUrl = "http://hn.fldcloud.com:8088/seeyon/rest";
        private readonly string tokenUrl = "/token/@restName/@password?loginName=@loginName";
        private readonly string flowUrl = "/bpm/process/start";
        private readonly string restName = "wujia";
        private readonly string password = "2f892ec2-dc64-48cb-841b-f2a1f1e36826";
        private readonly string uploadUrl = "/attachment?token=@token&applicationCategory=0&extensions=&firstSave=true";

        /// <summary>
        /// ioc
        /// </summary>
        /// <param name="httpUtil"></param>
        /// <param name="httpFile"></param>
        /// <param name="webHostEnvironment"></param>
        public HRController(HttpUtil httpUtil, HttpFile httpFile, IWebHostEnvironment webHostEnvironment)
        {
            this.httpUtil = httpUtil;
            this.httpFile = httpFile;
            this.webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> Token(string loginName)
        {
            string postUrl = oaUrl + tokenUrl.Replace("@restName", restName).Replace("@password", password).Replace("@loginName", loginName);
            dynamic responseContent = await httpUtil.GetAsync<dynamic>(postUrl, new Dictionary<string, string> 
            {
                { "Accept", "application/json" }
            });
            return responseContent.id.ToString();
        }

        /// <summary>
        /// 获取部门
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<dynamic> Departments(string token, string? keyword = null)
        {
            string url = $"{oaUrl}/orgDepartments/{groupId}";
            var source = await httpUtil.GetAsync<List<dynamic>>(url, new Dictionary<string, string>
            {
                { "Accept", "application/json" },
                { "token", token.Trim('"') }
            });
            source.ToList().ForEach(x => x.id = x.id.ToString());
            source = source.FindAll(x => !x.name.ToString().Contains("组") && !x.name.ToString().Contains("专员") && !x.name.ToString().Contains("助理"));
            dynamic result;
            if(string.IsNullOrEmpty(keyword))
                result = Recurve(source, groupId);
            else
                result = source.FindAll(x => x.name.ToString().Contains(keyword)).Select(y => new { y.id, y.name });
            return result!;
        }

        /// <summary>
        /// 获取岗位
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<dynamic> Poistions(string token, string? keyword=null)
        {
            string url = $"{oaUrl}/orgPosts/{groupId}";
            var source = await httpUtil.GetAsync<List<dynamic>>(url, new Dictionary<string, string>
            {
                { "Accept", "application/json" },
                { "token", token.Trim('"') }
            });
            source.ToList().ForEach(x => x.id = x.id.ToString());
            dynamic result = source.Select(y => new { y.id, y.name });
            if (!string.IsNullOrEmpty(keyword))
                result = source.FindAll(x => x.name.ToString().Contains(keyword)).Select(y => new { y.id, y.name });
            return result!;
        }

        /// <summary>
        /// 获取职级
        /// </summary>
        /// <param name="token">token</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<dynamic> Levels(string token)
        {
            string url = $"{oaUrl}/orgLevels/{groupId}";
            var resp = await httpUtil.GetAsync<List<dynamic>>(url, new Dictionary<string, string>
            {
                { "Accept", "application/json" },
                { "token", token.Trim('"') }
            });
            resp.ForEach(x => x.id = x.id.ToString());
            return resp;
        }

        /// <summary>
        /// 获取人员信息
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="workNo">工号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<dynamic> Persionnel(string token, string workNo)
        {
            string url = $"{oaUrl}/orgMembers/code/{workNo}";
            var resp = await httpUtil.GetAsync<dynamic>(url, new Dictionary<string, string>
            {
                { "Accept", "application/json" },
                { "token", token.Trim('"') }
            });
            return resp;
        }

        /// <summary>
        /// 应聘人员信息登记
        /// </summary>
        /// <param name="model">表单</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<dynamic> Registry([FromForm] Personnel model)
        {
            string templateCode = "HR_YPDJB",   //模板编号
                formMain29 = "formmain_0029",   //应聘登记主表
                formSon30 = "formson_0030",     //教育背景子表
                formSon37 = "formson_0037",     //家庭成员子表
                formSon38 = "formson_0038";     //工作经验子表

            try
            {
                string token = await Token("RZ00001");  //Rz00001 Fld@rz00001

                #region 请求构建
                //重置键值对
                model.FirstInterview = model.FirstInterview.Insert(0, "name|");
                model.Gender = model.Gender.Insert(0, "name|");
                model.Qualification = model.Qualification.Insert(0, "name|");
                model.PoliticsStatus = model.PoliticsStatus.Insert(0, "name|");
                model.Married = model.Married.Insert(0, "name|");
                model.ExtraWork = model.ExtraWork.Insert(0, "name|");
                model.JobTransfer = model.JobTransfer.Insert(0, "name|");
                model.Disease = model.Disease.Insert(0, "name|");
                model.Crime = model.Crime.Insert(0, "name|");
                model.ContactBoss = model.ContactBoss.Insert(0, "name|");
                model.FinancialCompensation = model.FinancialCompensation.Insert(0, "name|");
                model.CompetitiveRestriction = model.CompetitiveRestriction.Insert(0, "name|");

                #region 基础信息
                var personnel = new Dictionary<string, object>();
                foreach (var property in model.GetType().GetProperties())
                {
                    var value = property.GetValue(model);
                    var descriptionAttr = Attribute.GetCustomAttribute(property, typeof(DescriptionAttribute));
                    if (value == null || descriptionAttr == null)
                        continue;
                    string field = ((DescriptionAttribute)descriptionAttr).Description;
                    personnel.Add(field, value);
                }
                #endregion

                #region 附件
                string uploadUri = oaUrl + uploadUrl.Replace("@token", token);
                var thirdAttachments = new List<Dictionary<string, object>>();

                //本人照片
                var file1 = new FileInfo(model.Picture!);
                var longId1 = httpFile.Upload(file1, uploadUri);
                long guid1 = Guid.NewGuid().GetHashCode();
                thirdAttachments.Add(new Dictionary<string, object>
                {
                    { "subReference", guid1 },
                    { "fileUrl", longId1 },
                    { "sort", 1 }
                });
                personnel["上传照片"] = guid1.ToString();

                //本人签名
                string savePath = Path.Combine(webHostEnvironment.ContentRootPath, "Upload");
                string fileName = Guid.NewGuid().ToString() + ".jpg";
                string signature = model.Signature[(model.Signature.IndexOf(',') + 1)..];
                var success = ImgHelp.SaveBase64ToLocation(signature, savePath, fileName);

                if (!success)
                    return new { success = false, message = "本人签名保存失败" };

                var file2 = new FileInfo(Path.Combine(savePath, fileName));
                var longId2 = httpFile.Upload(file2, uploadUri);
                long guid2 = Guid.NewGuid().GetHashCode();
                thirdAttachments.Add(new Dictionary<string, object>
                {
                    { "subReference", guid2 },
                    { "fileUrl", longId2 },
                    { "sort", 2 }
                });
                personnel["本人签名"] = guid2.ToString();

                //学历证书
                var file3 = new FileInfo(model.EducationCertificate);
                var longId3 = httpFile.Upload(file3, uploadUri);
                long guid3 = Guid.NewGuid().GetHashCode();
                thirdAttachments.Add(new Dictionary<string, object>
                {
                    { "subReference", guid3 },
                    { "fileUrl", longId3 },
                    { "sort", 3 }
                });
                personnel["学历档案"] = guid3.ToString();

                //身份证
                long guid4 = Guid.NewGuid().GetHashCode();
                var arys = model.IdCard.Split(',');
                for (int i = 0; i < arys.Length; i++)
                {
                    var fileItem = new FileInfo(arys[i]);
                    var longId = httpFile.Upload(fileItem, uploadUri);
                    thirdAttachments.Add(new Dictionary<string, object>
                    {
                        { "subReference", guid4 },
                        { "fileUrl", longId },
                        { "sort", i + 4 }
                    });
                }
                //var file4 = new FileInfo(model.IdCard);
                //var longId4 = httpFile.Upload(file4, uploadUri);
                //long guid4 = Guid.NewGuid().GetHashCode();
                //thirdAttachments.Add(new Dictionary<string, object>
                //{
                //    { "subReference", guid4 },
                //    { "fileUrl", longId4 },
                //    { "sort", 4 }
                //});
                personnel["身份证照片附件"] = guid4.ToString();
                #endregion

                #region 教育、家庭、工作信息
                //教育经历
                var educationHistorys = new List<Dictionary<string, object>>();
                var educationTemps = JsonConvert.DeserializeObject<List<EducationHistory>>(model.EducationHistory);
                foreach (var temp in educationTemps!)
                {
                    var propertyInfos = typeof(EducationHistory).GetProperties();
                    var dictionary = new Dictionary<string, object>();
                    foreach (var info in propertyInfos)
                    {
                        var attr = Attribute.GetCustomAttribute(info, typeof(DescriptionAttribute));
                        var field = ((DescriptionAttribute)attr!).Description;
                        var value = info.GetValue(temp);
                        dictionary.Add(field, value!);
                    }
                    educationHistorys.Add(dictionary);
                }

                //家庭情况
                var familyMembers = new List<Dictionary<string, object>>();
                var familyTemps = JsonConvert.DeserializeObject<List<FamilyMember>>(model.FamilyMembers);
                foreach(var temp in familyTemps!)
                {
                    var propertyInfos = typeof(FamilyMember).GetProperties();
                    var dictionary = new Dictionary<string, object>();
                    foreach (var info in propertyInfos)
                    {
                        var attr = Attribute.GetCustomAttribute(info, typeof(DescriptionAttribute));
                        var field = ((DescriptionAttribute)attr!).Description;
                        var value = info.GetValue(temp);
                        dictionary.Add(field, value!);
                    }
                    familyMembers.Add(dictionary);
                }

                //工作经历
                var workExperinces = new List<Dictionary<string, object>>();
                if (!string.IsNullOrEmpty(model.WorkExperiences))
                {
                    var workTemps = JsonConvert.DeserializeObject<List<WorkExperience>>(model.WorkExperiences);
                    foreach (var temp in workTemps!)
                    {
                        var propertyInfos = typeof(WorkExperience).GetProperties();
                        var dictionary = new Dictionary<string, object>();
                        foreach (var info in propertyInfos)
                        {
                            var attr = Attribute.GetCustomAttribute(info, typeof(DescriptionAttribute));
                            var field = ((DescriptionAttribute)attr!).Description;
                            var value = info.GetValue(temp);
                            dictionary.Add(field, value!);
                        }
                        workExperinces.Add(dictionary);
                    }
                }
                #endregion

                //构建
                var formData = new Dictionary<string, object> 
                { 
                    { formMain29, personnel },
                    { "thirdAttachments", thirdAttachments },
                    { formSon30, educationHistorys },
                    { formSon37, familyMembers },
                    { formSon38, workExperinces }
                };
                var data = new Dictionary<string, object>
                {
                    { "templateCode", templateCode },
                    { "draft", "0" },
                    { "data", formData },
                    { "subject", "应聘人员信息登记" }
                };
                var requestData = new Dictionary<string, object>
                {
                    { "appName", "collaboration" },
                    { "data", data }
                };
                //var requestJson = JsonConvert.SerializeObject(requestData);
                //请求头
                var requestHeader = new Dictionary<string, string>
                {
                    { "token", token },
                };
                #endregion

                //发送请求
                string postUrl = oaUrl + flowUrl;
                var result = await httpUtil.PostAsync<dynamic>(postUrl, requestData, requestHeader);
                return result;
            }
            catch (Exception ex)
            {
                return new { success = false, message = ex.Message };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        private static dynamic Recurve(List<dynamic> source, string pid)
        {
            if (source == null || source.Count == 0)
                return default!;
            var newList = source.FindAll(x => pid.Equals(x.superior.ToString()));
            if (newList != null && newList.Count > 0)
            {
                for (int i = 0; i < newList.Count; i++)
                {
                    string id = newList[i].id.ToString();
                    var depart = newList[i];
                    depart.id = id;
                    IDictionary<string, object> dic = JsonConvert.DeserializeObject<IDictionary<string, object>>(depart.ToString());
                    var childrens = Recurve(source, depart.id.ToString());
                    dic.Add("childrens", childrens);
                    if (childrens != null && childrens!.Count > 0)
                    {
                        string dicStr = JsonConvert.SerializeObject(dic);
                        var newDepart = JsonConvert.DeserializeObject<dynamic>(dicStr);
                        newList[i] = newDepart!;
                    }
                }
            }
            return newList!;
        }
    }
}
