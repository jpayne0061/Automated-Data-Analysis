using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorrelationStation.Models
{
    public class AnovaStats
    {
        private ApplicationDbContext _context;

        public AnovaStats()
        {
            _context = new ApplicationDbContext();
        }

        public int Id { get; set; }

        public string CategoricalVariable { get; set; }
        public string NumericalVariable { get; set; }

        public double FStat { get; set; }
        public double CriticalValueAtAlphaZeroFive { get; set; }
        public bool SigAtPointZeroFive { get; set; }
        public bool SigAtPointZeroOne { get; set; }
        //public Dictionary<string, double> Means { get; set; }
        public List<KeyValue> Means { get; set; }


        public Dictionary<string, List<double>> Groups { get; set; }

        public bool SignificantResult {get; set; }

        public void GetAnovaStats(List<string> categories, List<string> numerals)
        {

            var anovaStats = new AnovaStats();

            Dictionary<string, List<double>> groups = new Dictionary<string, List<double>>();
            Dictionary<string, double> means = new Dictionary<string, double>();

            for (var i = 0; i < categories.Count; i++)
            {
                //need to clean numbers...exclude $ , and stuff like that

                if (categories[i] == "" || numerals[i] == "" || categories[i] == null || numerals[i] == null)
                    continue;

                double numeral;

                bool resultOfParse = double.TryParse(numerals[i], out numeral);

                if(resultOfParse)
                {
                    if (!groups.ContainsKey(categories[i]))
                    {
                        groups.Add(categories[i], new List<double> { numeral });
                    }
                    else
                    {
                        groups[categories[i]].Add(numeral);
                    }
                }


            }


            double ssb = 0;
            double ssw = 0;
            double sst = 0;
            int dfB;
            int dfW;
            double f;
            //df bt is #kvp -1
            //dfw 

            double indTotalSquare = 0;
            double totalSum = 0;
            int totalCount = 0;

            foreach (KeyValuePair<string, List<double>> kvp in groups)
            {
                var mean = kvp.Value.Sum() / kvp.Value.Count();
                means.Add(kvp.Key, mean);
                //for ssb
                indTotalSquare += (Math.Pow(kvp.Value.Sum(), 2)) / kvp.Value.Count();
                totalSum += kvp.Value.Sum();
                totalCount += kvp.Value.Count();
                //for ssw

                foreach (double num in kvp.Value)
                {
                    ssw += (Math.Pow(num - mean, 2));
                }

            }

            ssb = indTotalSquare - (Math.Pow(totalSum, 2) / totalCount);
            sst = ssb + ssw;

            dfB = groups.Count - 1;
            dfW = totalCount - groups.Count;

            f = (ssb / (double)dfB) / (ssw / (double)dfW);

            //Console.WriteLine("ssb: {0}, sst: {1}, dfB: {2}, dfW: {3}, ssw: {4}, f: {5}", ssb, sst, dfB, dfW, ssw, f);
            var keyValueMeans = new List<KeyValue>();

            foreach(KeyValuePair<string, double> kvp in means)
            {
                keyValueMeans.Add(new KeyValue { Key = kvp.Key, Value = kvp.Value });
            }

            FTable fTable = new FTable();
            CriticalValueAtAlphaZeroFive = fTable.sigAtZeroFive(dfB, dfW, f);

            SigAtPointZeroFive = f > CriticalValueAtAlphaZeroFive;
            SignificantResult = f > CriticalValueAtAlphaZeroFive;
            Means = keyValueMeans;
            Groups = groups;
            FStat = f;

        }

        public void SaveStat()
        {
            _context.AnovaStats.Add(this);
            _context.SaveChanges();

        }

    }
}