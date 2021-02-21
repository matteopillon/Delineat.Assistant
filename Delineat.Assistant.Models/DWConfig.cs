using System.Collections.Generic;

namespace Delineat.Assistant.Models
{
    public class DWConfig
    {
        public DWConfig()
        {
            Categories = new List<string>();
            Emails = new List<string>();
        }
        public List<string> Categories { get; set; }
        public List<string> Emails { get; set; }
    }
}
