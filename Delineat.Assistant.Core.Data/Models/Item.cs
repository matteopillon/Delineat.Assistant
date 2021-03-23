using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Delineat.Assistant.Core.Data.Models
{
    public partial class Item : BaseObject
    {
        public Item()
        {
            Documents = new HashSet<Document>();
            Notes = new HashSet<ItemsNotes>();
            Tags = new HashSet<ItemsTags>();
            Topics = new HashSet<ItemsTopics>();
            WorkLogs = new HashSet<ItemsWorkLogs>();
        }
        [Key]
        public int ItemId { get; set; }
        public string Description { get; set; }
        public int JobId { get; set; }
        public DateTime ReferenceDate { get; set; }
        public string Who { get; set; }
        public string ItemSource { get; set; }
        public string Color { get; set; }
        public Job Job { get; set; }
        public ICollection<Document> Documents { get; set; }
        public ICollection<ItemsNotes> Notes { get; set; }
        public ICollection<ItemsTags> Tags { get; set; }
        public ICollection<ItemsTopics> Topics { get; set; }
        public ICollection<ItemsWorkLogs> WorkLogs { get; set; }
        public string Path { get; set; }


    }
}
