using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorrelationStation.Models
{
    public class PearsonCorr
    {
        public int Id { get; set; }

        public double r { get; set; }
        public string Variable1 { get; set; }
        public string Variable1Data { get; set; }

        public string Variable2 { get; set; }
        public string Variable2Data { get; set; }

        public bool SignificantResult { get; set; }

        public void ComputeCoeff(List<string> values, List<string> valuesTwo)
        {
            int length = values.Count();
            List<double> values1 = new List<double>();
            List<double> values2 = new List<double>();

            for (var i = 0; i < length; i++ )
            {
                double numeral1;
                double numeral2;

                if(double.TryParse(values[i], out numeral1) && double.TryParse(valuesTwo[i], out numeral2))
                {
                    values1.Add(numeral1);
                    values2.Add(numeral2);
                }

            }


            if (values1.Count != values2.Count)
                throw new ArgumentException("values must be the same length");

            var avg1 = values1.Average();
            var avg2 = values2.Average();

            var sum1 = values1.Zip(values2, (x1, y1) => (x1 - avg1) * (y1 - avg2)).Sum();

            var sumSqr1 = values1.Sum(x => Math.Pow((x - avg1), 2.0));
            var sumSqr2 = values2.Sum(y => Math.Pow((y - avg2), 2.0));

            var result = sum1 / Math.Sqrt(sumSqr1 * sumSqr2);

            r = result;
            SignificantResult = r > 0.35;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            // the code that you want to measure comes here



            //Variable1Data = values1.Select(v => new AnInt { Number = v}).ToList();
            //Variable2Data = values2.Select(v => new AnInt { Number = v }).ToList();

            Variable1Data = String.Join(",", values1);
            Variable2Data = String.Join(",", values2);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
        }

    }

}