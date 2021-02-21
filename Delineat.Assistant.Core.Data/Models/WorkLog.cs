using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Delineat.Assistant.Core.Data.Models
{
    public class WorkLog : BaseObject
    {
        [Key]
        public int WorkLogId { get; set; }

        public DateTime? ExtimatedBeginDate { get; set; }
        public DateTime? ExtimatedEndDate { get; set; }
        public int ExtimatedHour { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int WorkedHour { get; set; }
        public int Status { get; set; }
        public WorkLogType WorkType { get; set; }
        public int AssignedTo { get; set; }
        public string Note { get; set; }

        public ICollection<DocumentsWorkLogs> Documents { get; set; }
        public ICollection<ItemsWorkLogs> Items { get; set; }
        public ICollection<NotesWorkLogs> Notes { get; set; }
        public ICollection<JobsWorkLogs> Jobs { get; set; }

    }
}
