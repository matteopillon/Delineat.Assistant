using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Delineat.Assistant.Core.Data.Models
{
    public class Topic : BaseObject
    {
        public Topic()
        {
            Documents = new HashSet<DocumentsTopics>();
            Notes = new HashSet<NotesTopics>();
            Items = new HashSet<ItemsTopics>();
        }

        [Key]
        public int TopicId { get; set; }

        public string Description { get; set; }
        public string Color { get; set; }
        public virtual ICollection<DocumentsTopics> Documents { get; set; }
        public virtual ICollection<NotesTopics> Notes { get; set; }
        public virtual ICollection<ItemsTopics> Items { get; set; }
        public virtual Job Job { get; set; }
    }
}
