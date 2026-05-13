namespace HomeTaste.Application.Interfaces.TimeManagement
{
    public interface IDateTimeService
    {

        //Time Zone & Global Date/Time Methods

        // Convert a DateTime to a specific time zone
        DateTime ConvertToTimeZone(DateTime dateTime, string timeZoneId);
        // Example: ConvertToTimeZone(DateTime.Now, "Eastern Standard Time") -> 2023-08-23 10:00:00 (for example)

        // Convert DateTime to a specific TimeZoneInfo
        DateTime ConvertToTimeZone(DateTime dateTime, TimeZoneInfo timeZone);
        // Example: ConvertToTimeZone(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time")) -> 2023-08-23 07:00:00

        // Convert a DateTime from a time zone to UTC
        DateTime ConvertToUtc(DateTime dateTime, string timeZoneId);
        // Example: ConvertToUtc(DateTime.Now, "Pacific Standard Time") -> 2023-08-23 14:00:00 (UTC)

        // Convert a UTC DateTime to a specific time zone
        DateTime ConvertFromUtc(DateTime utcDateTime, string timeZoneId);
        // Example: ConvertFromUtc(DateTime.UtcNow, "Central European Time") -> 2023-08-23 16:00:00

        // Get the time offset for a specific time zone
        TimeSpan GetTimeOffset(string timeZoneId);
        // Example: GetTimeOffset("Eastern Standard Time") -> -05:00:00 (offset for EST)

        // Get the current system time zone name
        string GetCurrentTimeZoneName();
        // Example: GetCurrentTimeZoneName() -> "Pacific Standard Time" (depending on system time zone)

        // Get the current time with offset for a specific time zone
        DateTimeOffset NowWithOffset(string timeZoneId);
        // Example: NowWithOffset("Greenwich Mean Time") -> 2023-08-23 15:00:00 +00:00 (current time in GMT)




        // Additional Time Zone & Global Date/Time Methods
        // Convert UTC DateTime to local time
        DateTime ConvertToLocalTime(DateTime utcDateTime);
        // Example: ConvertToLocalTime(DateTime.UtcNow) -> 2023-08-23 08:00:00 (local time for Pacific Time Zone)

        // Convert local DateTime to UTC
        DateTime ConvertFromLocalTime(DateTime localDateTime);
        // Example: ConvertFromLocalTime(DateTime.Now) -> 2023-08-23 15:00:00 (UTC time)

        // Get the abbreviation of a specific time zone (e.g., EST, PST)
        string GetTimeZoneAbbreviation(string timeZoneId);
        // Example: GetTimeZoneAbbreviation("Eastern Standard Time") -> "EST"

        // Check if the given date/time is in daylight saving time for a specific time zone
        bool IsDaylightSavingTime(DateTime dateTime, string timeZoneId);
        // Example: IsDaylightSavingTime(DateTime.Now, "Eastern Standard Time") -> true (during daylight saving time)

        // Convert DateTime from a specific TimeZoneInfo to UTC
        DateTime ConvertFromTimeZone(DateTime dateTime, TimeZoneInfo timeZone);
        // Example: ConvertFromTimeZone(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time")) -> 2023-08-23 15:00:00 (UTC)

        // Get the current UTC time
        DateTime GetUtcNow();
        // Example: GetUtcNow() -> 2023-08-23 12:00:00 (UTC time)

        // Get the current local time
        DateTime GetLocalNow();
        // Example: GetLocalNow() -> 2023-08-23 05:00:00 (local time for the machine's time zone)

        // Get the current time in a specific time zone
        DateTime GetTimeZoneNow(string timeZoneId);
        // Example: GetTimeZoneNow("Greenwich Mean Time") -> 2023-08-23 12:00:00 (current time in GMT)

        // Get the time zone ID for a specific location/country
        string GetTimeZoneIdByLocation(string location);
        // Example: GetTimeZoneIdByLocation("New York") -> "Eastern Standard Time"





















        //Date Arithmetic & Comparisons


        // Add days to a date
        DateTime AddDaysToDate(DateTime date, int days);
        // Example: AddDaysToDate(DateTime.Now, 5) -> 2023-08-28 (adds 5 days to today's date)

        // Subtract days from a date
        DateTime SubtractDaysFromDate(DateTime date, int days);
        // Example: SubtractDaysFromDate(DateTime.Now, 5) -> 2023-08-18 (subtracts 5 days from today's date)

        // Get the difference in days between two dates
        int GetDifferenceInDays(DateTime startDate, DateTime endDate);
        // Example: GetDifferenceInDays(new DateTime(2023, 8, 18), new DateTime(2023, 8, 23)) -> 5 (difference in days)

        // Get the difference in months between two dates
        int GetDifferenceInMonths(DateTime startDate, DateTime endDate);
        // Example: GetDifferenceInMonths(new DateTime(2023, 1, 15), new DateTime(2023, 8, 23)) -> 7 (difference in months)

        // Get the difference in years between two dates
        int GetDifferenceInYears(DateTime startDate, DateTime endDate);
        // Example: GetDifferenceInYears(new DateTime(2020, 8, 23), new DateTime(2023, 8, 23)) -> 3 (difference in years)

        // Check if the date is a weekend
        bool IsWeekend(DateTime date);
        // Example: IsWeekend(new DateTime(2023, 8, 19)) -> true (since 2023-08-19 is a Saturday)

        // Check if the date is a weekday
        bool IsWeekday(DateTime date);
        // Example: IsWeekday(new DateTime(2023, 8, 23)) -> true (since 2023-08-23 is a Wednesday)

        // Get the next specified weekday
        DateTime NextWeekday(DateTime date, DayOfWeek day);
        // Example: NextWeekday(new DateTime(2023, 8, 23), DayOfWeek.Monday) -> 2023-08-28 (next Monday)

        // Get the previous specified weekday
        DateTime PreviousWeekday(DateTime date, DayOfWeek day);
        // Example: PreviousWeekday(new DateTime(2023, 8, 23), DayOfWeek.Monday) -> 2023-08-21 (previous Monday)


        // Additional Date Arithmetic & Comparisons
        // Add a number of weeks to a date
        DateTime AddWeeksToDate(DateTime date, int weeks);
        // Example: AddWeeksToDate(DateTime.Now, 2) -> 2023-09-06 (adds 2 weeks to today's date)

        // Subtract a number of weeks from a date
        DateTime SubtractWeeksFromDate(DateTime date, int weeks);
        // Example: SubtractWeeksFromDate(DateTime.Now, 2) -> 2023-08-09 (subtracts 2 weeks from today's date)

        // Get the first day of the given month
        DateTime StartOfMonth(DateTime date);
        // Example: StartOfMonth(new DateTime(2023, 8, 23)) -> 2023-08-01 (first day of August)

        // Get the last day of the given month
        DateTime EndOfMonth(DateTime date);
        // Example: EndOfMonth(new DateTime(2023, 8, 23)) -> 2023-08-31 (last day of August)

        // Get the same day in the next month
        DateTime NextMonth(DateTime date);
        // Example: NextMonth(new DateTime(2023, 8, 23)) -> 2023-09-23 (same day in the next month)

        // Get the same day in the previous month
        DateTime PreviousMonth(DateTime date);
        // Example: PreviousMonth(new DateTime(2023, 8, 23)) -> 2023-07-23 (same day in the previous month)

        // Add a TimeSpan to a DateTime
        DateTime AddTimeSpan(DateTime date, TimeSpan timeSpan);
        // Example: AddTimeSpan(DateTime.Now, TimeSpan.FromDays(5)) -> 2023-08-28 (adds 5 days)

        // Subtract a TimeSpan from a DateTime
        DateTime SubtractTimeSpan(DateTime date, TimeSpan timeSpan);
        // Example: SubtractTimeSpan(DateTime.Now, TimeSpan.FromDays(5)) -> 2023-08-13 (subtracts 5 days)

        // Check if two DateTime objects represent the same day
        bool IsSameDay(DateTime date1, DateTime date2);
        // Example: IsSameDay(new DateTime(2023, 8, 23), new DateTime(2023, 8, 23)) -> true (same day)
        // Example: IsSameDay(new DateTime(2023, 8, 23), new DateTime(2023, 8, 24)) -> false (different day)

        // Additional Date Arithmetic & Comparisons

        // Add a certain number of months to a date
        DateTime AddMonthsToDate(DateTime date, int months);
        // Example: AddMonthsToDate(new DateTime(2023, 8, 23), 3) -> 2023-11-23 (adds 3 months)

        // Subtract a certain number of months from a date
        DateTime SubtractMonthsFromDate(DateTime date, int months);
        // Example: SubtractMonthsFromDate(new DateTime(2023, 8, 23), 3) -> 2023-05-23 (subtracts 3 months)

        // Add a certain number of years to a date
        DateTime AddYearsToDate(DateTime date, int years);
        // Example: AddYearsToDate(new DateTime(2023, 8, 23), 2) -> 2025-08-23 (adds 2 years)

        // Subtract a certain number of years from a date
        DateTime SubtractYearsFromDate(DateTime date, int years);
        // Example: SubtractYearsFromDate(new DateTime(2023, 8, 23), 2) -> 2021-08-23 (subtracts 2 years)

        // Check if a date is before another date
        bool IsDateBefore(DateTime date1, DateTime date2);
        // Example: IsDateBefore(new DateTime(2023, 8, 23), new DateTime(2023, 8, 24)) -> true (2023-08-23 is before 2023-08-24)

        // Check if a date is after another date
        bool IsDateAfter(DateTime date1, DateTime date2);
        // Example: IsDateAfter(new DateTime(2023, 8, 23), new DateTime(2023, 8, 22)) -> true (2023-08-23 is after 2023-08-22)





















        //Start/End of Period Calculations
        // Get the first day of the current year
        DateTime FirstDayOfYear();
        // Example: FirstDayOfYear() -> 2023-01-01 (first day of the current year)

        // Get the first day of a specific year
        DateTime FirstDayOfYear(int year);
        // Example: FirstDayOfYear(2024) -> 2024-01-01 (first day of 2024)

        // Get the last day of the current year
        DateTime LastDayOfYear();
        // Example: LastDayOfYear() -> 2023-12-31 (last day of the current year)

        // Get the last day of a specific year
        DateTime LastDayOfYear(int year);
        // Example: LastDayOfYear(2024) -> 2024-12-31 (last day of 2024)

        // Get the first day of the week for a given date
        DateTime FirstDayOfWeek(DateTime date);
        // Example: FirstDayOfWeek(new DateTime(2023, 8, 23)) -> 2023-08-21 (first day of the week, Monday)

        // Get the last day of the week for a given date
        DateTime LastDayOfWeek(DateTime date);
        // Example: LastDayOfWeek(new DateTime(2023, 8, 23)) -> 2023-08-27 (last day of the week, Sunday)

        // Get the first day of the quarter for a given date
        DateTime FirstDayOfQuarter(DateTime date);
        // Example: FirstDayOfQuarter(new DateTime(2023, 8, 23)) -> 2023-07-01 (first day of the quarter)

        // Get the last day of the quarter for a given date
        DateTime LastDayOfQuarter(DateTime date);
        // Example: LastDayOfQuarter(new DateTime(2023, 8, 23)) -> 2023-09-30 (last day of the quarter)


        // Additional Start/End of Period Calculations

        // Get the first day of the month for a given date
        DateTime FirstDayOfMonth(DateTime date);
        // Example: FirstDayOfMonth(new DateTime(2023, 8, 23)) -> 2023-08-01 (first day of August)

        // Get the last day of the month for a given date
        DateTime LastDayOfMonth(DateTime date);
        // Example: LastDayOfMonth(new DateTime(2023, 8, 23)) -> 2023-08-31 (last day of August)

        // Get the first day of the financial year for a given year (typically April 1st)
        DateTime FirstDayOfFinancialYear(int year);
        // Example: FirstDayOfFinancialYear(2023) -> 2023-04-01 (first day of financial year 2023)

        // Get the last day of the financial year for a given year (typically March 31st)
        DateTime LastDayOfFinancialYear(int year);
        // Example: LastDayOfFinancialYear(2023) -> 2023-03-31 (last day of financial year 2023)

        // Get the number of business days in a specific month
        int BusinessDaysInMonth(DateTime date);
        // Example: BusinessDaysInMonth(new DateTime(2023, 8, 23)) -> 23 (count of business days in August 2023)

        // Get the number of business days in a specific year
        int BusinessDaysInYear(int year);
        // Example: BusinessDaysInYear(2023) -> 252 (count of business days in 2023)

        // Get the number of weeks in a given year
        int WeeksInYear(int year);
        // Example: WeeksInYear(2023) -> 52 (number of weeks in 2023)

























        //Parsing & Formatting
        // Convert DateTime to Bangladesh standard date string format (e.g., "dd-MM-yyyy")
        string ConvertDateToBDString(DateTime date);
        // Example: ConvertDateToBDString(new DateTime(2023, 8, 23)) -> "23-08-2023"

        // Convert DateTime to US standard date string format (e.g., "MM/dd/yyyy")
        string ConvertDateToUSString(DateTime date);
        // Example: ConvertDateToUSString(new DateTime(2023, 8, 23)) -> "08/23/2023"

        // Convert DateTime to UK standard date string format (e.g., "dd/MM/yyyy")
        string ConvertDateToUKString(DateTime date);
        // Example: ConvertDateToUKString(new DateTime(2023, 8, 23)) -> "23/08/2023"

        // Convert DateTime to string with a custom format (e.g., "yyyy-MM-dd HH:mm:ss")
        string ConvertDateTimeToString(DateTime dateTime, string format);
        // Example: ConvertDateTimeToString(new DateTime(2023, 8, 23, 14, 30, 45), "yyyy-MM-dd HH:mm:ss") -> "2023-08-23 14:30:45"

        // Parse a date string to DateTime using exact format matching
        DateTime ParseExactDateString(string dateString, string format);
        // Example: ParseExactDateString("2023-08-23", "yyyy-MM-dd") -> DateTime(2023, 8, 23)

        // Try to parse a date string into DateTime and return a bool indicating success/failure
        bool TryParseDateString(string dateString, out DateTime result);
        // Example: TryParseDateString("2023-08-23", out result) -> result = DateTime(2023, 8, 23), return true


        // Additional Parsing & Formatting

        // Convert a DateTime to a short date format string (MM/dd/yyyy)
        string ConvertToShortDateString(DateTime date);
        // Example: ConvertToShortDateString(new DateTime(2023, 8, 23)) -> "08/23/2023"

        // Convert a DateTime to a long date format string (dddd, MMMM dd, yyyy)
        string ConvertToLongDateString(DateTime date);
        // Example: ConvertToLongDateString(new DateTime(2023, 8, 23)) -> "Wednesday, August 23, 2023"

        // Convert a DateTime to a time-only string (HH:mm:ss)
        string ConvertToTimeString(DateTime dateTime);
        // Example: ConvertToTimeString(new DateTime(2023, 8, 23, 14, 30, 45)) -> "14:30:45"

        // Parse a time string (HH:mm:ss) into a DateTime
        DateTime ParseTimeString(string timeString);
        // Example: ParseTimeString("14:30:45") -> DateTime(2023, 8, 23, 14, 30, 45)

        // Parse a date string with a specific culture (e.g., en-US, en-GB)
        DateTime ParseDateStringWithCulture(string dateString, string cultureCode);
        // Example: ParseDateStringWithCulture("23/08/2023", "en-GB") -> DateTime(2023, 8, 23)

        // Convert a DateTime to an RFC 1123 formatted string (RFC 822 date format)
        string ToRfc1123String(DateTime dateTime);
        // Example: ToRfc1123String(new DateTime(2023, 8, 23, 14, 30, 45)) -> "Wed, 23 Aug 2023 14:30:45 GMT"

        // Parse an RFC 1123 formatted string into a DateTime
        DateTime ParseRfc1123String(string rfc1123String);
        // Example: ParseRfc1123String("Wed, 23 Aug 2023 14:30:45 GMT") -> DateTime(2023, 8, 23, 14, 30, 45)

        // Convert a DateTime to an XML formatted string (ISO 8601 format)
        string ToXmlString(DateTime dateTime);
        // Example: ToXmlString(new DateTime(2023, 8, 23, 14, 30, 45)) -> "2023-08-23T14:30:45"

        // Parse an XML formatted string (ISO 8601) into a DateTime
        DateTime ParseXmlString(string xmlString);
        // Example: ParseXmlString("2023-08-23T14:30:45") -> DateTime(2023, 8, 23, 14, 30, 45)




































        //Validation & Special Cases

        // Check if the year is a leap year
        bool IsLeapYear(int year);
        // Example: IsLeapYear(2020) -> true, IsLeapYear(2021) -> false

        // Check if the string is a valid date
        bool IsValidDateString(string dateString);
        // Example: IsValidDateString("2023-08-23") -> true, IsValidDateString("2023-02-30") -> false

        // Check if the date is 'unlimited' (DateTime.MaxValue)
        bool IsUnlimitedDate(DateTime date);
        // Example: IsUnlimitedDate(DateTime.MaxValue) -> true, IsUnlimitedDate(DateTime.Now) -> false

        // Check if the date is in the past
        bool IsDateInPast(DateTime date);
        // Example: IsDateInPast(new DateTime(2022, 1, 1)) -> true, IsDateInPast(DateTime.Now) -> false

        // Check if the date is in the future
        bool IsDateInFuture(DateTime date);
        // Example: IsDateInFuture(new DateTime(2023, 12, 31)) -> true, IsDateInFuture(DateTime.Now) -> false

        // Check if two dates are the same (ignoring time)
        bool IsSameDate(DateTime date1, DateTime date2);
        // Example: IsSameDate(new DateTime(2023, 8, 23), new DateTime(2023, 8, 23)) -> true
        // Example: IsSameDate(new DateTime(2023, 8, 23, 14, 30, 45), new DateTime(2023, 8, 23)) -> true

        // Additional Validation & Special Cases

        // Check if the date is within a specific range (inclusive)
        bool IsDateInRange(DateTime date, DateTime startDate, DateTime endDate);
        // Example: IsDateInRange(DateTime.Now, new DateTime(2023, 1, 1), new DateTime(2023, 12, 31)) -> true

        // Check if the date is a business day (Monday to Friday)
        bool IsBusinessDay(DateTime date);
        // Example: IsBusinessDay(new DateTime(2023, 8, 23)) -> true, IsBusinessDay(new DateTime(2023, 8, 20)) -> false

        // Check if two dates fall in the same week
        bool IsSameWeek(DateTime date1, DateTime date2);
        // Example: IsSameWeek(new DateTime(2023, 8, 23), new DateTime(2023, 8, 21)) -> true
        // Example: IsSameWeek(new DateTime(2023, 8, 23), new DateTime(2023, 8, 30)) -> false

        // Check if two dates fall in the same month
        bool IsSameMonth(DateTime date1, DateTime date2);
        // Example: IsSameMonth(new DateTime(2023, 8, 23), new DateTime(2023, 8, 1)) -> true
        // Example: IsSameMonth(new DateTime(2023, 8, 23), new DateTime(2023, 9, 1)) -> false

        // Check if two dates fall in the same year
        bool IsSameYear(DateTime date1, DateTime date2);
        // Example: IsSameYear(new DateTime(2023, 8, 23), new DateTime(2023, 1, 1)) -> true
        // Example: IsSameYear(new DateTime(2023, 8, 23), new DateTime(2022, 12, 31)) -> false

        // Check if a date is within typical business hours (9am - 5pm)
        bool IsDateWithinBusinessHours(DateTime date);
        // Example: IsDateWithinBusinessHours(new DateTime(2023, 8, 23, 14, 30, 0)) -> true
        // Example: IsDateWithinBusinessHours(new DateTime(2023, 8, 23, 18, 0, 0)) -> false


























        //Serialization / SQL Compatibility

        // Converts a DateTime to a SQL-compatible date string (yyyy-MM-dd)
        string ToSqlDateString(DateTime date);  
        // Example: "2023-08-23"

        // Converts a DateTime to a SQL-compatible datetime string (yyyy-MM-dd HH:mm:ss)
        string ToSqlDateTimeString(DateTime dateTime);  
        // Example: "2023-08-23 14:30:00"

        // Converts a DateTime to an ISO 8601-compatible string (yyyy-MM-ddTHH:mm:ssZ)
        string ToIso8601String(DateTime dateTime);  
        // Example: "2023-08-23T14:30:00Z"

        // Parses an ISO 8601 string (yyyy-MM-ddTHH:mm:ssZ) into a DateTime
        DateTime ParseIso8601String(string isoDateTimeString);  
        // Example: "2023-08-23T14:30:00Z" -> DateTime object

        // Converts a DateTime to a Unix Timestamp (seconds since epoch)
        long ToUnixTimestamp(DateTime dateTime);  
        // Converts DateTime to Unix timestamp, e.g., "1692885600"

        // Converts a Unix Timestamp (seconds since epoch) to a DateTime
        DateTime FromUnixTimestamp(long unixTimestamp);  
        // Converts Unix timestamp to DateTime, e.g., "2023-08-23 14:30:00"

        // Converts a DateTime to a round-trip (full) string representation for serialization (ISO 8601 format)
        string ToRoundTripString(DateTime dateTime);  
        // Example: "2023-08-23T14:30:00.0000000Z"

        // Parses a round-trip string representation back to DateTime
        DateTime ParseRoundTripString(string roundTripDateTimeString);  
        // Example: "2023-08-23T14:30:00.0000000Z" -> DateTime object

        // Converts a DateTime to a SQL-compatible time string (HH:mm:ss)
        string ToSqlTimeString(DateTime dateTime);  // Example: "14:30:00"

        // Converts a DateTime to a SQL-compatible datetime string with time zone offset (yyyy-MM-dd HH:mm:ss zzz)
        string ToSqlDateTimeWithOffsetString(DateTime dateTime);  
        // Example: "2023-08-23 14:30:00 +00:00"

        // Parses a datetime with time zone offset (yyyy-MM-dd HH:mm:ss zzz) into DateTime
        DateTime ParseSqlDateTimeWithOffsetString(string dateTimeWithOffsetString);  
        // Example: "2023-08-23 14:30:00 +00:00" -> DateTime object

    }
}
