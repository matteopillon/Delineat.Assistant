using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Delineat.Assistant.Core.Data.Models
{
    public partial class Tag : BaseObject
    {
        public Tag()
        {
            Documents = new HashSet<DocumentsTags>();
            Items = new HashSet<ItemsTags>();
            Notes = new HashSet<NotesTags>();
            Jobs = new HashSet<JobsTags>();
        }
        [Key]
        public int TagId { get; set; }
        public string Description { get; set; }

        public ICollection<DocumentsTags> Documents { get; set; }
        public ICollection<ItemsTags> Items { get; set; }
        public ICollection<NotesTags> Notes { get; set; }
        public ICollection<JobsTags> Jobs { get; set; }
        public string Color { get; set; }
    }
}
