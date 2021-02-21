using System.ComponentModel.DataAnnotations;

namespace Delineat.Assistant.Core.Data.Models
{
    public class JobCode
    {
        [Key]
        public int CodeId { get; set; }

        public Job Job { get; set; }

        public string Code { get; set; }

        public string Note { get; set; }
    }
}
