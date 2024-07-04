﻿using System;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;

namespace Sfa.Tl.ResultsAndCertification.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ToDobFormat(this DateTime value)
        {
            return value.ToString("dd MMMM yyyy");
        }

        public static string ToDobFormat(this DateTime value, bool trimLeadingZero)
        {
            var strDate = value.ToString("dd MMMM yyyy");

            if (!trimLeadingZero)
                return value.ToDobFormat();

            return strDate.TrimStart('0');
        }

        /// <summary>
        /// Converts to dd MMMM yyyy format by default.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="dateFormat">The date format.</param>
        /// <returns></returns>
        public static string ToFormat(this DateTime value, string dateFormat = "dd MMMM yyyy")
        {
            return value.ToString(dateFormat);
        }

        public static string ToCertificateDateFormat(this DateTime value)
        {
            return value.ToString("MMMM yyyy");
        }

        public static string ToUcasFormat(this DateTime value)
        {
            return value.ToString("ddMMyyyy");
        }

        public static string ToPrintBatchDateFormat(this DateTime value)
        {
            return value.ToString("yyyy-MM-dd");
        }

        public static bool IsDateTimeWithFormat(this string value)
        {
            return DateTime.TryParseExact(value, "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }

        public static DateTime ParseStringToDateTime(this string value)
        {
            DateTime.TryParseExact(value, "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result);
            return result;
        }

        public static DateTime ParseStringToDateTimeWithFormat(this string value, string dateFormat = "dd/MM/yyyy")
        {
            DateTime.TryParseExact(value, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result);
            return result;
        }

        public static bool IsInt(this string value)
        {
            return int.TryParse(value, out _);
        }

        public static int ToInt(this string value)
        {
            return int.Parse(value);
        }

        public static bool IsLong(this string value)
        {
            return long.TryParse(value, out _);
        }

        public static long ToLong(this string value)
        {
            return long.Parse(value);
        }

        public static bool IsDateTime(this string value)
        {
            return DateTime.TryParse(value, out _);
        }

        public static DateTime ToDateTime(this string value)
        {
            return DateTime.Parse(value);
        }

        public static Guid ToGuid(this string value)
        {
            return Guid.Parse(value);
        }

        public static bool IsGuid(this string value)
        {
            return Guid.TryParse(value, out _);
        }

        public static int GetHashCodeWithNullCheck(this string value)
        {
            if (value == null)
                return 0;

            return value.GetHashCode();
        }

        public static string ToDoubleQuotesWrappedString(this string value)
            => "\"" + value + "\"";

        public static string ToTripleQuotesWrappedString(this string value)
            => $"\"\"\"{value}\"\"\"";
    }
}
