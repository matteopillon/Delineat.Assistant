using Delineat.Assistant.Models;
using System;

namespace Delineat.Assistant.API.Models
{

    public class DAJobRequest 
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public DWJobCustomerInfo CustomerInfo { get; set; }

        public DateTime? BeginDate { get; set; }
        public int CustomerId { get; set; }

        public DAJobFieldValue[] Fields { get; set; }
    }

    public class DAAddJobRequest
    {        
        public DAJobRequest Job { get; set; }
        public int? ParentId { get; set; }
    }

    public class DAJobFieldValue:DWExtraFieldValue
    {
        public int FieldId { get; set; }
        public int? JobFieldId { get; set; }
    }
}
