using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delineat.Assistant.API.Models
{
    public class CustomerRequest
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Domain { get; set; }
    }
}
