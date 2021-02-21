using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delineat.Assistant.Core.Data.Models
{
    public class DayWorkLog
    {
        [Key]
        public int DayWorkLogId { get; set; }

        public User User { get; set; }

        public  Job Job { get; set; }

        public DateTime Date { get; set; }

        public int Minutes { get; set; }

        public DayWorkType WorkType { get; set; }

        public SubJob SubJob { get; set; }
    }
}
