using HomeTaste.Application.Interfaces.TimeManagement;
using System.Globalization;

namespace HomeTaste.Application.Services.TimeManagement
{
    public class DateTimeService : IDateTimeService
    {
        // Time Zone & Global Date/Time Methods

        // Converts a DateTime to a specific time zone
        public DateTime ConvertToTimeZone(DateTime dateTime, string timeZoneId)
        {
            // Find the TimeZoneInfo object for the specified time zone
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

            // Convert the DateTime to the specified time zone
            return TimeZoneInfo.ConvertTime(dateTime, timeZone);
        }

        public DateTime ConvertToTimeZone(DateTime dateTime, TimeZoneInfo timeZone)
        {
            return TimeZoneInfo.ConvertTime(dateTime, timeZone);
        }

        // Converts a DateTime to UTC
        public DateTime ConvertToUtc(DateTime dateTime, string timeZoneId)
        {
            // Find the TimeZoneInfo object for the specified time zone
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

            // Convert the DateTime to UTC
            return TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZone);
        }

        // Converts a UTC DateTime to a specific time zone
        public DateTime ConvertFromUtc(DateTime utcDateTime, string timeZoneId)
        {
            // Find the TimeZoneInfo object for the specified time zone
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

            // Convert the UTC DateTime to the specified time zone
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);
        }

        // Gets the time offset for a specific time zone
        public TimeSpan GetTimeOffset(string timeZoneId)
        {
            // Find the TimeZoneInfo object for the specified time zone
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

            // Return the UTC offset for the current time in the specified time zone
            return timeZone.GetUtcOffset(DateTime.UtcNow);
        }

        // Gets the current time zone name (e.g., "Pacific Standard Time")
        public string GetCurrentTimeZoneName()
        {
            // Return the standard time zone name for the local machine
            return TimeZoneInfo.Local.StandardName;
        }

        // Gets the current DateTime with the offset for a specific time zone
        public DateTimeOffset NowWithOffset(string timeZoneId)
        {
            // Find the TimeZoneInfo object for the specified time zone
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

            // Convert the current UTC time to the specified time zone
            var dateTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZone);

            // Return the DateTimeOffset with the time zone offset
            return new DateTimeOffset(dateTime, timeZone.GetUtcOffset(dateTime));
        }

        // Additional Time Zone & Global Date/Time Methods

        public DateTime ConvertToLocalTime(DateTime utcDateTime)
        {
            return utcDateTime.ToLocalTime();
        }

        public DateTime ConvertFromLocalTime(DateTime localDateTime)
        {
            return localDateTime.ToUniversalTime();
        }

        public string GetTimeZoneAbbreviation(string timeZoneId)
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return timeZone.IsDaylightSavingTime(DateTime.UtcNow) ? timeZone.DaylightName : timeZone.StandardName;
        }

        public bool IsDaylightSavingTime(DateTime dateTime, string timeZoneId)
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return timeZone.IsDaylightSavingTime(dateTime);
        }        

        public DateTime ConvertFromTimeZone(DateTime dateTime, TimeZoneInfo timeZone)
        {
            return TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZone);
        }

        public DateTime GetUtcNow()
        {
            return DateTime.UtcNow;
        }

        public DateTime GetLocalNow()
        {
            return DateTime.Now;
        }

        public DateTime GetTimeZoneNow(string timeZoneId)
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTime(DateTime.Now, timeZone);
        }

        public string GetTimeZoneIdByLocation(string location)
        {
            // Example of location-based time zone. A more complex logic can be added here to map locations to time zones.
            if (location.Equals("New York", StringComparison.OrdinalIgnoreCase))
            {
                return "Eastern Standard Time";
            }
            return "Unknown";
        }

















        // Date Arithmetic & Comparisons
        public DateTime AddDaysToDate(DateTime date, int days)
        {
            return date.AddDays(days);
        }

        public DateTime SubtractDaysFromDate(DateTime date, int days)
        {
            return date.AddDays(-days);
        }

        public int GetDifferenceInDays(DateTime startDate, DateTime endDate)
        {
            return (endDate - startDate).Days;
        }

        public int GetDifferenceInMonths(DateTime startDate, DateTime endDate)
        {
            return ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month;
        }

        public int GetDifferenceInYears(DateTime startDate, DateTime endDate)
        {
            return endDate.Year - startDate.Year;
        }

        public bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        public bool IsWeekday(DateTime date)
        {
            return !IsWeekend(date);
        }

        public DateTime NextWeekday(DateTime date, DayOfWeek day)
        {
            // Move to the next weekday of the specified day
            int daysToAdd = ((int)day - (int)date.DayOfWeek + 7) % 7;
            return date.AddDays(daysToAdd == 0 ? 7 : daysToAdd); // If the day is today, skip to the next one
        }

        public DateTime PreviousWeekday(DateTime date, DayOfWeek day)
        {
            // Move to the previous weekday of the specified day
            int daysToSubtract = ((int)date.DayOfWeek - (int)day + 7) % 7;
            return date.AddDays(-daysToSubtract == 0 ? -7 : -daysToSubtract); // If the day is today, go to the previous one
        }

        // Additional Date Arithmetic & Comparisons

        // Add a number of weeks to a date
        public DateTime AddWeeksToDate(DateTime date, int weeks)
        {
            return date.AddDays(weeks * 7);
        }

        // Subtract a number of weeks from a date
        public DateTime SubtractWeeksFromDate(DateTime date, int weeks)
        {
            return date.AddDays(-weeks * 7);
        }

        // Get the first day of the given month
        public DateTime StartOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        // Get the last day of the given month
        public DateTime EndOfMonth(DateTime date)
        {
            int lastDay = DateTime.DaysInMonth(date.Year, date.Month);
            return new DateTime(date.Year, date.Month, lastDay);
        }

        // Get the same day in the next month
        public DateTime NextMonth(DateTime date)
        {
            return date.AddMonths(1);
        }

        // Get the same day in the previous month
        public DateTime PreviousMonth(DateTime date)
        {
            return date.AddMonths(-1);
        }

        // Add a TimeSpan to a DateTime
        public DateTime AddTimeSpan(DateTime date, TimeSpan timeSpan)
        {
            return date.Add(timeSpan);
        }

        // Subtract a TimeSpan from a DateTime
        public DateTime SubtractTimeSpan(DateTime date, TimeSpan timeSpan)
        {
            return date.Subtract(timeSpan);
        }

        // Check if two DateTime objects represent the same day
        public bool IsSameDay(DateTime date1, DateTime date2)
        {
            return date1.Date == date2.Date;
        }



        // Additional Date Arithmetic & Comparisons

        public DateTime AddMonthsToDate(DateTime date, int months)
        {
            return date.AddMonths(months);
        }

        public DateTime SubtractMonthsFromDate(DateTime date, int months)
        {
            return date.AddMonths(-months);
        }

        public DateTime AddYearsToDate(DateTime date, int years)
        {
            return date.AddYears(years);
        }

        public DateTime SubtractYearsFromDate(DateTime date, int years)
        {
            return date.AddYears(-years);
        }


        public bool IsDateBefore(DateTime date1, DateTime date2)
        {
            return date1 < date2;
        }

        public bool IsDateAfter(DateTime date1, DateTime date2)
        {
            return date1 > date2;
        }














        // Start/End of Period Calculations

        // First day of the current year
        public DateTime FirstDayOfYear()
        {
            var currentYear = DateTime.Now.Year;
            return new DateTime(currentYear, 1, 1);
        }

        // First day of a specific year
        public DateTime FirstDayOfYear(int year)
        {
            return new DateTime(year, 1, 1);
        }

        // Last day of the current year
        public DateTime LastDayOfYear()
        {
            var currentYear = DateTime.Now.Year;
            return new DateTime(currentYear, 12, 31);
        }

        // Last day of a specific year
        public DateTime LastDayOfYear(int year)
        {
            return new DateTime(year, 12, 31);
        }

        // First day of the week for a given date (assumes Sunday as the start of the week)
        public DateTime FirstDayOfWeek(DateTime date)
        {
            int diff = date.DayOfWeek - DayOfWeek.Sunday;
            if (diff < 0) diff += 7; // Adjust if the current day is before Sunday
            return date.AddDays(-diff).Date; // Set time to 00:00
        }

        // Last day of the week for a given date (assumes Saturday as the end of the week)
        public DateTime LastDayOfWeek(DateTime date)
        {
            int diff = DayOfWeek.Saturday - date.DayOfWeek;
            if (diff < 0) diff += 7; // Adjust if the current day is after Saturday
            return date.AddDays(diff).Date; // Set time to 00:00
        }

        // First day of the quarter for a given date
        public DateTime FirstDayOfQuarter(DateTime date)
        {
            int quarter = (date.Month - 1) / 3 + 1; // Determine the quarter based on the month
            int firstMonthOfQuarter = (quarter - 1) * 3 + 1; // Get the first month of the quarter
            return new DateTime(date.Year, firstMonthOfQuarter, 1);
        }

        // Last day of the quarter for a given date
        public DateTime LastDayOfQuarter(DateTime date)
        {
            int quarter = (date.Month - 1) / 3 + 1; // Determine the quarter based on the month
            int lastMonthOfQuarter = quarter * 3; // Get the last month of the quarter
            int lastDay = DateTime.DaysInMonth(date.Year, lastMonthOfQuarter); // Get the last day of the month
            return new DateTime(date.Year, lastMonthOfQuarter, lastDay);
        }


        // Additional Start/End of Period Calculations

        public DateTime FirstDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public DateTime LastDayOfMonth(DateTime date)
        {
            int lastDay = DateTime.DaysInMonth(date.Year, date.Month);
            return new DateTime(date.Year, date.Month, lastDay);
        }

        public DateTime FirstDayOfFinancialYear(int year)
        {
            return new DateTime(year, 4, 1);  // Assuming financial year starts in April
        }

        public DateTime LastDayOfFinancialYear(int year)
        {
            return new DateTime(year + 1, 3, 31);  // Financial year ends on March 31st of the following year
        }

        public int BusinessDaysInMonth(DateTime date)
        {
            int startDay = 1;
            int endDay = DateTime.DaysInMonth(date.Year, date.Month);
            int businessDays = 0;

            for (int i = startDay; i <= endDay; i++)
            {
                var currentDate = new DateTime(date.Year, date.Month, i);
                if (currentDate.DayOfWeek != DayOfWeek.Saturday && currentDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    businessDays++;
                }
            }
            return businessDays;
        }

        public int BusinessDaysInYear(int year)
        {
            int businessDays = 0;

            for (int month = 1; month <= 12; month++)
            {
                businessDays += BusinessDaysInMonth(new DateTime(year, month, 1));
            }

            return businessDays;
        }

        public int WeeksInYear(int year)
        {
            var firstDayOfYear = new DateTime(year, 1, 1);
            var lastDayOfYear = new DateTime(year, 12, 31);
            var fullWeeks = (lastDayOfYear - firstDayOfYear).Days / 7;
            return fullWeeks + 1;
        }



















        // Parsing & Formatting

        // Convert a DateTime to a BD (Bangladesh) format string: "dd/MM/yyyy"
        public string ConvertDateToBDString(DateTime date)
        {
            return date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        // Convert a DateTime to a US format string: "MM/dd/yyyy"
        public string ConvertDateToUSString(DateTime date)
        {
            return date.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
        }

        // Convert a DateTime to a UK format string: "dd/MM/yyyy"
        public string ConvertDateToUKString(DateTime date)
        {
            return date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        // Convert a DateTime to a string based on the given format
        public string ConvertDateTimeToString(DateTime dateTime, string format)
        {
            return dateTime.ToString(format, CultureInfo.InvariantCulture);
        }

        // Parse a date string to a DateTime using the exact format
        public DateTime ParseExactDateString(string dateString, string format)
        {
            return DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture);
        }

        // Try parsing a date string to a DateTime, return true if successful, otherwise false
        public bool TryParseDateString(string dateString, out DateTime result)
        {
            return DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        }

        public string ConvertToShortDateString(DateTime date)
        {
            return date.ToString("d", CultureInfo.InvariantCulture);
        }

        public string ConvertToLongDateString(DateTime date)
        {
            return date.ToString("D", CultureInfo.InvariantCulture);
        }

        public string ConvertToTimeString(DateTime dateTime)
        {
            return dateTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
        }

        public DateTime ParseTimeString(string timeString)
        {
            return DateTime.ParseExact(timeString, "HH:mm:ss", CultureInfo.InvariantCulture);
        }

        public DateTime ParseDateStringWithCulture(string dateString, string cultureCode)
        {
            var cultureInfo = new CultureInfo(cultureCode);
            return DateTime.Parse(dateString, cultureInfo);
        }

        public string ToRfc1123String(DateTime dateTime)
        {
            return dateTime.ToString("R", CultureInfo.InvariantCulture);
        }

        public DateTime ParseRfc1123String(string rfc1123String)
        {
            return DateTime.ParseExact(rfc1123String, "R", CultureInfo.InvariantCulture);
        }

        public string ToXmlString(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
        }

        public DateTime ParseXmlString(string xmlString)
        {
            return DateTime.ParseExact(xmlString, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
        }

        












        // Validation & Special Cases

        // Check if a year is a leap year
        public bool IsLeapYear(int year)
        {
            return DateTime.IsLeapYear(year);
        }

        // Check if the given date string is a valid date
        public bool IsValidDateString(string dateString)
        {
            return DateTime.TryParse(dateString, out _);
        }

        // Check if a date is "unlimited", meaning it is set to DateTime.MaxValue
        public bool IsUnlimitedDate(DateTime date)
        {
            return date == DateTime.MaxValue;
        }

        // Check if the given date is in the past
        public bool IsDateInPast(DateTime date)
        {
            return date < DateTime.Now;
        }

        // Check if the given date is in the future
        public bool IsDateInFuture(DateTime date)
        {
            return date > DateTime.Now;
        }

        // Check if two dates are the same (same date, regardless of time)
        public bool IsSameDate(DateTime date1, DateTime date2)
        {
            return date1.Date == date2.Date;
        }


        public bool IsDateInRange(DateTime date, DateTime startDate, DateTime endDate)
        {
            return date >= startDate && date <= endDate;
        }

        public bool IsBusinessDay(DateTime date)
        {
            return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
        }

        public bool IsSameWeek(DateTime date1, DateTime date2)
        {
            var calendar = System.Globalization.CultureInfo.CurrentCulture.Calendar;
            var week1 = calendar.GetWeekOfYear(date1, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            var week2 = calendar.GetWeekOfYear(date2, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return week1 == week2 && date1.Year == date2.Year;
        }

        public bool IsSameMonth(DateTime date1, DateTime date2)
        {
            return date1.Month == date2.Month && date1.Year == date2.Year;
        }

        public bool IsSameYear(DateTime date1, DateTime date2)
        {
            return date1.Year == date2.Year;
        }

        public bool IsDateWithinBusinessHours(DateTime date)
        {
            return date.Hour >= 9 && date.Hour < 17;
        }










        // Serialization / SQL Compatibility

        // Converts a DateTime to a SQL-compatible date string (yyyy-MM-dd)
        public string ToSqlDateString(DateTime date)
        {
            return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        // Converts a DateTime to a SQL-compatible datetime string (yyyy-MM-dd HH:mm:ss)
        public string ToSqlDateTimeString(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }

        // Converts a DateTime to an ISO 8601-compatible string (yyyy-MM-ddTHH:mm:ssZ)
        public string ToIso8601String(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        }

        // Parses an ISO 8601 string (yyyy-MM-ddTHH:mm:ssZ) into a DateTime
        public DateTime ParseIso8601String(string isoDateTimeString)
        {
            return DateTime.ParseExact(isoDateTimeString, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
        }

        // Converts a DateTime to a Unix Timestamp (seconds since epoch)
        public long ToUnixTimestamp(DateTime dateTime)
        {
            DateTimeOffset dto = new DateTimeOffset(dateTime);
            return dto.ToUnixTimeSeconds();
        }

        // Converts a Unix Timestamp (seconds since epoch) to a DateTime
        public DateTime FromUnixTimestamp(long unixTimestamp)
        {
            DateTimeOffset dto = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp);
            return dto.DateTime;
        }

        // Converts a DateTime to a round-trip (full) string representation for serialization (ISO 8601 format)
        public string ToRoundTripString(DateTime dateTime)
        {
            return dateTime.ToString("o", CultureInfo.InvariantCulture); // The "o" format specifier is for Round-trip DateTime format
        }

        // Parses a round-trip string representation back to DateTime
        public DateTime ParseRoundTripString(string roundTripDateTimeString)
        {
            return DateTime.ParseExact(roundTripDateTimeString, "o", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
        }

        // Converts a DateTime to a SQL-compatible time string (HH:mm:ss)
        public string ToSqlTimeString(DateTime dateTime)
        {
            return dateTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
        }

        // Converts a DateTime to a SQL-compatible datetime string with time zone offset (yyyy-MM-dd HH:mm:ss zzz)
        public string ToSqlDateTimeWithOffsetString(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss zzz", CultureInfo.InvariantCulture);
        }

        // Parses a datetime with time zone offset (yyyy-MM-dd HH:mm:ss zzz) into DateTime
        public DateTime ParseSqlDateTimeWithOffsetString(string dateTimeWithOffsetString)
        {
            return DateTime.ParseExact(dateTimeWithOffsetString, "yyyy-MM-dd HH:mm:ss zzz", CultureInfo.InvariantCulture);
        }



    }
}
