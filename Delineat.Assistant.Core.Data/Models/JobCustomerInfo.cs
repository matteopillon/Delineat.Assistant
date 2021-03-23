using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delineat.Assistant.Core.Data.Models
{
    public class JobCustomerInfo
    {
        public string Info { get; set; }
        public string QuotationRef { get; set; }

        public string OrderRef { get; set; }

        public double Quotation { get; set; }

        public double InvoiceAmount { get; set; }

        public double OrderAmount { get; set; }

        public DateTime? Completed { get; set; }

        public User CompletedBy { get; set; }

        public DateTime? Sent { get; set; }

        public User SentBy { get; set; }

        public DateTime? EstimatedClosingDate { get; set; }
    }
}
