using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delineat.Assistant.API.Models
{
    public class DANoteApiRequest
    {
        public string Note { get; set; }
        public bool IsRemainder { get; set; }
        public DateTime remainderDateTime { get; set; }
        public string[] Emails { get; set; }
    }
}
