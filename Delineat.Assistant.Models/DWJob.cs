using System.Collections.Generic;

namespace Delineat.Assistant.Models
{
    public class DWJob
    {
        #region Fields
        private readonly List<DWTag> tags = new List<DWTag>();
        private readonly List<DWTopic> topics = new List<DWTopic>();
        private List<DWItem> items = new List<DWItem>();
        private List<DWNote> notes = new List<DWNote>();
        private List<DWJobCode> codes = new List<DWJobCode>();
        private List<DWSubJob> subJobs = new List<DWSubJob>();
        
        #endregion

        #region Properties

        public int JobId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }

        public DWCustomer Customer { get; set; }

        public DWGroup Group { get; set; }
        public List<DWTag> Tags => tags;

        public List<DWTopic> Topics => topics;

        public List<DWItem> Items => items;
        public List<DWNote> Notes => notes;
        public List<DWJobCode> Codes => codes;

        public List<DWSubJob> SubJobs => subJobs;


        public string CustomerInfo { get; set; }
        public string OrderRef { get; set; }
        public string QuotationRef { get; set; }

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
}
