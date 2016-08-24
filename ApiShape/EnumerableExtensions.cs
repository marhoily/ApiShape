using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiShape
{

    /// <summary>Extension methods container </summary>
    internal static class EnumerableExtensions
    {
        /// <summary>Gets this and all base types in form of a sequence</summary>
        public static IEnumerable<Type> AncestorsAndSelf(this Type t)
        {
            while (t != typeof(object) && t != null)
            {
                yield return t;
                t = t.BaseType;
            }
        }
        /// <summary>Concatenates the members of a collection, using the specified separator between each member </summary>
        public static string Join<T>(this IEnumerable<T> source,
            string separator = ", ") => string.Join(separator, source);
        /// <summary>Concatenates the members of a collection, using the specified separator between each member </summary>
        public static string Join<T>(this IEnumerable<T> source,
            Func<T, object> selector, string separator = ", ") => string.Join(separator, source.Select(selector));
    }
}