using System.ComponentModel;

namespace FullantiOAExtensions.Core.Models.HR
{
    /// <summary>
    /// 应聘人员
    /// </summary>
    public class Personnel
    {
        [Description("是否第一次参加本公司的招聘")]
        public string FirstInterview { get; set; }

        [Description("详述")]
        public string? Explain { get; set; }

        [Description("招聘信息")]
        public string InformationChannel { get; set; }
        
        [Description("应聘部门")]
        public string Department { get; set; }
        
        [Description("岗位")]
        public string Position { get; set; }

        //[Description("职务级别")]
        //public string? Level { get; set; }
        
        [Description("姓名")]
        public string UserName { get; set; }
        
        [Description("性别")]
        public string Gender { get; set; }
        
        [Description("出生年月")]
        public DateTime Birthday { get; set; }
        
        [Description("籍贯")]
        public string Origin { get; set; }
        
        [Description("民族")]
        public string Ethnic { get; set; }

        [Description("政治面貌")]
        public string PoliticsStatus { get; set; }

        [Description("最高学历")]
        public string Qualification { get; set; }
        
        [Description("专业")]
        public string Professional { get; set; }
        
        [Description("所获专业职称")]
        public string ProfessionalTitle { get; set; }
        
        [Description("身份证号码")]
        public string CardNumber { get; set; }
        
        [Description("联系电话")]
        public string Phone { get; set; }
        
        [Description("婚育情况")]
        public string Married { get; set; }
        
        [Description("入职前工作年限")]
        public string Seniority { get; set; }
        
        [Description("上班路线")]
        public string Traffic { get; set; }
        
        [Description("户籍所在地")]
        public string OriginAddress { get; set; }
        
        [Description("现居住地址")]
        public string FamilyAddress { get; set; }
        
        [Description("紧急联系人")]
        public string EmergencyContact { get; set; }
        
        [Description("紧急联系电话")]
        public string EmergencyContactPhone { get; set; }

        [Description("填表日期")]
        public DateTime CreateTime { get; set; }

        [Description("兴趣爱好")]
        public string Hobbies { get; set; }

        [Description("专长")]
        public string Specialty { get; set; }
        
        [Description("能否接受加班安排")]
        public string ExtraWork { get; set; }
        
        [Description("能接受加班原因")]
        public string? Explain1 { get; set; }
        
        [Description("能否愿意接受工作调动")]
        public string JobTransfer { get; set; }
        
        [Description("接受工作调动原因")]
        public string? Explain2 { get; set; }
        
        [Description("是否有过重大疾病")]
        public string Disease { get; set; }
        
        [Description("重大疾病原因")]
        public string? Explain3 { get; set; }
        
        [Description("是否有刑事罪或失信人")]
        public string Crime { get; set; }
        
        [Description("刑事罪原因")]
        public string? Explain4 { get; set; }
        
        [Description("我们能联系您现雇主吗")]
        public string ContactBoss { get; set; }
        
        [Description("期望工资")]
        public string Salary { get; set; }
        
        [Description("最快到岗日期")]
        public DateTime ArrivalDate { get; set; }
        
        [Description("是否与前雇主签订任何含经济补偿条款的培训协议")]
        public string FinancialCompensation { get; set; }
        
        [Description("是否与前雇主签订含有竞业限制条款的劳动合同")]
        public string CompetitiveRestriction { get; set; }
        
        [Description("上传照片")]
        public string? Picture { get; set; }

        [Description("本人签名")]
        public string Signature { get; set; }

        [Description("学历档案")]
        public string EducationCertificate { get; set; }

        [Description("身份证照片附件")]
        public string IdCard { get; set; }


        public string EducationHistory { get; set; }

        public string FamilyMembers { get; set; }

        public string? WorkExperiences { get; set; }
    }
}
