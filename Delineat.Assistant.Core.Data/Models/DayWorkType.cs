using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delineat.Assistant.Core.Data.Models
{
    public class DayWorkType
    {
        [Key]
        public int DayWorkTypeId { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public ICollection<DayWorkLog> DayWorkLogs { get; set; }

        public bool Enabled { get; set; }
    }
}
