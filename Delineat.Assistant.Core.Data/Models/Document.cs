using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Delineat.Assistant.Core.Data.Models
{
    public partial class Document : BaseObject
    {
        public Document()
        {
            Notes = new HashSet<DocumentsNotes>();
            Tags = new HashSet<DocumentsTags>();
            Versions = new HashSet<DocumentVersion>();
            Topics = new HashSet<DocumentsTopics>();
            WorkLogs = new HashSet<DocumentsWorkLogs>();
            Item = null;
        }
        [Key]
        public int DocumentId { get; set; }

        public ICollection<DocumentsNotes> Notes { get; set; }
        public ICollection<DocumentsTags> Tags { get; set; }
        public ICollection<DocumentsTopics> Topics { get; set; }
        public Item Item { get; set; }
        public int ItemId { get; set; }
        public ICollection<DocumentVersion> Versions { get; set; }
        public ICollection<DocumentsWorkLogs> WorkLogs { get; set; }
        public int OpenedCount { get; set; }

    }
}
