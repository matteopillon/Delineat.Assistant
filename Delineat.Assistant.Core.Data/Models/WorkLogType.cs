using System.Collections.Generic;

namespace Delineat.Assistant.Core.Data.Models
{
    public class WorkLogType
    {
        public WorkLogType()
        {
            WorkLogs = new HashSet<WorkLog>();
        }
        public int WorkLogTypeId { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public ICollection<WorkLog> WorkLogs { get; set; }
    }
}
