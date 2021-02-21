using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Delineat.Assistant.Core.Data.Models
{
    public partial class Customer : BaseObject
    {
        public Customer()
        {
            Notes = new HashSet<CustomersNotes>();
            Specifications = new HashSet<Specification>();
            Job = new HashSet<Job>();
        }
        [Key]
        public int CustomerId { get; set; }

        public string Code { get; set; }
        public string Description { get; set; }
        public string Domain { get; set; }

        public ICollection<CustomersNotes> Notes { get; set; }
        public ICollection<Specification> Specifications { get; set; }
        public ICollection<Job> Job { get; set; }
    }
}
