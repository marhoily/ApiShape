using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiShape
{
    /// <summary> Extension methods container </summary>
    public static class TypeNameFormattingExtensions
    {
        /// <summary>Gets C# name of type </summary>
        public static string CSharpName(this Type type)
        {
            return type.FormatTypeName((t, s) => s);
        }
        private static string FormatTypeName(this Type type, Func<Type, string, string> selectName)
        {
            while (true)
            {
                string niceName;
                if (NiceNames.TryGetValue(type, out niceName)) return niceName;
                if (type.IsArray) return FormatTypeName(type.GetElementType(), selectName) + "[]";
                if (!type.IsGenericType)
                {
                    if (!type.IsByRef) return selectName(type, type.Name);
                    type = type.GetElementType();
                    continue;
                }
                var args = type.GetGenericArguments();
                if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    return FormatTypeName(args[0], selectName) + "?";
                var genericArgs = string.Join(", ",
                    args.Select(a => FormatTypeName(a, selectName)));
                return selectName(type, type.Name.Before("`")) + "<" + genericArgs + ">";
            }
        }
        private static readonly Dictionary<Type, string> NiceNames = new Dictionary<Type, string>
        {
            {typeof (byte), "byte"},
            {typeof (sbyte), "sbyte"},
            {typeof (short), "short"},
            {typeof (ushort), "ushort"},
            {typeof (int), "int"},
            {typeof (uint), "uint"},
            {typeof (long), "long"},
            {typeof (ulong), "ulong"},
            {typeof (void), "void"},
            {typeof (string), "string"},
            {typeof (bool), "bool"},
            {typeof (object), "object"},
        };
    }
}