using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Delineat.Assistant.Core.Data.Models
{
    public class JobGroup
    {
        private HashSet<Job> jobs;

        public JobGroup()
        {
            jobs = new HashSet<Job>();
        }

        [Key]
        public int GroupId { get; set; }
        public string Name { get; set; }

        public bool IsCurrent { get; set; }

        public string Path { get; set; }

        public ICollection<Job> Jobs => jobs;
    }
}
