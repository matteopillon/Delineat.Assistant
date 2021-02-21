using System;

namespace Delineat.Assistant.Models
{
    public class DWWorkLog
    {
        public int WorkLogId { get; set; }

        public DateTime? ExtimatedBeginDate { get; set; }
        public DateTime? ExtimatedEndDate { get; set; }
        public TimeSpan ExtimatedHour { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan WorkedHour { get; set; }
        public WorkLogStatus Status { get; set; }
        public DWWorkLogType WorkType { get; set; }
        public WorkLogAssignedType AssignedTo { get; set; }
        public string Note { get; set; }
    }
}

