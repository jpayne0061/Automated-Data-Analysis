using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using CorrelationStation.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using CorrelationStation.Services;

namespace CorrelationStation.Controllers
{
    public class DataPointsController : ApiController
    {
        private ApplicationDbContext _context;
        private ParsingService _parsingService;
        private ScatterPlotService _scatterPlotService;
        private SummaryService _summaryService;

        public DataPointsController()
        {
            _context = new ApplicationDbContext();
            _parsingService = new ParsingService();
            _scatterPlotService = new ScatterPlotService();
            _summaryService = new SummaryService();
        }


        [HttpPost]
        public List<double[]> ReturnPearson([FromBody] ScatterPlotRequest scatterPlotRequest)
        {
            int statId = scatterPlotRequest.StatId;

            PearsonCorr pearsonCorr = _context.PearsonCorrs.SingleOrDefault(ss => ss.Id == statId);
            List<double[]> dataPairs;

            if (scatterPlotRequest.Switch == "false")
            {
                dataPairs = _scatterPlotService.ProcessScatterPlotRequest(pearsonCorr.Variable1Data, pearsonCorr.Variable2Data);
            }
            else
            {
                dataPairs = _scatterPlotService.ProcessScatterPlotRequest(pearsonCorr.Variable2Data, pearsonCorr.Variable1Data);
            }

            return dataPairs;
        }

        [HttpGet]
        public List<DateAndNumeralValues> GetNumeralLinePlot(int id)
        {
            DateAndNumeral dn = _context.DateAndNumerals.SingleOrDefault(x => x.Id == id);

            List<string> dates = dn.DateData.Split(',').ToList();
            List<string> nums = dn.NumeralData.Split(',').ToList();

            List<DateAndNumeralValues> dns = _parsingService.ProcessDateNumerals(dates, nums).OrderBy(x => x.Date).ToList();

            return dns;
        }

        [HttpGet]
        public List<List<KeyValue>> GetChiTablesData(int id)
        {
            var values = new List<List<KeyValue>>();
            var compare = new List<List<KeyValue>>();
            var significant = new List<KeyValue>();

            var chiStat = _context.ChiStats.Include(c => c.ExpectedValues)
                                            .Include(c => c.ObservedValues)
                                            .Include(c => c.VariableCategories)
                                            .Include(c => c.Variable2Categories)
                                            .SingleOrDefault(c => c.Id == id);

            int count = chiStat.ExpectedValues.Count;

            for(var i = 0; i < count; i++)
            {
                compare.Add(new List<KeyValue> { chiStat.ExpectedValues[i], chiStat.ObservedValues[i] });
            }

            List<List<KeyValue>> compareDescending = compare.OrderByDescending(ls => (Math.Abs(ls[0].Value - ls[1].Value))).Take((int)(count*.05)).ToList();
            foreach(var kv in compareDescending)
            {
                significant.Add(kv[0]);
            }


            values.Add(chiStat.ExpectedValues);
            values.Add(chiStat.ObservedValues);
            values.Add(chiStat.VariableCategories);
            values.Add(chiStat.Variable2Categories);
            values.Add(significant);


            return values;
        }

        public List<List<KeyValue>> GetChiPercentages(int id)
        {
            var percentages = new List<List<KeyValue>>();

            var chiStat = _context.ChiStats.Include(c => c.ObservedPercentage)
                                            .Include(c => c.ExpectedPercentage)
                                            .SingleOrDefault(c => c.Id == id);

            int count = chiStat.ObservedPercentage.Count; 

            for(var i = 0; i < count; i++)
            {
                percentages.Add(new List<KeyValue> { chiStat.ObservedPercentage[i], chiStat.ExpectedPercentage[i] });
            }

            List<List<KeyValue>> percentagesDescending = percentages.OrderByDescending(ls => (Math.Abs(ls[0].Value - ls[1].Value))).ToList();

            return percentagesDescending;
        }

        public List<KeyValue> GetAnovaMeans(int id)
        {
            var anovaStat = _context.AnovaStats.Include(a => a.Means)
                                                .SingleOrDefault(a => a.Id == id);

            return anovaStat.Means;
        }


        //GetDateCategoryLinePlot
        [HttpGet]
        public ICollection<LinePlotCategory> GetDateCategoryLinePlot(int id)
        {
            var dateCat = _context.DateAndCategories.Include(d => d.LinePlotCategories.Select(t => t.DateAndCounts))
                                                .SingleOrDefault(a => a.Id == id);

            foreach(LinePlotCategory lineCat in dateCat.LinePlotCategories)
            {
                lineCat.DateAndCounts = lineCat.DateAndCounts.OrderBy(x => x.DateTime).ToList();
            }
            return dateCat.LinePlotCategories;
        }



        [HttpGet]
        public IHttpActionResult SaveToReports(int id)
        {
            string userId = User.Identity.GetUserId();

            ApplicationUser user = _context.Users.Include(u => u.StatSummaries).SingleOrDefault(u => u.Id == userId);
            StatSummaryVM statSummary = _context.StatSummaryVMs.SingleOrDefault(ss => ss.Id == id);

            if(!_summaryService.CheckIfReportSaved(userId, statSummary.Id))
            {
                statSummary.ApplicationUsers.Add(user);
                user.StatSummaries.Add(statSummary);
                _context.SaveChanges();
            }

            return Ok();
        }

    }
}
