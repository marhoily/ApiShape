using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ApiShape
{
    internal static class Discovery
    {
        public static IEnumerable<Type> Derives(this Type c)
        {
            if (c.BaseType != null)
                if (c.BaseType != typeof(object))
                    if (!c.IsValueType)
                        yield return c.BaseType;

            foreach (var ifc in c.MinimalSetOfInterfaces()
                .Where(ifc => ifc.IsPublic)
                .OrderBy(t => t.FullName)) yield return ifc;
        }

        public static IEnumerable<MethodInfo> VisibleMethods(this Type t)
        {
            return t
                .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.IsPublic || m.IsFamily)
                .Where(m => !m.IsSpecialName)
                .Where(m => m.DeclaringType == t)
                .Where(m => !m.IsOverride() || m.IsFinal)
                .OrderBy(m => m.Name);
        }
        public static IEnumerable<PropertyInfo> VisibleProperties(this Type t)
        {
            return t
                .GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(GetterOrSetterIsVisible)
                .Where(p => !p.IsOverride())
                .Where(p => p.DeclaringType == t)
                .OrderBy(p => p.Name);
        }
        public static IEnumerable<FieldInfo> VisibleFields(this Type t)
        {
            return t
                .GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f.IsPublic || f.IsFamily)
                .Where(f => f.DeclaringType == t)
                .OrderBy(f => f.Name);
        }
        public static IEnumerable<EventInfo> VisibleEvents(this Type t)
        {
            return t
                .GetEvents(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(e => e.AddMethod.IsPublic || e.AddMethod.IsFamily)
                .Where(e => e.DeclaringType == t)
                .Where(e => !e.IsOverride())
                .OrderBy(e => e.Name);
        }
        private static bool GetterOrSetterIsVisible(PropertyInfo p)
        {
            return p.GetMethod?.IsPublic == true
                   || p.GetMethod?.IsFamily == true
                   || p.SetMethod?.IsPublic == true
                   || p.SetMethod?.IsFamily == true;
        }
    }
}