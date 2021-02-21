namespace Delineat.Assistant.Core.Data.Models
{
    #region Notes
    public class DocumentsNotes
    {
        public int DocumentId { get; set; }
        public int NoteId { get; set; }
        public Document Document { get; set; }
        public Note Note { get; set; }
    }

    public class CustomersNotes
    {
        public int CustomerId { get; set; }
        public int NoteId { get; set; }
        public Customer Customer { get; set; }
        public Note Note { get; set; }
    }

    public class JobsNotes
    {
        public int JobId { get; set; }
        public int NoteId { get; set; }
        public Job Job { get; set; }
        public Note Note { get; set; }
    }

    public class ItemsNotes
    {
        public int ItemId { get; set; }
        public int NoteId { get; set; }
        public Item Item { get; set; }
        public Note Note { get; set; }
    }
    #endregion

    #region Topic
    public class DocumentsTopics
    {
        public int DocumentId { get; set; }
        public int TopicId { get; set; }
        public Document Document { get; set; }
        public Topic Topic { get; set; }
    }

    public class NotesTopics
    {
        public int NoteId { get; set; }
        public int TopicId { get; set; }
        public Note Note { get; set; }
        public Topic Topic { get; set; }
    }

    public class ItemsTopics
    {
        public int ItemId { get; set; }
        public int TopicId { get; set; }
        public Item Item { get; set; }
        public Topic Topic { get; set; }
    }
    #endregion

    #region Work Logs
    public class ItemsWorkLogs
    {
        public int ItemId { get; set; }
        public int WorkLogId { get; set; }
        public Item Item { get; set; }
        public WorkLog WorkLog { get; set; }
    }

    public class DocumentsWorkLogs
    {
        public int DocumentId { get; set; }
        public int WorkLogId { get; set; }
        public Document Document { get; set; }
        public WorkLog WorkLog { get; set; }
    }

    public class JobsWorkLogs
    {
        public int JobId { get; set; }
        public int WorkLogId { get; set; }
        public Job Job { get; set; }
        public WorkLog WorkLog { get; set; }
    }

    public class NotesWorkLogs
    {
        public int NoteId { get; set; }
        public int WorkLogId { get; set; }
        public Note Note { get; set; }
        public WorkLog WorkLog { get; set; }
    }
    #endregion

    #region Tags
    public class DocumentsTags
    {
        public int DocumentId { get; set; }
        public int TagId { get; set; }
        public Document Document { get; set; }
        public Tag Tag { get; set; }
    }

    public class NotesTags
    {
        public int NoteId { get; set; }
        public int TagId { get; set; }
        public Note Note { get; set; }
        public Tag Tag { get; set; }
    }

    public class ItemsTags
    {
        public int ItemId { get; set; }
        public int TagId { get; set; }
        public Item Item { get; set; }
        public Tag Tag { get; set; }
    }

    public class JobsTags
    {
        public int JobId { get; set; }
        public int TagId { get; set; }
        public Job Job { get; set; }
        public Tag Tag { get; set; }
    }
    #endregion
}
