using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using CsvHelper;
using CorrelationStation.Models;
using CorrelationStation.Helpers;

namespace CorrelationStation.Services
{
    public class ParsingService
    {
        public Dictionary<string, List<string>> CsvToDictionary(string path)
        {
            Dictionary<int, string> columnIndex = new Dictionary<int, string>();

            Dictionary<string, List<string>> dictFile = new Dictionary<string, List<string>>();


            string line1 = File.ReadLines(path).First();

            string[] values = line1.Split(',');

            for (var i = 0; i < values.Length; i++)
            {

                columnIndex.Add(i, values[i].Replace("\"", ""));
                dictFile.Add(values[i].Replace("\"", ""), new List<string>());
            }


            using (TextReader fileReader = File.OpenText(path))
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
                    ProcessLine(row, columnIndex, dictFile);
                }
            }

            return dictFile;
        }

        private void ProcessLine(string[] values, Dictionary<int, string> columnIndex, Dictionary<string, List<string>> dictFile)
        {
            for (var i = 0; i < values.Length; i++)
            {
                if (columnIndex.ContainsKey(i))
                {
                    var keyForFile = columnIndex[i];
                    dictFile[keyForFile].Add(values[i]);
                }

            }
        }

        public List<DateAndNumeralValues> ProcessDateNumerals(List<string> dates, List<string> numerals)
        {
            List<DateAndNumeralValues> dateNumPairs = new List<DateAndNumeralValues>();

            for (var i = 0; i < dates.Count; i++)
            {
                if ((dates[i] == null || dates[i] == "") || (numerals[i] == null || numerals[i] == ""))
                {
                    continue;
                }
                if (DateHelper.CheckIfDateValid(dates[i]))
                {
                    DateAndNumeralValues dn = new DateAndNumeralValues();
                    DateTime dt;
                    dn.Date = DateTime.TryParse(dates[i], out dt) ? dt : DateTime.Parse(dates[i] + "/1");
                    dn.Numeral = double.Parse(numerals[i]);
                    dateNumPairs.Add(dn);
                }
            }

            return dateNumPairs;
        }

        public Dictionary<string, List<string>> CheckForInvalidColumns(Dictionary<string, string> columnTypes, List<List<string>> firstFive)
        {
            Dictionary<string, List<string>> invalidColumns = new Dictionary<string, List<string>>();

            foreach (KeyValuePair<string, string> kvp in columnTypes)
            {
                if (kvp.Value == "numeral")
                {
                    int index = firstFive[0].IndexOf(kvp.Key);
                    for (var i = 1; i < 5; i++)
                    {
                        if (firstFive[i][index] == "" || firstFive[i][index] == null)
                        {
                            continue;
                        }

                        double x;
                        if (!double.TryParse(firstFive[i][index], out x))
                        {
                            if (invalidColumns.ContainsKey(kvp.Key))
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
    }
}