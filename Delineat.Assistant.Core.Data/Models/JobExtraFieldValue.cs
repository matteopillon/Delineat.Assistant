using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delineat.Assistant.Core.Data.Models
{
    public class JobExtraFieldValue:ExtraFieldValue
    {
        [Key]
        public int Id { get; set; }

        public Job Job { get; set; } 

        public ExtraField ExtraField { get; set; }

       
    }
}
