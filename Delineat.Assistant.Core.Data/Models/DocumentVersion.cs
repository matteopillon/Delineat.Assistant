using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Delineat.Assistant.Core.Data.Models
{
    public class DocumentVersion : BaseObject
    {
        public DocumentVersion()
        {
            Thumbnails = new HashSet<Thumbnail>();
        }
        [Key]
        public int DocumentVersionId { get; set; }
        public string Filename { get; set; }
        public string Extension { get; set; }
        public Document Document { get; set; }
        public ICollection<Thumbnail> Thumbnails { get; set; }
        public string RelativePath { get; set; }
        public int Status { get; set; }
        public DateTime? StatusSince { get; set; }
        public DateTime? WaitingForReply { get; set; }
        public DateTime? Reply { get; set; }

        public bool InEvidence { get; set; }
    }
}
