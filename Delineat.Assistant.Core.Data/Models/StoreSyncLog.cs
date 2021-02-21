using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Delineat.Assistant.Core.Data.Models
{
    public class StoreSyncLog
    {
        public StoreSyncLog()
        {
            Entries = new HashSet<StoreSyncLogEntry>();
        }
        [Key]
        public int SyncId { get; set; }
        public JobGroup Group { get; set; }
        public string TargetName { get; set; }
        public bool Completed { get; set; }
        public DateTime InsertDate { get; set; }
        public ICollection<StoreSyncLogEntry> Entries { get; set; }
    }
}
