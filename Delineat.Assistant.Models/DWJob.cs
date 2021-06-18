using System;
using System.Collections.Generic;

namespace Delineat.Assistant.Models
{
    public class DWJob
    {
        #region Fields
        private readonly List<DWTag> tags = new List<DWTag>();
        private readonly List<DWTopic> topics = new List<DWTopic>();
        private readonly List<DWJobExtraFieldValue> fields = new List<DWJobExtraFieldValue>();
        private List<DWItem> items = new List<DWItem>();
        private List<DWNote> notes = new List<DWNote>();
        private List<DWJobCode> codes = new List<DWJobCode>();
        private List<DWJob> subJobs = new List<DWJob>();
        
        #endregion

        #region Properties

        public int JobId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }

        public DWCustomer Customer { get; set; }

        public DWGroup Group { get; set; }
        public List<DWTag> Tags => tags;

        public DWJob Parent  { get;set; }
        public List<DWTopic> Topics => topics;

        public List<DWItem> Items => items;
        public List<DWNote> Notes => notes;
        public List<DWJobCode> Codes => codes;

        public List<DWJob> SubJobs => subJobs;

        public List<DWJobExtraFieldValue> Fields => fields;
        public DWJobCustomerInfo CustomerInfo { get; set; }     

        public bool IsDeleted { get; set; }
        public DateTime? BeginDate { get; set; }
        #endregion

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(this.Code))
            {
                return $"{this.Code ?? string.Empty} - {this.Description ?? string.Empty}";
            }
            else
            {
                return string.Empty;
            }
        }

    }

    public class DWJobCustomerInfo
    {
        public string Info { get; set; }
        public string QuotationRef { get; set; }

        public string OrderRef { get; set; }

        public double Quotation { get; set; }

        public double InvoiceAmount { get; set; }

        public double OrderAmount { get; set; }

        public DateTime? Completed { get; set; }

        public DWUser CompletedBy { get; set; }

        public DateTime? Sent { get; set; }

        public DWUser SentBy { get; set; }

        public DateTime? EstimatedClosingDate { get; set; }
        public double MinutesQuotation { get; set; }
    }
}
