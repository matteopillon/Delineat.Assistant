using System;
using System.Collections.Generic;

namespace Delineat.Assistant.Models
{
    public class DWDocumentVersion
    {
        public int DocumentVersionId { get; set; }
        public string Filename { get; set; }
        public string Extension { get; set; }
        public DateTime InsertDate { get; set; }
        public List<DWThumbnail> Thumbnails => thumbnails;

        public string RelativePath { get; set; }

        public DocumentVersionStatus Status { get; set; }
        public DateTime? StatusSince { get; set; }
        public DateTime? WaitingForReply { get; set; }
        public DateTime? Reply { get; set; }

        public string PhysicalPath { get; set; }
        public bool InEvidence { get; set; }

        private readonly List<DWThumbnail> thumbnails = new List<DWThumbnail>();

    }
}
