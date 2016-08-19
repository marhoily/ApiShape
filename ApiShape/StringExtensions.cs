using System;

namespace ApiShape
{
    /// <summary>Extension methods container </summary>
    public static class StringExtensions
    {
        /// <summary>Gets part of string before a delimiter </summary>
        public static string Before(this string s, string delimiter)
        {
            var idx = s.IndexOf(delimiter, StringComparison.Ordinal);
            if (idx == -1) return s;
            return s.Substring(0, idx);
        }
    }
}