using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorrelationStation.Models
{
    public class DateAndCount
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public DateTime DateTime { get; set; }
        public string MonthAndYear { get; set; }
        public int Count { get; set; }

        public LinePlotCategory linePlotCategory { get; set; }


    }
}