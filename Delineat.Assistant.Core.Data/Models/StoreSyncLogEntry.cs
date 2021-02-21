using System.ComponentModel.DataAnnotations;

namespace Delineat.Assistant.Core.Data.Models
{
    public class StoreSyncLogEntry
    {
        [Key]
        public int LogEntryId { get; set; }
        public StoreSyncLog Log;
        public string Message { get; set; }
    }
}
