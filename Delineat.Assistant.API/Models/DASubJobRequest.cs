using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delineat.Assistant.API.Models
{
    public class DASubJobRequest
    {
        public string Code { get; set; }
        public string Description { get; set; }       
        public int CustomerId { get; set; }
    }
}
