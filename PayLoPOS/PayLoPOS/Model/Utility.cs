using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayLoPOS.Model
{
    class Utility
    {

        public static string displayDateFormat = "dd MMM, yyyy (hh:mm tt)";

        public static Boolean isNotNull(Dictionary<string, object> param, string key)
        {
            if(param[key] != null)
            {
                return true;
            }
            return false;
        }

        public static string getCurrentDate(string format)
        {
            DateTime time = DateTime.Now;
            return time.ToString(format);
        }

        public static string getDate(string dateString, string fromFormat, string toFormat)
        {
            DateTime dt = DateTime.ParseExact(dateString, fromFormat, CultureInfo.InvariantCulture);
            return dt.ToString(toFormat);
        }

        public static DateTime getPreviousDateDate(double previousDay)
        {
            DateTime dateTime = DateTime.Now;
            return dateTime.AddDays(previousDay);
        }
    }
}
