﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Sfa.Tl.ResultsAndCertification.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool IsDateTimeWithFormat(this string value)
        {
            return DateTime.TryParseExact(value, "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }

        public static DateTime ParseStringToDateTime(this string value)
        {
            DateTime.TryParseExact(value, "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result);
            return result;
        }

        public static long ToLong(this string value)
        {
            return long.Parse(value);
        }

        public static bool IsGuid(this string value)
        {
            return Guid.TryParse(value, out _);
        }
    }
}
