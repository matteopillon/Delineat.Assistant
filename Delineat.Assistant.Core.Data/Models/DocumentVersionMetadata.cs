using System.ComponentModel.DataAnnotations;

namespace Delineat.Assistant.Core.Data.Models
{
    public class DocumentVersionMetadata : BaseObject
    {
        [Key]
        public int MetadataId { get; set; }

        public DocumentVersion DocumentVersion { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}
