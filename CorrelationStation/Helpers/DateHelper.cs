using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorrelationStation.Helpers
{
    public static class DateHelper
    {
        public static bool CheckIfDateValid(string value)
        {
            DateTime dt;
            if (DateTime.TryParse(value, out dt))
            {
                return true;
            }
            else
            {
                if (DateTime.TryParse(value + "/1", out dt))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}