using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;


namespace CorrelationStation.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ChiStats> ChiStats { get; set; }
        public DbSet<AnovaStats> AnovaStats { get; set; }
        public DbSet<PearsonCorr> PearsonCorrs { get; set; }
        public DbSet<KeyValue> KeyValues { get; set; }
        public DbSet<StatSummaryVM> StatSummaryVMs { get; set; }
        public DbSet<AnInt> AnInts { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }


    }
}