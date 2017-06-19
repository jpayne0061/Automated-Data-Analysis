using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorrelationStation.Models
{
    public class LinePlotCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<DateAndCount> DateAndCounts { get; set; }

        public LinePlotCategory()
        {
            DateAndCounts = new List<DateAndCount>();
        }


    }
}