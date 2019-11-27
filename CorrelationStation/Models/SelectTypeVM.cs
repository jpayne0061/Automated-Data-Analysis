using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CorrelationStation.Models
{
  
    public class SelectTypeVM
    {
        public string Path { get; set; }
        public Dictionary<string, string> ColumnTypes { get; set; }
        public SelectList Types { get; set; }
        public List<List<string>> FirstFiveRows { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
    }
}