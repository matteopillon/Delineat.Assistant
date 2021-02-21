using System.ComponentModel.DataAnnotations;

namespace Delineat.Assistant.Core.Data.Models
{
    public class ApplicationSetting
    {
        [Key]
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
