using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delineat.Assistant.Models
{
    public class DWSubJob
    {
        public int SubJobId { get; set; }
        public string Description { get; set; }

        public DWSubJob Parent { get; set; }
        public DWJob Job { get; set; }

        public DWSubJob[] SubJobs { get; set; }

    }
}
