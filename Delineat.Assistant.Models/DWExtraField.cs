using System;
using System.Collections.Generic;


namespace Delineat.Assistant.Models
{
    public enum DWExtraFieldType
    {
        Text,
        Numeric,
        Date,
        Bool,
        Combo,
        Price,
    }

    public class DWExtraField
    {
        public int Id { get; set; }

        public string Label { get; set; }

        public string Description { get; set; }

        public string ValidationExpression { get; set; }

        public DWExtraFieldType Type { get; set; }

        ICollection<DWExtraFieldDomainValue> ValuesDomain { get; set; }
    }

    public class DWExtraFieldValue
    {
        public double NumberValue { get; set; }

        public string TextValue { get; set; }

        public DateTime? DateTimeValue { get; set; }
    }

    public class DWExtraFieldDomainValue : DWExtraFieldValue
    {
    
        public int Id { get; set; }

    }

    public class DWJobExtraFieldValue : DWExtraFieldValue
    {
        public int Id { get; set; }

        public DWExtraField ExtraField { get; set; }


    }
}
