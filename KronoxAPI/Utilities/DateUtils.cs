using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;


namespace KronoxAPI.Utilities
{
    /// <summary>
    /// Several utility methods relating to DateTimes and calendar mathematics.
    /// </summary>
    public class DateUtils
    {
        /// <summary>
        /// Calculate the culture relative week number from a given DateTime <paramref name="date"/>.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public int GetIso8601WeekOfYear(DateTime date)
        {
            // Method code found at https://stackoverflow.com/a/72561916/18690430

            // Get culture of current thread to help determine how weeks are defined
            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            DayOfWeek firstDay = cultureInfo.DateTimeFormat.FirstDayOfWeek;
            CalendarWeekRule weekRule = cultureInfo.DateTimeFormat.CalendarWeekRule;
            Calendar cal = cultureInfo.Calendar;

            // Use the culture week definition to determine current week
            return cal.GetWeekOfYear(date, weekRule, firstDay);
        }
    }
}
