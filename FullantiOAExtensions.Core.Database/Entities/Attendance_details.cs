using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullantiOAExtensions.Core.Database.Entities
{

    public class Attendance_details
    {
        public int Id { get; set; }

        [ExcelColumnIndex("A")]
        [ExcelColumnName("人员姓名")]
        public string? Name { get; set; }

        [ExcelColumnIndex("B")]
        [ExcelColumnName("人员编号")]
        public string? WorkNo { get; set; }

        [ExcelColumnIndex("C")]
        [ExcelColumnName("部门层级")]
        public string? DepartmentsLevel { get; set; }

        [ExcelColumnIndex("D")]
        [ExcelColumnName("部门")]
        public string? Departments { get; set; }

        [ExcelColumnIndex("E")]
        [ExcelColumnName("主岗")]
        public string? Post { get; set; }

        [ExcelColumnIndex("F")]
        [ExcelColumnName("薪酬类型")]
        public string? salary_type { get; set; }

        [ExcelColumnIndex("G")]
        [ExcelColumnName("应出勤天数")]
        public string? AttendanceDays { get; set; }

        [ExcelColumnIndex("H")]
        [ExcelColumnName("实际出勤天数")]
        public string? ActualAttendanceDays { get; set; }

        [ExcelColumnIndex("I")]
        [ExcelColumnName("缺勤天数")]
        public string? AbsenceDays { get; set; }

        [ExcelColumnIndex("J")]
        [ExcelColumnName("夜班天数")]
        public string? Total_night_shift_days { get; set; }

        [ExcelColumnIndex("K")]
        [ExcelColumnName("中班天数")]
        public string? Total_middle_shift_days { get; set; }

        [ExcelColumnIndex("L")]
        [ExcelColumnName("出差天数")]
        public string? BusinessTripDays { get; set; }

        [ExcelColumnIndex("M")]
        [ExcelColumnName("法定假天数")]
        public string? StatutoryHolidayDays { get; set; }

        [ExcelColumnIndex("N")]
        [ExcelColumnName("工伤天数")]
        public string? WorkInjuryDays { get; set; }

        [ExcelColumnIndex("O")]
        [ExcelColumnName("缺卡次数")]
        public string? MissingCardCount { get; set; }

        [ExcelColumnIndex("P")]
        [ExcelColumnName("补卡次数")]
        public string? MakeUpCardCount { get; set; }

        [ExcelColumnIndex("Q")]
        [ExcelColumnName("迟到分钟数")]
        public string? LateMinutes { get; set; }

        [ExcelColumnIndex("R")]
        [ExcelColumnName("早退分钟数")]
        public string? EarlyLeaveMinutes { get; set; }

        [ExcelColumnIndex("S")]
        [ExcelColumnName("调休天数")]
        public string? CompensatoryLeaveDays { get; set; }

        [ExcelColumnIndex("T")]
        [ExcelColumnName("年假天数")]
        public string? AnnualLeaveDays { get; set; }

        [ExcelColumnIndex("U")]
        [ExcelColumnName("病假天数")]
        public string? SickLeaveDays { get; set; }

        [ExcelColumnIndex("V")]
        [ExcelColumnName("事假天数")]
        public string? PersonalLeaveDays { get; set; }

        [ExcelColumnIndex("W")]
        [ExcelColumnName("产假天数")]
        public string? MaternityLeaveDays { get; set; }

        [ExcelColumnIndex("X")]
        [ExcelColumnName("婚假天数")]
        public string? MarriageLeaveDays { get; set; }

        [ExcelColumnIndex("Y")]
        [ExcelColumnName("丧假天数")]
        public string? FuneralLeaveDays { get; set; }

        [ExcelColumnIndex("Z")]
        [ExcelColumnName("探亲假天数")]
        public string? FamilyVisitLeaveDays { get; set; }

        [ExcelColumnIndex("AA")]
        [ExcelColumnName("请假天数合计")]
        public string? TotalLeaveDays { get; set; }

        [ExcelColumnIndex("AB")]
        [ExcelColumnName("加班总时长")]
        public string? OvertimeTotalDuration { get; set; }

        [ExcelColumnIndex("AC")]
        [ExcelColumnName("工作日转加班费")]
        public string? WeekdayOvertimeToPay { get; set; }
        [ExcelColumnIndex("AD")]
        [ExcelColumnName("休息日转加班费")]
        public string? HolidayOvertimeToPay { get; set; }

        [ExcelColumnIndex("AE")]
        [ExcelColumnName("节假日转加班费")]
        public string? HolidayToOvertimeFlag { get; set; }

        [ExcelColumnIndex("AF")]
        [ExcelColumnName("工作日转调休")]
        public string? WeekdayOvertimeToCompensatoryLeave { get; set; }

        [ExcelColumnIndex("AG")]
        [ExcelColumnName("休息日转调休")]
        public string? WeekendOvertimeToCompensatoryLeave { get; set; }

        [ExcelColumnIndex("AH")]
        [ExcelColumnName("节假日转调休")]
        public string? HolidayOvertimeToCompensatoryLeave { get; set; }

        [ExcelColumnIndex("AI")]
        [ExcelColumnName("月份")]
        public string? Month { get; set; }

        [ExcelColumnIndex("AJ")]
        [ExcelColumnName("1号")]
        public string? Day1 { get; set; }
        [ExcelColumnIndex("AK")]
        [ExcelColumnName("2号")]
        public string? Day2 { get; set; }
        [ExcelColumnIndex("AL")]
        [ExcelColumnName("3号")]
        public string? Day3 { get; set; }
        [ExcelColumnIndex("AM")]
        [ExcelColumnName("4号")]
        public string? Day4 { get; set; }
        [ExcelColumnIndex("AN")]
        [ExcelColumnName("5号")]
        public string? Day5 { get; set; }
        [ExcelColumnIndex("AO")]
        [ExcelColumnName("6号")]
        public string? Day6 { get; set; }
        [ExcelColumnIndex("AP")]
        [ExcelColumnName("7号")]
        public string? Day7 { get; set; }
        [ExcelColumnIndex("AQ")]
        [ExcelColumnName("8号")]
        public string? Day8 { get; set; }
        [ExcelColumnIndex("AR")]
        [ExcelColumnName("9号")]
        public string? Day9 { get; set; }
        [ExcelColumnIndex("AS")]
        [ExcelColumnName("10号")]
        public string? Day10 { get; set; }
        [ExcelColumnIndex("AT")]
        [ExcelColumnName("11号")]
        public string? Day11 { get; set; }
        [ExcelColumnIndex("AU")]
        [ExcelColumnName("12号")]
        public string? Day12 { get; set; }
        [ExcelColumnIndex("AV")]
        [ExcelColumnName("13号")]
        public string? Day13 { get; set; }
        [ExcelColumnIndex("AW")]
        [ExcelColumnName("14号")]
        public string? Day14 { get; set; }
        [ExcelColumnIndex("AX")]
        [ExcelColumnName("15号")]
        public string? Day15 { get; set; }
        [ExcelColumnIndex("AY")]
        [ExcelColumnName("16号")]
        public string? Day16 { get; set; }
        [ExcelColumnIndex("AZ")]
        [ExcelColumnName("17号")]
        public string? Day17 { get; set; }
        [ExcelColumnIndex("BA")]
        [ExcelColumnName("18号")]
        public string? Day18 { get; set; }
        [ExcelColumnIndex("BB")]
        [ExcelColumnName("19号")]
        public string? Day19 { get; set; }
        [ExcelColumnIndex("BC")]
        [ExcelColumnName("20号")]
        public string? Day20 { get; set; }
        [ExcelColumnIndex("BD")]
        [ExcelColumnName("21号")]
        public string? Day21 { get; set; }
        [ExcelColumnIndex("BE")]
        [ExcelColumnName("22号")]
        public string? Day22 { get; set; }
        [ExcelColumnIndex("BF")]
        [ExcelColumnName("23号")]
        public string? Day23 { get; set; }
        [ExcelColumnIndex("BG")]
        [ExcelColumnName("21号")]
        public string? Day24 { get; set; }
        [ExcelColumnIndex("BH")]
        [ExcelColumnName("25号")]
        public string? Day25 { get; set; }
        [ExcelColumnIndex("BI")]
        [ExcelColumnName("26号")]
        public string? Day26 { get; set; }
        [ExcelColumnIndex("BJ")]
        [ExcelColumnName("27号")]
        public string? Day27 { get; set; }
        [ExcelColumnIndex("BK")]
        [ExcelColumnName("28号")]
        public string? Day28 { get; set; }
        [ExcelColumnIndex("BL")]
        [ExcelColumnName("29号")]
        public string? Day29 { get; set; }
        [ExcelColumnIndex("BM")]
        [ExcelColumnName("30号")]
        public string? Day30 { get; set; }

        [ExcelColumnIndex("BN")]
        [ExcelColumnName("31号")]
        public string? Day31 { get; set; }
       

        [ExcelColumnName("确认状态")]
        public string? StatusChecker { get; set; }

        [ExcelColumnName("创建时间")]
        public string? CreateTime { get; set; }

        [ExcelColumnName("创建者工号")]
        public string? Create_WorkNo { get; set; }



    }
}

