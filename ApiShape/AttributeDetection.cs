using System;
using System.Reflection;

namespace ApiShape
{
    /// <summary>Where to look when searching for attributes </summary>
    internal enum LookAt
    {
        /// <summary>Only look at the type\member itself</summary>
        Self,
        /// <summary>Search inheritance chain to find the attributes; 
        ///     This option is ignored for properties and events;</summary>
        SelfAndAncestors
    }
    /// <summary>Extension methods related to attribute detection </summary>
    internal static class AttributeDetection
    {
        /// <summary>Generic version of <see cref="MemberInfo.IsDefined"/> </summary>
        public static bool HasAttribute<TAttribute>(this MemberInfo method, LookAt lookAt = LookAt.Self) 
            where TAttribute : Attribute 
            => method.IsDefined(typeof(TAttribute), lookAt == LookAt.SelfAndAncestors);
    }
}
