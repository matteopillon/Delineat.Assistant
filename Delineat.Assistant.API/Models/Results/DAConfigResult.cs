using System.Collections.Generic;

namespace Delineat.Assistant.API.Models.Results
{
    public class DAConfigResult
    {
        public DAConfigResult()
        {
            Categories = new List<string>();
            Emails = new List<string>();
        }
        public List<string> Categories { get; set; }
        public List<string> Emails { get; set; }
    }
}
