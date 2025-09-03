using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Helper.Logic
{
    public static class DateHelper
    {
        public static bool IsBetweenTwoDates(this DateTime date, DateTime startDate, DateTime endDate)
        {
            return date >= startDate && date <= endDate;
        }

        public static DateTime GetDateTimeNow()
        {
            return DateTime.Now.ToUniversalTime();
        }

        /// <summary>
        /// Combines the date portion of <paramref name="date"/> with the time portion of <paramref name="dateTime"/>.
        /// </summary>
        public static DateTime Combine(DateTime date, DateTime dateTime)
        {
	        return date.Date.Add(dateTime.TimeOfDay);
        }
	}
}
