using System.ComponentModel.DataAnnotations;

namespace Delineat.Assistant.Core.Data.Models
{
    public class WeekWorkHours
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public double OnMonday { get; set; }
        public double OnTuesday { get; set; }

        public double OnWednesday { get; set; }
        public double OnThursday { get; set; }
        public double OnFriday { get; set; }
        public double OnSaturday { get; set; }
        public double OnSunday { get; set; }
    }
}
