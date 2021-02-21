using System;

namespace Delineat.Assistant.Models
{
    public class DWFilePassword
    {
        public Guid SessionId { get; set; }
        public string Path { get; set; }

        public string Password { get; set; }
    }
}
