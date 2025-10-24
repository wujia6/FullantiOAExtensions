using System.ComponentModel;

namespace FullantiOAExtensions.Core.Models.HR
{
    /// <summary>
    /// 家庭成员
    /// </summary>
    public class FamilyMember
    {
        [Description("家庭情况-姓名")]
        public string Name { get; set; }
        
        [Description("家庭情况-关系")]
        public string Relation { get; set; }

        [Description("家庭情况-年龄")]
        public string Age { get; set; }

        [Description("家庭情况-电话号码")]
        public string PhoneNumber { get; set; }
    }
}
