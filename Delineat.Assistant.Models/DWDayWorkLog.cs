using System;


namespace Delineat.Assistant.Models
{
    public class DWDayWorkLog
    {
       
        public int DayWorkLogId { get; set; }

        public DWUser User { get; set; }

        public DWJob Job { get; set; }

        public DateTime Date { get; set; }

        public int Minutes { get; set; }

        public DWDayWorkType WorkType { get; set; }
    }
}
