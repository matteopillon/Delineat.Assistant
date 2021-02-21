using System;
using System.Collections.Generic;

namespace Delineat.Assistant.Models
{
    public class DWNote
    {
        public DWNote()
        {
            Id = 0;
            Note = string.Empty;
            InsertDate = DateTime.Now;
            IsRemainder = false;
            RemainderDate = null;
            Emails = new string[0];
            Scope = new DWScope();
            NoteType = NoteType.NotSet;
            Level = 0;
            Tags = new List<DWTag>();
            Topics = new List<DWTopic>();
        }

        public string Note { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? RemainderDate { get; set; }
        public string[] Emails { get; set; }
        public DWScope Scope { get; set; }
        public NoteType NoteType { get; set; }
        public int Id { get; set; }
        public bool IsRemainder { get; set; }
        public DateTime RemaindedDate { get; set; }
        public int Level { get; set; }
        public List<DWTag> Tags { get; set; }
        public List<DWTopic> Topics { get; set; }

    }
}
