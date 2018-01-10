using System;
using System.Globalization;

namespace TaxisBeat.Code
{
    // Stolen from Dependencies.sln !
    public static class DateTimeExtensions
    {
        private const long _jsEpochTicks = 621355968000000000;

        /// <summary>
        /// Round the date time to the next timespan
        /// </summary>
        /// <remarks>datetime rounding extensions found on http://stackoverflow.com/questions/1393696/c-sharp-rounding-datetime-objects</remarks>
        public static DateTime Round(this DateTime date, TimeSpan span)
        {
            long ticks = (date.Ticks + (span.Ticks / 2) + 1) / span.Ticks;
            return new DateTime(ticks * span.Ticks, date.Kind);
        }

        /// <summary>
        /// Get the rounded down datetime closest to the given timespan
        /// </summary>
        public static DateTime Floor(this DateTime date, TimeSpan span)
        {
            long ticks = (date.Ticks / span.Ticks);
            return new DateTime(ticks * span.Ticks, date.Kind);
        }

        /// <summary>
        /// Get the rounded up datetime closest to the given timespan
        /// </summary>
        public static DateTime Ceil(this DateTime date, TimeSpan span)
        {
            long ticks = (date.Ticks + span.Ticks - 1) / span.Ticks;
            return new DateTime(ticks * span.Ticks, date.Kind);
        }


        /// <summary>
        /// Convert javascript getTime() millisecs to <see cref="DateTime"/> (UTC)
        /// </summary>
        public static DateTime JsMillisToDateTime(this long jsMillisecs)
        {
            return new DateTime(_jsEpochTicks + jsMillisecs * TimeSpan.TicksPerMillisecond, DateTimeKind.Utc);
        }

        
        /// <summary>
        /// Convert a <see cref="DateTime"/> struct to js millisecs
        /// </summary>
        public static long ToJsMillis(this DateTime date)
        {
            if (!date.IsValid())
            {
                return 0;
            }
            if (date.Kind == DateTimeKind.Local)
            {
                date = date.ToUniversalTime();
            }
            return (long)new TimeSpan(date.Ticks - _jsEpochTicks).TotalMilliseconds;
        }

        /// <summary>
        /// Convert a <see cref="DateTime"/> struct to js millisecs
        /// </summary>
        public static long? ToJsMillis(this DateTime? date)
        {
            return date.HasValue
                ? (long?)date.Value.ToJsMillis()
                : null;
        }

        /// <summary>
        /// Returns true if value other than <see cref="DateTime.MinValue"/> and <see cref="DateTime.MaxValue"/>
        /// </summary>
        public static bool IsValid(this DateTime date)
        {
            return date != DateTime.MinValue && date != DateTime.MaxValue;
        }

        /// <summary>
        /// Returns true if date between given dates. Return false if any date is null
        /// </summary>
        public static bool Between(this DateTime now, DateTime? from, DateTime? to)
        {
            if (from == null || to == null)
            {
                return false;
            }
            return now > from.Value && now < to.Value;
        }

        /// <summary>
        /// DateTime to return start of the week of DateTime.UtcNow
        /// </summary>
        /// <remarks>DateTime for the start of the week extension found on http://stackoverflow.com/questions/38039/how-can-i-get-the-datetime-for-the-start-of-the-week</remarks>
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }



        /// <summary>
        /// Returns start and end time of a period according to given datetime and offset
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static DateTime[] ToCurrentPeriod(this DateTime dt, int offset)
        {
            var to = dt;
            var now = DateTime.UtcNow;
            to = to > now ? now : to;
            while (to <= now)
            {
                to = to.AddDays(offset);
            }
            var from = to.AddDays(-offset);
            return new[] { from, to };
        }


        // This presumes that weeks start with Monday.
        // Week 1 is the 1st week of the year with a Thursday in it.
        public static int GetIso8601WeekOfYear(this DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }

}