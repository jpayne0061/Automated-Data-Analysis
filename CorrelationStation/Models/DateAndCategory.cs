using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorrelationStation.Models
{
    public class DateAndCategory
    {
        public int Id { get; set; }
        public ICollection<LinePlotCategory> LinePlotCategories { get; set; }

        public string Variable1 { get; set; }
        public string Variable2 { get; set; }

        public DateAndCategory()
        {
            LinePlotCategories = new List<LinePlotCategory>();
        }
        public void GetLinePlotData(List<string> times, List<string> categories)
        {


            Dictionary<string, Dictionary<string, DateAndCount>> categoryAndDateCounts = new Dictionary<string, Dictionary<string, DateAndCount>>();
            
            for(var i = 0; i < categories.Count; i++)
            {
                if((categories[i] == null || categories[i] == "") || (times[i] == null || times[i] == ""))
                {
                    continue;
                }
                bool needsMonth = false;
                string formattedDate = "";
                DateTime dt;

                if(DateTime.TryParse(times[i], out dt))
                {
                    formattedDate = dt.ToString("y");
                }
                else
                {
                    formattedDate = DateTime.Parse(times[i] + "/1").ToString("yyyy");
                    needsMonth = true;
                }


                if (categoryAndDateCounts.ContainsKey(categories[i]))
                {
                    if(categoryAndDateCounts[categories[i]].ContainsKey(formattedDate))
                    {
                        categoryAndDateCounts[categories[i]][formattedDate].Count += 1;
                    }
                    else
                    {
                        if(needsMonth)
                        {
                            categoryAndDateCounts[categories[i]].Add(formattedDate, new DateAndCount
                            {
                                CategoryName = categories[i],
                                Count = 1,
                                MonthAndYear = formattedDate,
                                DateTime = DateTime.Parse(formattedDate + "/1")
                            });
                        }
                        else
                        {
                            categoryAndDateCounts[categories[i]].Add(formattedDate, new DateAndCount
                            {
                                CategoryName = categories[i],
                                Count = 1,
                                MonthAndYear = formattedDate,
                                DateTime = DateTime.Parse(formattedDate)
                            });
                        }

                    }

                }
                else
                {
                    if(needsMonth)
                    {
                        categoryAndDateCounts[categories[i]] = new Dictionary<string, DateAndCount>();
                        categoryAndDateCounts[categories[i]].Add(formattedDate, new DateAndCount
                        {
                            CategoryName = categories[i],
                            Count = 1,
                            MonthAndYear = formattedDate,
                            DateTime = DateTime.Parse(formattedDate + "/1")

                        });
                    }
                    else
                    {
                        categoryAndDateCounts[categories[i]] = new Dictionary<string, DateAndCount>();
                        categoryAndDateCounts[categories[i]].Add(formattedDate, new DateAndCount
                        {
                            CategoryName = categories[i],
                            Count = 1,
                            MonthAndYear = formattedDate,
                            DateTime = DateTime.Parse(formattedDate)

                        });
                    }

                }

            }

            foreach(KeyValuePair<string, Dictionary<string, DateAndCount>> kvp in categoryAndDateCounts)
            {
                LinePlotCategory linePlotCategory = new LinePlotCategory();
                linePlotCategory.Name = kvp.Key;
                linePlotCategory.DateAndCounts = new List<DateAndCount>();
                foreach(KeyValuePair<string, DateAndCount> nestedKvp in kvp.Value)
                {
                    linePlotCategory.DateAndCounts.Add(nestedKvp.Value);
                }

                LinePlotCategories.Add(linePlotCategory);
            }

        }

    }
}