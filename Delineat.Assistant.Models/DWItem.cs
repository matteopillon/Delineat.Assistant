using Delineat.Assistant.Models;
using System;
using System.Collections.Generic;

namespace Delineat.Assistant.Models
{
    public class DWItem
    {
        #region Fields

        private readonly List<DWAttachment> attachments = new List<DWAttachment>();

        private readonly List<DWNote> notes = new List<DWNote>();
        private readonly List<DWTag> tags = new List<DWTag>();
        private readonly List<DWTopic> topics = new List<DWTopic>();
        private readonly List<string> categories = new List<string>();
        private readonly List<DWDocument> documents = new List<DWDocument>();
        private readonly List<DWWorkLog> workLogs = new List<DWWorkLog>();

        #endregion

        #region Properties
        public int ItemId { get; set; }
        public DateTime Date { get; set; }
        public int JobId { get; set; }
        public ItemType ItemType { get; set; }
        public string Who { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }

/* Unmerged change from project 'Delineat.Workflow.Core'
Before:
        public List<DWAttachment> Attachments => attachments;
        
        public List<DWNote> Notes => notes;
After:
        public List<DWAttachment> Attachments => attachments;

        public List<DWNote> Notes => notes;
*/
        public List<DWAttachment> Attachments => attachments;

        public List<DWNote> Notes => notes;

        public List<DWTag> Tags => tags;

        public List<DWDocument> Documents => documents;
        public List<DWTopic> Topics => topics;
        public List<DWWorkLog> WorkLogs => workLogs;

        #endregion
    }
}
