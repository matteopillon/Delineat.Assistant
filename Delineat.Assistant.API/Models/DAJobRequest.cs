using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delineat.Assistant.API.Models
{
    public class DAJobRequest
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string CustomerInfo { get; set; }
        public string QuotationRef { get; set; }
        public string OrderRef  { get; set; }
        public int CustomerId { get; set; }
    }
}
