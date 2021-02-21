using System.ComponentModel.DataAnnotations;

namespace Delineat.Assistant.Core.Data.Models
{
    public class Thumbnail : BaseObject
    {
        [Key]
        public int ThumbnailId { get; set; }
        public string Title { get; set; }
        public DocumentVersion DocumentVersion { get; set; }
    }
}
