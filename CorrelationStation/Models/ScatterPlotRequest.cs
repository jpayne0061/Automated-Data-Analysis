using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorrelationStation.Models
{
    public class ScatterPlotRequest
    {
        public string Variable1 { get; set; }
        public string Variable2 { get; set; }
        public string Path { get; set; }
        public int StatId { get; set; }
        public string Switch { get; set; }
    }
}