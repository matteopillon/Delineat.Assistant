using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Delineat.Assistant.Core.Data.Models
{
    public partial class Job : BaseObject
    {
        public Job()
        {
            Items = new HashSet<Item>();
            Notes = new HashSet<JobsNotes>();
            Tags = new HashSet<JobsTags>();
            Topics = new HashSet<Topic>();
            WorkLogs = new HashSet<JobsWorkLogs>();
            Codes = new HashSet<JobCode>();
            IsAbsence = false;
        }
        [Key]
        public int JobId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime? BeginDate { get; set; }
        public Job Parent { get; set; }
        public Customer Customer { get; set; }
        public JobGroup Group { get; set; }
        public ICollection<Item> Items { get; set; }
        public ICollection<JobsNotes> Notes { get; set; }
        public ICollection<JobsTags> Tags { get; set; }
        public ICollection<Topic> Topics { get; set; }
        public ICollection<JobsWorkLogs> WorkLogs { get; set; }
        public ICollection<Job> SubJobs { get; set; }
        public ICollection<JobCode> Codes { get; set; }
        public ICollection<DayWorkLog> DayWorkLogs { get; set; }
        public ICollection<JobExtraFieldValue> Fields { get; set; }

        public JobCustomerInfo CustomerInfo { get; set; }

        public JobType JobType { get; set; }
        public string Path { get; set; }

        public bool IsAbsence { get; set; }
    }
}
