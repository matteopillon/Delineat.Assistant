using System.ComponentModel.DataAnnotations;

namespace Delineat.Assistant.Core.Data.Models
{
    public class JobType
    {
        [Key]
        public int JobTypeId { get; set; }

        public string Description { get; set; }

        public string Color { get; set; }
    }
}
