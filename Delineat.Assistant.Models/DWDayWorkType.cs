using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delineat.Assistant.Models
{
    public class DWDayWorkType
    {
        public int DayWorkTypeId { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }
        public bool Enabled { get; set; }
    }
}
