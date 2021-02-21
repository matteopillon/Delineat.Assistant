using System.Collections.Generic;


namespace Delineat.Assistant.Models
{
    public class DWDocument
    {

        public int DocumentId { get; set; }
        public int OpenedCount { get; set; }

        public List<DWNote> Notes => notes;
        public List<DWTag> Tags => tags;
        public List<DWDocumentVersion> Versions => versions;
        public List<DWWorkLog> WorkLogs => workLogs;

        private readonly List<DWNote> notes = new List<DWNote>();
        private readonly List<DWTag> tags = new List<DWTag>();
        private readonly List<DWDocumentVersion> versions = new List<DWDocumentVersion>();
        private readonly List<DWWorkLog> workLogs = new List<DWWorkLog>();


    }
}
