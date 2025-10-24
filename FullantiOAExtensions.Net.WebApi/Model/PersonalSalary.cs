using MiniExcelLibs.Attributes;

namespace FullantiOAExtensions.Net.WebApi.Model
{
    public class PersonalSalary
    {
        [ExcelColumnIndex("A")]
        [ExcelColumnName("人员编号")]
        public string? WorkNo { get; set; }

        [ExcelColumnIndex("B")]
        [ExcelColumnName("身份证号")]
        public string? IdCard { get; set; }

        [ExcelColumnIndex("C")]
        [ExcelColumnName("姓名")]
        public string? Name { get; set; }

        [ExcelColumnIndex("D")]
        [ExcelColumnName("所属部门")]
        public string? Department { get; set; }

        [ExcelColumnIndex("E")]
        [ExcelColumnName("所属单位")]
        public string? Company { get; set; }

        [ExcelColumnIndex("F")]
        [ExcelColumnName("月份")]
        public string? Month { get; set; }

        [ExcelColumnIndex("G")]
        [ExcelColumnName("分组")]
        public string? Group { get; set; }

        [ExcelColumnIndex("H")]
        [ExcelColumnName("职位")]
        public string? Duty { get; set; }

        [ExcelColumnIndex("I")]
        [ExcelColumnName("入职时间")]
        public string? JoinTime { get; set; }

        [ExcelColumnIndex("J")]
        [ExcelColumnName("薪资制")]
        public string? Type { get; set; }

        [ExcelColumnIndex("K")]
        [ExcelColumnName("应出勤天数")]
        public string? AttendanceDays { get; set; }

        [ExcelColumnIndex("L")]
        [ExcelColumnName("实出勤天数")]
        public string? RequireAttendanceDays { get; set; }

        [ExcelColumnIndex("M")]
        [ExcelColumnName("缺勤天数"),]
        public string? AbsenceDays { get; set; }

        [ExcelColumnIndex("N")]
        [ExcelColumnName("1.5工时")]
        public string? WorkTime1 { get; set; }

        [ExcelColumnIndex("O")]
        [ExcelColumnName("2.0工时")]
        public string? WorkTime2 { get; set; }

        [ExcelColumnIndex("P")]
        [ExcelColumnName("3.0工时")]
        public string? WorkTime3 { get; set; }

        [ExcelColumnIndex("Q")]
        [ExcelColumnName("夜班天数")]
        public string? NightDays { get; set; }

        [ExcelColumnIndex("R")]
        [ExcelColumnName("综合工资")]
        public string? SynthesisSalary { get; set; }

        [ExcelColumnIndex("S")]
        [ExcelColumnName("基本工资")]
        public string? BasicSalary { get; set; }

        [ExcelColumnIndex("T")]
        [ExcelColumnName("岗位工资")]
        public string? PositionSalary { get; set; }

        [ExcelColumnIndex("U")]
        [ExcelColumnName("保密工资")]
        public string? ConfidentialitySalary { get; set; }

        [ExcelColumnIndex("V")]
        [ExcelColumnName("绩效工资")]
        public string? PerformanceSalary { get; set; }

        [ExcelColumnIndex("W")]
        [ExcelColumnName("绩效奖励")]
        public string? PerformanceAward { get; set; }

        [ExcelColumnIndex("X")]
        [ExcelColumnName("绩效扣除")]
        public string? PerformanceDeduct { get; set; }

        [ExcelColumnIndex("Y")]
        [ExcelColumnName("工龄补贴")]
        public string? SenioritySubsidy { get; set; }

        [ExcelColumnIndex("Z")]
        [ExcelColumnName("租房补贴")]
        public string? HousingSubsidy { get; set; }

        [ExcelColumnIndex("AA")]
        [ExcelColumnName("话费补贴")]
        public string? PhoneChargeSubsidy { get; set; }

        [ExcelColumnIndex("AB")]
        [ExcelColumnName("夜班补贴")]
        public string? NightSubsidy { get; set; }

        [ExcelColumnIndex("AC")]
        [ExcelColumnName("岗位补贴")]
        public string? PositionSubsidy { get; set; }

        [ExcelColumnIndex("AD")]
        [ExcelColumnName("其他补贴")]
        public string? OtherSubsidy { get; set; }

        [ExcelColumnIndex("AE")]
        [ExcelColumnName("福利小计")]
        public string? WelfareTotal { get; set; }

        [ExcelColumnIndex("AF")]
        [ExcelColumnName("成长奖励")]
        public string? GrowthAward { get; set; }

        [ExcelColumnIndex("AG")]
        [ExcelColumnName("生产奖励")]
        public string? ProductionAward { get; set; }

        [ExcelColumnIndex("AH")]
        [ExcelColumnName("品质奖励")]
        public string? QualityAward { get; set; }

        [ExcelColumnIndex("AI")]
        [ExcelColumnName("其他奖励")]
        public string? OtherAward { get; set; }

        [ExcelColumnIndex("AJ")]
        [ExcelColumnName("奖励小计")]
        public string? AwardTotal { get; set; }

        [ExcelColumnIndex("AK")]
        [ExcelColumnName("提成")]
        public string? Enhanced { get; set; }

        [ExcelColumnIndex("AL")]
        [ExcelColumnName("加班费")]
        public string? OverTimePay { get; set; }

        [ExcelColumnIndex("AM")]
        [ExcelColumnName("缺勤扣除")]
        public string? AbsenceDeduction { get; set; }

        [ExcelColumnIndex("AN")]
        [ExcelColumnName("应发工资")]
        public string? BudgetSalary { get; set; }

        [ExcelColumnIndex("AO")]
        [ExcelColumnName("社保(单位)")]
        public string? SocialSecurity { get; set; }

        [ExcelColumnIndex("AP")]
        [ExcelColumnName("公积金(单位)")]
        public string? ProvidentFund { get; set; }

        [ExcelColumnIndex("AQ")]
        [ExcelColumnName("社保(个人)")]
        public string? PersionalSocialSecurity { get; set; }

        [ExcelColumnIndex("AR")]
        [ExcelColumnName("公积金(个人)")]
        public string? PersionalProvidentFund { get; set; }

        [ExcelColumnIndex("AS")]
        [ExcelColumnName("水电费")]
        public string? Utilities { get; set; }

        [ExcelColumnIndex("AT")]
        [ExcelColumnName("补卡")]
        public string? RepairAttendance { get; set; }

        [ExcelColumnIndex("AU")]
        [ExcelColumnName("个税")]
        public string? PersionalTax { get; set; }

        [ExcelColumnIndex("AV")]
        [ExcelColumnName("其他")]
        public string? Other { get; set; }

        [ExcelColumnIndex("AW")]
        [ExcelColumnName("扣除小计")]
        public string? DeductionTotal { get; set; }

        [ExcelColumnIndex("AX")]
        [ExcelColumnName("实发工资")]
        public string? PracticalSalary { get; set; }
    }
}
