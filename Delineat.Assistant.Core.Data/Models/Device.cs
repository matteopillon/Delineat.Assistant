using System.ComponentModel.DataAnnotations;

namespace Delineat.Assistant.Core.Data.Models
{
    public class Device : BaseObject
    {
        [Key]
        public int DeviceId { get; set; }

        public string Name { get; set; }

        public string PushUrl { get; set; }

        public string HostName { get; set; }
    }
}
