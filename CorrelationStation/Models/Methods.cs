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


        public static string GetExceptionMessage(Dictionary<string, List<string>> invalidColumns)
        {
            string exception = "<p class='customr-error'>*Invalid data found*" + "<br/>";

            foreach (KeyValuePair<string, List<string>> kvp in invalidColumns)
            {
                string exceptionLine = "";

                exceptionLine += kvp.Key + ": ";
                for (var i = 0; i < kvp.Value.Count; i++)
                {
                    if (i == kvp.Value.Count - 1)
                    {
                        exceptionLine += kvp.Value[i] + " " + "<br/>";
                    }
                    else
                    {
                        exceptionLine += kvp.Value[i] + ", ";
                    }

                }

                exception += exceptionLine;
            }
            return exception + "</p>";
        }


        public static Dictionary<string, List<string>> CheckForInvalidColumns(Dictionary<string, string> columnTypes, List<List<string>> firstFive)
        {
            Dictionary<string, List<string>> invalidColumns = new Dictionary<string, List<string>>();
            //int index;
            foreach(KeyValuePair<string, string> kvp in columnTypes)
            {
                if(kvp.Value == "numeral")
                {
                    int index = firstFive[0].IndexOf(kvp.Key);
                    for(var i = 1; i < 5; i++)
                    {
                        double x;
                        if(!double.TryParse(firstFive[i][index], out x))
                        {
                            if(invalidColumns.ContainsKey(kvp.Key))
                            {
                                invalidColumns[kvp.Key].Add(firstFive[i][index]);
                            }
                            else
                            {
                                invalidColumns.Add(kvp.Key, new List<string> { firstFive[i][index] });
                            }
                        }
                    }

                }
            }

            return invalidColumns;
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

        public static void MakeDropDownAndFirstFive(SelectTypeVM selectTypeVM)
        {
            selectTypeVM.Types = new SelectList(new List<string> { "categorical", "numeral", "exclude", "date time" });

            List<List<string>> firstFive = new List<List<string>>();

            //***************
            using (TextReader fileReader = System.IO.File.OpenText(selectTypeVM.Path))
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

        }


        public static SelectTypeVM MakeSelectType(UploadVM vm, string path)
        {
            var fileName = Path.GetFileName(vm.File.FileName);

            vm.File.SaveAs(path);

            SelectTypeVM selectTypeVM = new SelectTypeVM();
            selectTypeVM.Path = path;
            selectTypeVM.ColumnTypes = new Dictionary<string, string>();

            string line1 = System.IO.File.ReadLines(selectTypeVM.Path).First();

            string[] values = line1.Split(',');

            for (var i = 0; i < values.Length; i++)
            {
                selectTypeVM.ColumnTypes.Add(values[i].Replace("\"", ""), "");
            }

            MakeDropDownAndFirstFive(selectTypeVM);

            return selectTypeVM;
        }


        public static StatSummaryVM GetSummaryVM(Dictionary<string, List<string>> dictFile, SelectTypeVM vm)
        {
            ApplicationDbContext _context = new ApplicationDbContext();

            List<string> checkedColumns = new List<string>();
            List<ChiStats> chiStatsList = new List<ChiStats>();
            List<AnovaStats> anovaStatsList = new List<AnovaStats>();
            List<PearsonCorr> pearsonCorrelations = new List<PearsonCorr>();
            List<DateAndCategory> dateAndCategories = new List<DateAndCategory>();

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


                    if ((vm.ColumnTypes[entry.Key] == "date time" || vm.ColumnTypes[entryCompare.Key] == "date time")
                        &&
                        (vm.ColumnTypes[entry.Key] == "categorical" || vm.ColumnTypes[entryCompare.Key] == "categorical"))
                    {

                        DateAndCategory dateCat = new DateAndCategory();

                        if(vm.ColumnTypes[entry.Key] == "date time")
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
                        _context.SaveChanges();
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
            statsSummary.DateAndCatories = dateAndCategories;
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
                .Include(s => s.DateAndCatories)
                .SingleOrDefault(s => s.Id == id);
        }

        public static List<StatSummaryVM> GetUserSummaries(string userId)
        {
            ApplicationDbContext _context = new ApplicationDbContext();
            ApplicationUser user = _context.Users.SingleOrDefault(u => u.Id == userId);

            List<StatSummaryVM> statSummaries = _context.StatSummaryVMs.Include(s => s.AnovaStats)
                                                                        .Include(s => s.ChiStats)
                                                                        .Include(s => s.PearsonCorrs)
                                                                        .Include(s => s.DateAndCatories)
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
                                                                        .Include(s => s.DateAndCatories)
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