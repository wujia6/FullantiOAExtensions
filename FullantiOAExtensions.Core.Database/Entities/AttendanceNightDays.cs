using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullantiOAExtensions.Core.Database.Entities
{
    public class AttendanceNightDays
    {

        [ExcelColumnIndex("A")]
        [ExcelColumnName("人员姓名")]
        public string? Name { get; set; }

        [ExcelColumnIndex("B")]
        [ExcelColumnName("人员编号")]
        public string? WorkNo { get; set; }

        [ExcelColumnIndex("C")]
        [ExcelColumnName("夜班天数汇总")]
        public string? total_night_shift_days { get; set; }


    }
}
