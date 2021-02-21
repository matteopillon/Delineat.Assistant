using System.ComponentModel.DataAnnotations;

namespace Delineat.Assistant.Core.Data.Models
{
    public partial class Specification : BaseObject
    {
        [Key]
        public int SpecificationId { get; set; }
        public string Description { get; set; }
        public Note Note { get; set; }
    }
}
