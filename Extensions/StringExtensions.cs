﻿using System;

namespace Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string e) => string.IsNullOrWhiteSpace(e);

        public static bool IsNullOrWhiteSpace(this string e, out string value)
        {
            value = e;
            return string.IsNullOrEmpty(e);
        }

        public static int ToInt32(this string e) => Convert.ToInt32(string.IsNullOrEmpty(e) ? "0" : e);

        public static bool ToBoolean(this string e) => Convert.ToBoolean(Convert.ToInt32(string.IsNullOrEmpty(e) ? "0" : e));

        public static decimal ToDecimal(this string e) => Convert.ToDecimal(Convert.ToInt32(string.IsNullOrEmpty(e) ? "0" : e));
        public static long ToInt64(this string e) => Convert.ToInt64(Convert.ToInt32(string.IsNullOrEmpty(e) ? "0" : e));
    }
}
