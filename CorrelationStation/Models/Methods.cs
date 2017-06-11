using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Meta.Numerics;
using System.IO;
using CsvHelper;
using System.Data.Entity;
using System.Web.Mvc;
using Microsoft.VisualBasic.FileIO;

namespace CorrelationStation.Models
{
    public class Methods
    {

        ApplicationDbContext _context;

        public Methods()
        {
            _context = new ApplicationDbContext();
        }

        public static void ProcessLine(string[] values, Dictionary<int, string> columnIndex, Dictionary<string, List<string>> dictFile)
        {
            //string[] values = line.Split(',');

            for (var i = 0; i < values.Length; i++)
            {
                if(columnIndex.ContainsKey(i))
                {
                    var keyForFile = columnIndex[i];
                    dictFile[keyForFile].Add(values[i]);
                }
                
            }
        }

        internal void DeleteRecords()
        {

            var itemsToDelete = _context.Set<KeyValue>();
            _context.KeyValues.RemoveRange(itemsToDelete);


            var itemsToDelete2 = _context.Set<ChiStats>();
            _context.ChiStats.RemoveRange(itemsToDelete2);


            var itemsToDelete3 = _context.Set<PearsonCorr>();
            _context.PearsonCorrs.RemoveRange(itemsToDelete3);

            var itemsToDelete4 = _context.Set<AnovaStats>();
            _context.AnovaStats.RemoveRange(itemsToDelete4);

            var itemsToDelete5 = _context.Set<StatSummaryVM>();
            _context.StatSummaryVMs.RemoveRange(itemsToDelete5);

            _context.SaveChanges();
        }


        public static List<double[]> ProcessScatterPlotRequest(string data1, string data2)
        {
            List<double> dataOne = data1.Split(',').Select(x => double.Parse(x)).ToList();
            List<double> dataTwo = data2.Split(',').Select(x => double.Parse(x)).ToList();


            List<double[]> dataPairs = new List<double[]>();

            for(var i = 0; i < dataOne.Count; i++)
            {
                dataPairs.Add(new double[2] { dataOne[i], dataTwo[i] });
            }

            return dataPairs;
        }

        public static string ConcatAlph(string a, string b)
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

        public static Dictionary<string, List<string>> CsvToDictionary(string path)
        {
            Dictionary<int, string> columnIndex = new Dictionary<int, string>();

            Dictionary<string, List<string>> dictFile = new Dictionary<string, List<string>>();


            string line1 = System.IO.File.ReadLines(path).First();

            string[] values = line1.Split(',');

            for (var i = 0; i < values.Length; i++)
            {

                columnIndex.Add(i, values[i].Replace("\"", ""));
                dictFile.Add(values[i].Replace("\"", ""), new List<string>());
            }


            using (TextReader fileReader = System.IO.File.OpenText(path))
            {
                var parser = new CsvParser(fileReader);
                parser.Read();
                while (true)
                {
                    var row = parser.Read();
                    if (row == null)
                    {
                        break;
                    }
                    Methods.ProcessLine(row, columnIndex, dictFile);
                }
            }

            return dictFile;

        }

        public static SelectTypeVM MakeSelectType(UploadVM vm, string path)
        {
            var fileName = Path.GetFileName(vm.File.FileName);

            vm.File.SaveAs(path);

            SelectTypeVM selectTypeVM = new SelectTypeVM();
            selectTypeVM.Path = path;
            selectTypeVM.ColumnTypes = new Dictionary<string, string>();
            string line1 = System.IO.File.ReadLines(path).First();

            string[] values = line1.Split(',');

            for (var i = 0; i < values.Length; i++)
            {
                selectTypeVM.ColumnTypes.Add(values[i].Replace("\"", ""), "");
            }

            selectTypeVM.Types = new SelectList(new List<string> { "categorical", "numeral", "exclude" });

            List<List<string>> firstFive = new List<List<string>>();

            //***************
            using (TextReader fileReader = System.IO.File.OpenText(path))
            {
                var parser = new CsvParser(fileReader);
                firstFive.Add(parser.Read().ToList());
                int count = 0;
                while (count < 5)
                {
                    var row = parser.Read();
                    firstFive.Add(row.ToList());
                    count += 1;
                }
            }

            //*******************
            selectTypeVM.FirstFiveRows = firstFive;

            return selectTypeVM;
        }


