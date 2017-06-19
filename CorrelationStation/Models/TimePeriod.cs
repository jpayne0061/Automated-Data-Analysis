using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorrelationStation.Models
{
    public class TimePeriod
    {
        public int Id { get; set; }
        public string DateString { get; set; } 
        public DateTime Date { get; set; }

        public ICollection<CategoryCount> CategoryCounts { get; set; }

        public TimePeriod()
        {
            CategoryCounts = new List<CategoryCount>();
        }


    }
}