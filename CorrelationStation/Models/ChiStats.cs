using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorrelationStation.Models
{
    public class ChiStats
    {
        public int Id { get; set; }
        public List<KeyValue> ExpectedValues { get; set; }
        public List<KeyValue> ObservedValues { get; set; }

        public string Variable1 { get; set; }
        public string Variable2 { get; set; }

        public List<KeyValue> VariableCategories { get; set; }
        public List<KeyValue> Variable2Categories { get; set; }

        public List<KeyValue> ExpectedPercentage { get; set; }
        public List<KeyValue> ObservedPercentage { get; set; }


        public double PValue { get; set; }
        public double ChiStatistic { get; set; }

        public bool SignificantResult { get; set; }

        public bool HighDF { get; set; }

        public void GetChiStat(List<string> values, List<string> values2)
        {
            //values get %
            //values2 get # of each catgeory
            Dictionary<string, double> valuesCategories = new Dictionary<string, double>();
            Dictionary<string, int> values2Categories = new Dictionary<string, int>();
            Dictionary<string, double> values2Percentage = new Dictionary<string, double>();

            for (var i = 0; i < values.Count; i++)
            {
                if (values[i] == "" || values2[i] == "" || values[i] == null || values2[i] == null)
                    continue;

                if (!valuesCategories.ContainsKey(values[i]))
                {
                    valuesCategories.Add(values[i], 1);
                }
                else
                {
                    valuesCategories[values[i]] += 1;
                }

                if (!values2Categories.ContainsKey(values2[i]))
                {
                    values2Categories.Add(values2[i], 1);
                }
                else
                {
                    values2Categories[values2[i]] += 1;
                }

            }

            if(valuesCategories.Count * values2Categories.Count > 5000 )
            {
                HighDF = true;

                return;
            }

            int degreesFreedom = (valuesCategories.Count - 1) * (values2Categories.Count - 1);

            //convert valuesCategories to percentage
            int totalForValues = values.Count;
            var keys = new List<string>(valuesCategories.Keys);
            foreach (string key in keys)
            {
                valuesCategories[key] = valuesCategories[key] / totalForValues;
            }

            var keys2 = new List<string>(values2Categories.Keys);
            foreach (string key in keys2)
            {
                //values2Percentage[key] = values2Categories[key] / totalForValues;
                values2Percentage.Add(key, (double)values2Categories[key] / (double)totalForValues);
            }


            //build dictionary of expected values for each category of selected variable/column
            Dictionary<string, double> expectedValues = new Dictionary<string, double>();
            Dictionary<string, int> observedCounts = new Dictionary<string, int>();
            Dictionary<string, double> observedPercentages = new Dictionary<string, double>();
            Dictionary<string, double> expectedPercentages = new Dictionary<string, double>();


            //this loop may be able to replace the ones below---subtractedSquaredDivided 
            foreach (KeyValuePair<string, double> kvp in valuesCategories)
            {
                foreach (KeyValuePair<string, int> kvp2 in values2Categories)
                {
                    expectedValues.Add(kvp.Key + " " + kvp2.Key, kvp.Value * kvp2.Value);
                    observedCounts.Add(kvp.Key + " " + kvp2.Key, 0);
                }
            }

            foreach (KeyValuePair<string, double> kvp in valuesCategories)
            {
                foreach (KeyValuePair<string, double> kvp2 in values2Percentage)
                {
                    expectedPercentages.Add(kvp.Key + " " + kvp2.Key, kvp.Value * kvp2.Value);
                    //observedCounts.Add(kvp.Key + " " + kvp2.Key, 0);
                }
            }



            //AT THIS POINT WE HAVE THE DICTIONARY OF EXPECTED OUTCOMES
            //dICTIONARY = KVP["WHITE CITATION"]=>(46), KVP["bLACK WARNING"]=>(21)

            //next:
            //find actual values/occurances
            //build dictionary/table of possible combinations of categories and count for each
            //Dictionary<string, int> observedCounts = new Dictionary<string, int>();

            for (var i = 0; i < values.Count; i++)
            {
                if (values[i] == "" || values2[i] == "" || values[i] == null || values2[i] == null)
                    continue;

                //for(var j = 0; j < values.Count; j++)
                //{
                if (!observedCounts.ContainsKey(values[i] + " " + values2[i]))
                {
                    observedCounts.Add(values[i] + " " + values2[i], 1);
                }
                else
                {
                    observedCounts[values[i] + " " + values2[i]] += 1;
                }
            }


            List<string> observedKeys = new List<string>(observedCounts.Keys);

            foreach (var key in observedKeys)
            {
                observedPercentages.Add(key, (double)observedCounts[key] / (double)totalForValues);
            }


            Dictionary<string, double> valuesSubtractedSquaredDivided = new Dictionary<string, double>();

            foreach (KeyValuePair<string, int> kvp in observedCounts)
            {
                valuesSubtractedSquaredDivided.Add(kvp.Key, Math.Pow((observedCounts[kvp.Key] - expectedValues[kvp.Key]), 2) / expectedValues[kvp.Key]);
            }

            double chiStat = 0;


            //this loop can be removed and included in the loop above
            foreach (KeyValuePair<string, double> kvp in valuesSubtractedSquaredDivided)
            {
                chiStat += kvp.Value;
            }

            ObservedValues = new List<KeyValue>();
            ExpectedValues = new List<KeyValue>();
            foreach (KeyValuePair<string, double> kvp in expectedValues)
            {
                ExpectedValues.Add(new KeyValue { Key = kvp.Key, Value = kvp.Value });
                ObservedValues.Add(new KeyValue { Key = kvp.Key, Value = observedCounts[kvp.Key] });
            }

            VariableCategories = new List<KeyValue>();
            foreach (KeyValuePair<string, double> kvp in valuesCategories)
            {
                VariableCategories.Add(new KeyValue { Key = kvp.Key, Value = kvp.Value });
            }

            Variable2Categories = new List<KeyValue>();
            foreach (KeyValuePair<string, int> kvp in values2Categories)
            {
                Variable2Categories.Add(new KeyValue { Key = kvp.Key, Value = kvp.Value });
            }

            ExpectedPercentage = new List<KeyValue>();
            ObservedPercentage = new List<KeyValue>();
            foreach (KeyValuePair<string, double> kvp in expectedPercentages)
            {
                ExpectedPercentage.Add(new KeyValue { Key = kvp.Key, Value = kvp.Value });
                ObservedPercentage.Add(new KeyValue { Key = kvp.Key, Value = observedPercentages[kvp.Key] });
            }

            PValue = ChiSquareUtils.pochisq(chiStat, degreesFreedom);
            ChiStatistic = Math.Round(chiStat, 2);
            HighDF = false;
            SignificantResult = PValue < 0.05;

        }

    }




}