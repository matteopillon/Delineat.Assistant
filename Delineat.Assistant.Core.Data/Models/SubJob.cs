using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delineat.Assistant.Core.Data.Models
{
    public class SubJob
    {
        [Key]
        public int SubJobId { get; set; }
        public string Description { get; set; }

        public SubJob Parent { get; set; }
        public Job Job { get; set; }

        public ICollection<SubJob> SubJobs { get; set; }

        public ICollection<DayWorkLog> DayWorkLogs { get; set; }
    }
}
