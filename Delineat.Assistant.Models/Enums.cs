namespace Delineat.Assistant.Models
{
    public enum NoteReminderType
    {
        None = 0,
        Email = 1,
    }

    public enum NoteType
    {
        NotSet = ' ',
        Job = 'J',
        Item = 'I',
        Customer = 'C',
        Document = 'D',
    }

    public enum ItemType
    {
        None,
        Incoming = 'r',
        Outcoming = 'i',
        Internal = 'd'
    }

    public enum WorkLogAssignedType
    {
        None = 'x',
        Internal = 'i',
        External = 'e',
    }

    public enum WorkLogStatus
    {
        None = 'x',
        ToStart = 's',
        Waiting = 'w',
        InProgress = 'i',
        Paused = 'p',
        Completed = 'c',
        Canceled = 'd',
    }

    public enum DocumentVersionStatus
    {
        Valid = 0,
        PartiallyValid = 1,
        NotValid = 2,
    }

}
