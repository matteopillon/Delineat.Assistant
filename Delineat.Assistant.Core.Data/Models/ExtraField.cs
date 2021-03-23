using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delineat.Assistant.Core.Data.Models
{
    public enum ExtraFieldType
    {
        Text,     
        Numeric,
        Date,
        Bool,
        Combo,
        Price,
    }

    public class ExtraField
    {
        [Key]
        public int Id { get; set; }

        public string Label { get; set; }

        public string Description { get; set; }
        
        public string ValidationExpression { get; set; }

        public ExtraFieldType Type { get; set; }

        public ICollection<ExtraFieldDomainValue> ValuesDomain { get; set; }

        public int Order { get; set; }
       
    }

    public class ExtraFieldValue:BaseObject {
        public double NumberValue { get; set; }

        public string TextValue { get; set; }

        public DateTime? DateTimeValue { get; set; }

    }

    public class ExtraFieldDomainValue: ExtraFieldValue
    {
        [Key]
        public int Id { get; set; }
  
    }
}
