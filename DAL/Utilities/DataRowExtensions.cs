using System;
using System.Data;
using System.Globalization;
using OCM.TFI.OCM.Utilities;

namespace Utilities
{
    public static class DataRowExtensions
    {
        public static string ElementAt(this DataRow input, string index) => 
            input[index]?.ToString().Trim();

        public static DateTime? RawDateElementAt(this DataRow input, string index, StandardUse source = StandardUse.Internal) =>
            string.IsNullOrWhiteSpace(input.ElementAt(index)) ? null : input.ElementAt(index).StandardizeDate(source);

        public static DateTime? GetRawDateCurrentCultureElementAt(this DataRow input, string index, StandardUse source = StandardUse.Internal) =>
            string.IsNullOrWhiteSpace(input.ElementAt(index)) ? null : input.ElementAt(index).CurrentCultureDate();
        
        public static string DateElementAt(this DataRow input, string index, StandardUse use = StandardUse.Internal)
            => input.RawDateElementAt(index, StandardUse.Readable)?.StandardizeDateString(use);

        public static int? IntElementAt(this DataRow input, string index) => 
            string.IsNullOrWhiteSpace(input.ElementAt(index)) ? null : Convert.ToInt32(input[index]?.ToString().Trim());

        public static double? DoubleElementAt(this DataRow input, string index) => 
            string.IsNullOrWhiteSpace(input.ElementAt(index)) ? null : Convert.ToDouble(input[index]?.ToString().Trim());

        public static decimal? DecimalElementAt(this DataRow input, string index) => 
            string.IsNullOrWhiteSpace(input.ElementAt(index)) ? null : Convert.ToDecimal(input[index]?.ToString().Trim());
    }
}