        public static StatSummaryVM GetSummaryVM(Dictionary<string, List<string>> dictFile, SelectTypeVM vm)
        {
            ApplicationDbContext _context = new ApplicationDbContext();

            List<string> checkedColumns = new List<string>();
            List<ChiStats> chiStatsList = new List<ChiStats>();
            List<AnovaStats> anovaStatsList = new List<AnovaStats>();
            List<PearsonCorr> pearsonCorrelations = new List<PearsonCorr>();
            StatSummaryVM statsSummary = new StatSummaryVM();
            foreach (KeyValuePair<string, List<string>> entry in dictFile)
            {
                foreach (KeyValuePair<string, List<string>> entryCompare in dictFile)
                {
                    var checkedKey = Methods.ConcatAlph(entry.Key, entryCompare.Key);

                    if (entry.Key == entryCompare.Key ||
                        checkedColumns.Contains(checkedKey) || 
                        (entry.Value.Count != entryCompare.Value.Count) ||
                        (vm.ColumnTypes[entry.Key] == "exclude" || vm.ColumnTypes[entryCompare.Key] == "exclude")) 
                    {
                        continue;
                    }

                    if (vm.ColumnTypes[entry.Key] == "categorical" && vm.ColumnTypes[entryCompare.Key] == "categorical")
                    {
                        ChiStats chiStats = new ChiStats();

                        chiStats.GetChiStat(entry.Value, entryCompare.Value);

                        chiStats.Variable1 = entryCompare.Key.Replace("\"", "");
                        chiStats.Variable2 = entry.Key.Replace("\"", "");

                        chiStatsList.Add(chiStats);
                        _context.ChiStats.Add(chiStats);
                        _context.SaveChanges();

                    }

                    if ((vm.ColumnTypes[entry.Key] == "categorical" || vm.ColumnTypes[entryCompare.Key] == "categorical")
                        &&
                        (vm.ColumnTypes[entry.Key] == "numeral" || vm.ColumnTypes[entryCompare.Key] == "numeral"))
                    {

                        AnovaStats anovaStats = new AnovaStats();

                        if (vm.ColumnTypes[entry.Key] == "numeral")
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

                        //anovaStats.SaveStat();
                        _context.AnovaStats.Add(anovaStats);
                        _context.SaveChanges();
                    }

                    if (vm.ColumnTypes[entry.Key] == "numeral" && vm.ColumnTypes[entryCompare.Key] == "numeral")
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
            statsSummary.Path = vm.Path;
            statsSummary.FileName = vm.FileName;
            
            
            return statsSummary;
        }

        public static void SaveStatSummary(StatSummaryVM ss, string userId)
        {
            ApplicationDbContext _context = new ApplicationDbContext();

            if(userId != null)
            {
                ApplicationUser user = _context.Users.Include(u => u.StatSummaries).SingleOrDefault(u => u.Id == userId);
                ss.Author = user.UserName;
                user.StatSummaries.Add(ss);
 
            }

            _context.StatSummaryVMs.Add(ss);
            _context.SaveChanges();
        }

        public static StatSummaryVM GetSummaryById(int id)
        {
            ApplicationDbContext _context = new ApplicationDbContext();

            return _context.StatSummaryVMs.Include(s => s.AnovaStats)
                .Include(s => s.ChiStats)
                .Include(s => s.PearsonCorrs)
                .SingleOrDefault(s => s.Id == id);
        }

        public static List<StatSummaryVM> GetUserSummaries(string userId)
        {
            ApplicationDbContext _context = new ApplicationDbContext();
            ApplicationUser user = _context.Users.SingleOrDefault(u => u.Id == userId);

            List<StatSummaryVM> statSummaries = _context.StatSummaryVMs.Include(s => s.AnovaStats)
                                                                        .Include(s => s.ChiStats)
                                                                        .Include(s => s.PearsonCorrs)
                                                                        .Include(s => s.ApplicationUsers)
                                                                        .Where(s => s.ApplicationUsers.Any(u => u.Id == userId))
                                                                        .ToList();
            return statSummaries;
        }

        public static List<StatSummaryVM> GetAllSummaries()
        {
            ApplicationDbContext _context = new ApplicationDbContext();

            List<StatSummaryVM> statSummaries = _context.StatSummaryVMs.Include(s => s.AnovaStats)
                                                                        .Include(s => s.ChiStats)
                                                                        .Include(s => s.PearsonCorrs)
                                                                        .Include(s => s.ApplicationUsers)
                                                                        .ToList();
            return statSummaries;
        }

        public static bool CheckIfReportSaved(string userId, int id)
        {
            ApplicationDbContext _context = new ApplicationDbContext();

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