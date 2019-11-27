using CorrelationStation.Models;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CorrelationStation.Services
{
    public class ConfigurationService
    {
        private static readonly List<string> _types = new List<string> { "categorical", "numeral", "exclude", "date time" };

        public void MakeDropDownAndFirstFive(SelectTypeVM selectTypeVM)
        {
            selectTypeVM.Types = new SelectList(_types);

            List<List<string>> firstFive = new List<List<string>>();

            using (TextReader fileReader = File.OpenText(selectTypeVM.Path))
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

            selectTypeVM.FirstFiveRows = firstFive;
        }

        public SelectTypeVM MakeSelectType(UploadVM vm, string path)
        {
            var fileName = Path.GetFileName(vm.File.FileName);

            vm.File.SaveAs(path);

            SelectTypeVM selectTypeVM = new SelectTypeVM();
            selectTypeVM.Path = path;
            selectTypeVM.ColumnTypes = new Dictionary<string, string>();

            string line1 = File.ReadLines(selectTypeVM.Path).First();

            string[] values = line1.Split(',');

            for (var i = 0; i < values.Length; i++)
            {
                selectTypeVM.ColumnTypes.Add(values[i].Replace("\"", ""), "");
            }

            MakeDropDownAndFirstFive(selectTypeVM);

            return selectTypeVM;
        }
    }
}