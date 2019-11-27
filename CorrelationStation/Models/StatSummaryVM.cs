using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorrelationStation.Models
{
    public class StatSummaryVM
    {

        public int Id { get; set; }
        public List<AnovaStats> AnovaStats { get; set; }
        public List<ChiStats> ChiStats { get; set; }
        public List<PearsonCorr> PearsonCorrs { get; set; }
        public List<DateAndCategory> DateAndCatories { get; set; }
        public List<DateAndNumeral> DateAndNumerals { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Name { get; set; }


        public ICollection<ApplicationUser> ApplicationUsers { get; set; }

        public StatSummaryVM()
        {
            ApplicationUsers = new List<ApplicationUser>();
        }

    }
}