using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delineat.Assistant.API.Configuration
{
    public class DAJobsConfiguration
    {
        public double Interval { get; set; }
        public int CleanDays { get; set; }

        public bool Disabled { get; set; }
    }
}
