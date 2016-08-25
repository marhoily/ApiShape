using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ApiShape
{
    internal static class Reflection
    {
        public static IEnumerable<Type> MinimalSetOfInterfaces(this Type c)
        {
            var ancestorsInterfaces = c.Ancestors().SelectMany(t => t.GetInterfaces());
            var interfacesInterfaces = c.GetInterfaces().SelectMany(i => i.GetInterfaces());
            return c.GetInterfaces().Except(ancestorsInterfaces.Concat(interfacesInterfaces));
        }
        public static bool IsOverride(this MethodInfo methodInfo) 
            => methodInfo.GetBaseDefinition() != methodInfo;

        public static bool IsOverride(this PropertyInfo p) =>
            p.GetMethod?.IsOverride() == true ||
            p.SetMethod?.IsOverride() == true;

        public static bool IsOverride(this EventInfo e) =>
            e.AddMethod.IsOverride();
    }
}