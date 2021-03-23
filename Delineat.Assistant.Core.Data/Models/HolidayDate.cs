using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delineat.Assistant.Core.Data.Models
{
    public class HolidayDate
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }

        public int Year { get; set; }

        public string FormulaId { get; set; }

        public int Minutes { get; set; }
    }
}
