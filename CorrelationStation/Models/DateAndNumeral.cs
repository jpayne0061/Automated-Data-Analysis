using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorrelationStation.Models
{
    public class DateAndNumeral
    {
        public int Id { get; set; }
        public string DateData { get; set; }
        public string NumeralData { get; set; }

        public string DateName { get; set; }
        public string NumeralName { get; set; }

        public void MakeDataBlob(List<string> dates, List<string> numerals)
        {

            DateData = String.Join(",", dates);
            NumeralData = String.Join(",", numerals);

        }


    }
}