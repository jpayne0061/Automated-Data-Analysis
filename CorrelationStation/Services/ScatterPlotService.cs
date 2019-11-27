using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorrelationStation.Services
{
    public class ScatterPlotService
    {
        public List<double[]> ProcessScatterPlotRequest(string data1, string data2)
        {
            List<double>[] dataOne = RemoveOutliers(data1.Split(',').Select(x => double.Parse(x)).ToList(), data2.Split(',').Select(x => double.Parse(x)).ToList());

            int count = dataOne[0].Count();

            List<double[]> dataPairs = new List<double[]>();

            for (var i = 0; i < count; i++)
            {
                dataPairs.Add(new double[2] { dataOne[0][i], dataOne[1][i] });
            }

            return dataPairs;
        }

        public List<double>[] RemoveOutliers(List<double> nums, List<double> nums2)
        {
            List<double> numsOrdered = nums.OrderBy(x => x).ToList();

            double q3 = numsOrdered[(int)(numsOrdered.Count() * 0.75)];
            double q1 = numsOrdered[(int)(numsOrdered.Count() * 0.25)];
            double iqr = q3 - q1;

            List<double> numsOrdered2 = nums2.OrderBy(x => x).ToList();

            double q32 = numsOrdered2[(int)(numsOrdered2.Count() * 0.75)];
            double q12 = numsOrdered2[(int)(numsOrdered2.Count() * 0.25)];
            double iqr2 = q32 - q12;

            List<double> noOuts1 = new List<double>();
            List<double> noOuts2 = new List<double>();

            for (var i = 0; i < nums.Count; i++)
            {
                if (!(nums[i] > iqr * 2 + q3) && !(nums2[i] > iqr2 * 2 + q32))
                {
                    noOuts1.Add(nums[i]);
                    noOuts2.Add(nums2[i]);
                }
            }

            return new List<double>[] { noOuts1, noOuts2 };

        }
    }
}