using System;
using System.Globalization;

namespace OCM.TFI.OCM.Utilities
{
    public static class DateHelper
    {
        private const string ServerCulture = "it-IT";

        private const string InternalDateFormat = "yyyy-MM-dd";
        private const string InternalDateTimeFormat = "O";
        
        private const string ReadableDateFormat = "dd/MM/yyyy";
        private const string ReadableDateTimeFormat = "dd/MM/yyyy HH:mm:ss";
        private const string ReadableTimeFormat = "t";

        public static string StandardizeTime(this DateTime input)
            => input.ToString(ReadableTimeFormat, CultureInfo.GetCultureInfo(ServerCulture));
        
        public static DateTime StandardizeDate(this string input, StandardUse source)
            => source == StandardUse.Readable
                ? DateTime.Parse(input, CultureInfo.GetCultureInfo(ServerCulture), DateTimeStyles.None)
                : DateTime.Parse(input, CultureInfo.InvariantCulture, DateTimeStyles.None);

        public static DateTime CurrentCultureDate(this string input)
            => DateTime.Parse(input, CultureInfo.CurrentCulture, DateTimeStyles.None);
        
        public static string StandardizeDateString(this DateTime input, StandardUse destination) 
            => input.ToString(destination == StandardUse.Readable ? ReadableDateFormat : InternalDateFormat);

        public static string StandardizeDateString(this string input, StandardUse source, StandardUse destination)
            => string.IsNullOrWhiteSpace(input) ? null : input.StandardizeDate(source).StandardizeDateString(destination);

        public static DateTime StandardizeDateTime(this string input, StandardUse source)
            => source == StandardUse.Readable
                ? DateTime.Parse(input, CultureInfo.GetCultureInfo(ServerCulture), DateTimeStyles.None)
                : DateTime.Parse(input, CultureInfo.InvariantCulture, DateTimeStyles.None);

        public static string StandardizeDateTimeString(this DateTime input, StandardUse destination)
            => input.ToString(destination == StandardUse.Readable ? ReadableDateTimeFormat : InternalDateTimeFormat);

        public static string StandardizeDateTimeString(this string input, StandardUse source, StandardUse destination)
            => string.IsNullOrWhiteSpace(input) ? null : input.StandardizeDate(source).StandardizeDateString(destination);
        
    }

    public enum StandardUse
    {
        Readable,
        Internal
    }

}