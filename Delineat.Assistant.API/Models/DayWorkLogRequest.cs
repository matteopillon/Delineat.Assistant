using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delineat.Assistant.API.Models
{
    public class DayWorkLogRequest
    {
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public string Note { get; set; }
        public int Minutes { get; set; }
        public int JobId { get; set; }

        public int DayWorkTypeId { get; set; }
    }
}
