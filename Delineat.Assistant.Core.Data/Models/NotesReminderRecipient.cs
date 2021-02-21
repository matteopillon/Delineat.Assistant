using System.ComponentModel.DataAnnotations;

namespace Delineat.Assistant.Core.Data.Models
{
    public partial class NotesReminderRecipient : BaseObject
    {
        [Key]
        public int RecipientId { get; set; }
        public string Email { get; set; }
        public byte[] SentDate { get; set; }

        public Note Note { get; set; }
    }
}
