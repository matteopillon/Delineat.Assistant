using System;

namespace Delineat.Assistant.Core.Data.Models
{
    public class BaseObject
    {
        public DateTime InsertDate { get; set; }
        public User InsertUser;
        public DateTime? UpdateDate { get; set; }
        public User UpdateUser;
        public DateTime? DeleteDate { get; set; }
        public User DeleteUser;
        public int? ImportSyncId { get; set; }
        public int? ExportSyncId { get; set; }
    }
}
