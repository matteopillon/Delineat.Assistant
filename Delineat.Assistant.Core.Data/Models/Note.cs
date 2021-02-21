using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Delineat.Assistant.Core.Data.Models
{
    public partial class Note : BaseObject
    {
        public Note()
        {
            NotesReminderRecipients = new HashSet<NotesReminderRecipient>();
            Topics = new HashSet<NotesTopics>();
            Tags = new HashSet<NotesTags>();
            Customers = new HashSet<CustomersNotes>();
            Documents = new HashSet<DocumentsNotes>();
            Jobs = new HashSet<JobsNotes>();
            Items = new HashSet<ItemsNotes>();
            WorkLogs = new HashSet<NotesWorkLogs>();
        }

        [Key]
        public int NoteId { get; set; }
        public string NoteType { get; set; }
        public string Text { get; set; }
        public DateTime? ReminderDate { get; set; }
        public int ReminderType { get; set; }
        public DateTime? RemindedDate { get; set; }

        public ICollection<CustomersNotes> Customers { get; set; }
        public ICollection<DocumentsNotes> Documents { get; set; }
        public ICollection<JobsNotes> Jobs { get; set; }
        public ICollection<ItemsNotes> Items { get; set; }
        public ICollection<NotesReminderRecipient> NotesReminderRecipients { get; set; }

        public ICollection<NotesTopics> Topics { get; set; }
        public ICollection<NotesTags> Tags { get; set; }
        public ICollection<NotesWorkLogs> WorkLogs { get; set; }
        public int Level { get; set; }
    }
}
