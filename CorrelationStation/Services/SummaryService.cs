using CorrelationStation.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CorrelationStation.Services
{
    public class SummaryService
    {
        private static readonly string EXCLUDE = "exclude";
        private static readonly string DATE_TIME = "date time";
        private static readonly string CATEGORICAL = "categorical";
        private static readonly string NUMERAL = "numeral";

        ApplicationDbContext _context;

        public SummaryService()
        {
            _context = new ApplicationDbContext();
        }

        public string CompareAndConcat(string a, string b)
        {
            if (string.Compare(a, b) == -1)
            {
                return a + b;
            }
            else
            {
                return b + a;
            }
        }

        public StatSummaryVM GetSummaryVM(Dictionary<string, List<string>> dictFile, SelectTypeVM vm)
        {
            
            List<string> checkedColumns = new List<string>();
            List<ChiStats> chiStatsList = new List<ChiStats>();
            List<AnovaStats> anovaStatsList = new List<AnovaStats>();
            List<PearsonCorr> pearsonCorrelations = new List<PearsonCorr>();
            List<DateAndCategory> dateAndCategories = new List<DateAndCategory>();
            List<DateAndNumeral> dateAndNumerals = new List<DateAndNumeral>();

            StatSummaryVM statsSummary = new StatSummaryVM();
            foreach (KeyValuePair<string, List<string>> entry in dictFile)
            {
                foreach (KeyValuePair<string, List<string>> entryCompare in dictFile)
                {
                    var checkedKey = CompareAndConcat(entry.Key, entryCompare.Key);

                    if (entry.Key == entryCompare.Key ||
                        checkedColumns.Contains(checkedKey) ||
                        (entry.Value.Count != entryCompare.Value.Count) ||
                        (vm.ColumnTypes[entry.Key] == EXCLUDE || vm.ColumnTypes[entryCompare.Key] == EXCLUDE))
                    {
                        continue;
                    }


                    if ((vm.ColumnTypes[entry.Key] == DATE_TIME || vm.ColumnTypes[entryCompare.Key] == DATE_TIME)
                        &&
                        (vm.ColumnTypes[entry.Key] == CATEGORICAL || vm.ColumnTypes[entryCompare.Key] == CATEGORICAL))
                    {

                        DateAndCategory dateCat = new DateAndCategory();

                        if (vm.ColumnTypes[entry.Key] == DATE_TIME)
                        {
                            dateCat.GetLinePlotData(entry.Value, entryCompare.Value);
                        }
                        else
                        {
                            dateCat.GetLinePlotData(entryCompare.Value, entry.Value);
                        }
                        dateCat.Variable1 = entry.Key;
                        dateCat.Variable2 = entryCompare.Key;

                        dateAndCategories.Add(dateCat);
                    }

                    if ((vm.ColumnTypes[entry.Key] == DATE_TIME || vm.ColumnTypes[entryCompare.Key] == DATE_TIME)
                        &&
                        (vm.ColumnTypes[entry.Key] == NUMERAL || vm.ColumnTypes[entryCompare.Key] == NUMERAL))
                    {

                        DateAndNumeral dateNum = new DateAndNumeral();

                        if (vm.ColumnTypes[entry.Key] == DATE_TIME)
                        {
                            dateNum.MakeDataBlob(entry.Value, entryCompare.Value);
                            dateNum.DateName = entry.Key;
                            dateNum.NumeralName = entryCompare.Key;
                        }
                        else
                        {
                            dateNum.MakeDataBlob(entryCompare.Value, entry.Value);
                            dateNum.DateName = entryCompare.Key;
                            dateNum.NumeralName = entry.Key;
                        }


                        dateAndNumerals.Add(dateNum);
                    }


                    if (vm.ColumnTypes[entry.Key] == CATEGORICAL && vm.ColumnTypes[entryCompare.Key] == CATEGORICAL)
                    {
                        ChiStats chiStats = new ChiStats();

                        chiStats.GetChiStat(entry.Value, entryCompare.Value);

                        chiStats.Variable1 = entryCompare.Key.Replace("\"", "");
                        chiStats.Variable2 = entry.Key.Replace("\"", "");

                        chiStatsList.Add(chiStats);
                    }

                    if ((vm.ColumnTypes[entry.Key] == CATEGORICAL || vm.ColumnTypes[entryCompare.Key] == CATEGORICAL)
                        &&
                        (vm.ColumnTypes[entry.Key] == NUMERAL || vm.ColumnTypes[entryCompare.Key] == NUMERAL))
                    {

                        AnovaStats anovaStats = new AnovaStats();

                        if (vm.ColumnTypes[entry.Key] == NUMERAL)
                        {
                            anovaStats.GetAnovaStats(entryCompare.Value, entry.Value);
                            anovaStats.CategoricalVariable = entryCompare.Key;
                            anovaStats.NumericalVariable = entry.Key;
                            anovaStatsList.Add(anovaStats);

                        }
                        else
                        {
                            anovaStats.GetAnovaStats(entry.Value, entryCompare.Value);
                            anovaStats.CategoricalVariable = entry.Key;
                            anovaStats.NumericalVariable = entryCompare.Key;
                            anovaStatsList.Add(anovaStats);
                        }

                        _context.AnovaStats.Add(anovaStats);
                        _context.SaveChanges();
                    }

                    if (vm.ColumnTypes[entry.Key] == NUMERAL && vm.ColumnTypes[entryCompare.Key] == NUMERAL)
                    {
                        PearsonCorr pearsonCorr = new PearsonCorr();
                        pearsonCorr.Variable1 = entry.Key;
                        pearsonCorr.Variable2 = entryCompare.Key;
                        pearsonCorr.ComputeCoeff(entry.Value, entryCompare.Value);

                        pearsonCorrelations.Add(pearsonCorr);

                        _context.SaveChanges();
                    }

                    checkedColumns.Add(checkedKey);

                }
            }
            statsSummary.Name = vm.Name != null ? vm.Name : "Untitled";
            statsSummary.Description = vm.Description != null ? vm.Description : "no description available";

            statsSummary.AnovaStats = anovaStatsList;
            statsSummary.ChiStats = chiStatsList;
            statsSummary.PearsonCorrs = pearsonCorrelations;
            statsSummary.DateAndCatories = dateAndCategories;
            statsSummary.DateAndNumerals = dateAndNumerals;
            statsSummary.Path = vm.Path;
            statsSummary.FileName = vm.FileName;

            return statsSummary;
        }

        public void SaveStatSummary(StatSummaryVM ss, string userId)
        {
            if (userId != null)
            {
                ApplicationUser user = _context.Users.Include(u => u.StatSummaries).SingleOrDefault(u => u.Id == userId);
                ss.Author = user.UserName;
                user.StatSummaries.Add(ss);
            }

            _context.StatSummaryVMs.Add(ss);
            _context.SaveChanges();
        }

        public StatSummaryVM GetSummaryById(int id)
        {
            return _context.StatSummaryVMs.Include(s => s.AnovaStats)
                .Include(s => s.ChiStats)
                .Include(s => s.PearsonCorrs)
                .Include(s => s.DateAndCatories)
                .Include(s => s.DateAndNumerals)
                .SingleOrDefault(s => s.Id == id);
        }

        public List<StatSummaryVM> GetUserSummaries(string userId)
        {
            ApplicationUser user = _context.Users.SingleOrDefault(u => u.Id == userId);

            List<StatSummaryVM> statSummaries = _context.StatSummaryVMs.Include(s => s.AnovaStats)
                                                                        .Include(s => s.ChiStats)
                                                                        .Include(s => s.PearsonCorrs)
                                                                        .Include(s => s.DateAndCatories)
                                                                        .Include(s => s.DateAndNumerals)
                                                                        .Include(s => s.ApplicationUsers)
                                                                        .Where(s => s.ApplicationUsers.Any(u => u.Id == userId))
                                                                        .ToList();
            return statSummaries;
        }

        public List<StatSummaryVM> GetAllSummaries()
        {
            List<StatSummaryVM> statSummaries = _context.StatSummaryVMs.Include(s => s.AnovaStats)
                                                                        .Include(s => s.ChiStats)
                                                                        .Include(s => s.PearsonCorrs)
                                                                        .Include(s => s.DateAndCatories)
                                                                        .Include(s => s.DateAndNumerals)
                                                                        .Include(s => s.ApplicationUsers)
                                                                        .ToList();
            return statSummaries;
        }

        public bool CheckIfReportSaved(string userId, int id)
        {
            int count = _context.Users
                            .Include(u => u.StatSummaries)
                            .SingleOrDefault(u => u.Id == userId)
                            .StatSummaries.Where(ss => ss.Id == id)
                            .Count();
            if (count > 0)
                return true;
            else
                return false;

        }
    }
}