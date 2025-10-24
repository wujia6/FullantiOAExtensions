using System.ComponentModel;

namespace FullantiOAExtensions.Core.Models.HR
{
    /// <summary>
    /// 工作经验
    /// </summary>
    public class WorkExperience
    {
        [Description("工作经验-起止年月")]
        public string BeginEnd { get; set; }

        [Description("工作经验-工作单位")]
        public string WorkUnit { get; set; }

        [Description("工作经验-职位")]
        public string Duty { get; set; }

        [Description("工作经验-联系电话")]
        public string PhoneNumber { get; set; }
    }
}
