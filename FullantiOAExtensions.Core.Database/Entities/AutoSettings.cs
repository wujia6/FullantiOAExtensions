using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullantiOAExtensions.Core.Database.Entities
{
    public class AutoSettings
    {
        public int id { get; set; }

        public int AutoEnsure { get; set; }

        public int Day { get; set; }

        public string Create_WorkNo { get; set; }
    }
}
