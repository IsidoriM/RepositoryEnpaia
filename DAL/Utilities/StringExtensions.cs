using System;
using System.Globalization;

namespace Utilities
{
    public static class StringExtensions
    {
        public static string ToDomainStringFormat(this string input) => input?.Trim();
        public static string ToDomainStringUpperFormat(this string input) => input.ToDomainStringFormat()?.ToUpperInvariant();
        public static string ToDomainMoneyFormat(this string input) => Convert.ToDecimal(input.ToDomainStringFormat() ?? "0").ToString("C", CultureInfo.GetCultureInfo("it-IT"));
        public static string ToDomainDateStringFormat(this string input) => input.ToDomainDateFormat().ToString("yyyy-MM-dd");
        public static DateTime ToDomainDateFormat(this string input) => DateTime.TryParse(input.ToDomainStringFormat(), CultureInfo.GetCultureInfo("it-IT"), DateTimeStyles.AssumeLocal, out var result) ?
            result : Convert.ToDateTime(input, CultureInfo.InvariantCulture);
        public static string ToDomainDecimalFormat(this string input) =>
            Convert.ToDecimal(input.ToDomainStringFormat() ?? "0", CultureInfo.GetCultureInfo("it-IT")).ToString("N", CultureInfo.GetCultureInfo("it-IT"));
        public static Decimal FromDomainCurrencyFormat(this string input) =>
            decimal.TryParse(input, NumberStyles.Currency, CultureInfo.GetCultureInfo("it-IT").NumberFormat, out var result) ? result : decimal.Zero;
    }
}