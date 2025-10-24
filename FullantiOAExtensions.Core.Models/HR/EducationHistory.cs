using System.ComponentModel;

namespace FullantiOAExtensions.Core.Models.HR
{
    /// <summary>
    /// 教育背景
    /// </summary>
    public class EducationHistory
    {
        [Description("教育-起止年月")]
        public string BeginEnd { get; set; }

        [Description("教育-学校")]
        public string School { get; set; }
        
        [Description("教育-专业")]
        public string Professional { get; set; }

        [Description("教育-学习形式")]
        public string StudyMode { get; set; }

        [Description("教育-学历")]
        public string Qualification { get; set; }
    }
}
