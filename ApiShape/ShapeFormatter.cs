using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using static System.Reflection.BindingFlags;
using static System.Reflection.GenericParameterAttributes;

namespace ApiShape
{
    /// <summary> Extension methods container </summary>
    public static class ShapeFormatter
    {
        /// <summary>Prints out all publically visible artifacts an assembly has</summary>
        public static string GetShape(this Assembly asm)
        {
            var sb = new StringBuilder();
            var w = new IndentedTextWriter(new StringWriter(sb));
            w.WriteLine($"Full name: {asm.FullName}");
            // asm.CustomAttributes
            w.WriteLine($"Image runtime version: {asm.ImageRuntimeVersion}");
            // asm.GetManifestResourceNames()
            foreach (var exportedType in asm.GetExportedTypes().OrderBy(t => t.FullName))
            {
                if (exportedType.BaseType == typeof(MulticastDelegate))
                    exportedType.WriteDelegateShape(w);
                else if (exportedType.IsClass) exportedType.WriteClassShape(w);
                else if (exportedType.IsEnum) exportedType.WriteEnumShape(w);
                else if (exportedType.IsInterface) exportedType.WriteClassShape(w);
                else exportedType.WriteClassShape(w);
            }
            return sb.ToString();
        }

        private static IEnumerable<string> Constraints(this Type[] genericArguments)
        {

            return from a in genericArguments
                   let cs = a.GetGenericParameterConstraints()
                   where cs.Length > 0
                   select $"where {a.Name} : {a.GenericConstraints().Join()}";
        }

        private static IEnumerable<string> Derives(this Type c)
        {
            if (c.BaseType != null && c.BaseType != typeof(object) && !c.IsValueType)
                yield return c.BaseType.CSharpName();
            var minimalInterfaces = c.GetInterfaces()
                .Except(c.GetAllInterfaces())
                .Where(ifc => ifc.IsPublic)
                .OrderBy(t => t.FullName);
            foreach (var ifc in minimalInterfaces)
                yield return ifc.CSharpName();
        }

        private static IEnumerable<Type> GetAllInterfaces(this Type c)
        {
            return c.AncestorsAndSelf().Skip(1)
                .Concat(c.GetInterfaces()).SelectMany(t => t.GetAllInterfaces());
        }
        private static void WriteClassShape(this Type c, IndentedTextWriter w)
        {
            if (!c.IsInterface)
            {
                if (c.IsAbstract) w.Write("abstract ");
                else if (!c.IsSealed) w.Write("virtual ");
            }
            if (c.IsClass) w.Write("class ");
            else if (c.IsInterface) w.Write("interface ");
            else if (c.IsEnum) throw new Exception();
            else if (c.IsValueType) w.Write("struct ");
            if (c.IsNested) w.Write($"{c.DeclaringType.CSharpName()}+");

            w.Write(c.TypeDeclarationName());

            var derives = c.Derives().ToList();
            if (derives.Count > 0) w.Write($" : {derives.Join()}");
            WriteConstraints(w, c.GetGenericArguments());
            w.WriteLine();
            w.WriteLine("{");
            w.Indent++;
            foreach (var fieldInfo in c.GetFields(Instance | Static | Public | NonPublic)
                .Where(f => f.IsPublic || f.IsFamily)
                .OrderBy(t => t.Name)) fieldInfo.WriteShape(w);
            foreach (var propertyInfo in c.VisibleProperties())
                    if (propertyInfo.GetIndexParameters().Length == 0)
                        propertyInfo.WriteShape(w);
            foreach (var constructorInfo in c.GetConstructors()) constructorInfo.WriteShape(w);
            foreach (var methodInfo in c.GetMethods().OrderBy(t => t.Name))
                if (!methodInfo.IsSpecialName && methodInfo.DeclaringType == c)
                    methodInfo.WriteShape(w);
            foreach (var eventInfo in c.GetEvents().OrderBy(t => t.Name))
                eventInfo.WriteShape(w);
            w.Indent--;
            w.WriteLine("}");
        }

        private static IEnumerable<PropertyInfo> VisibleProperties(this Type t)
        {
            return t
                .GetProperties(Instance | Static | Public | NonPublic)
                .Where(GetterOrSetterIsVisible)
                .Where(p => p.DeclaringType == t)
                .OrderBy(x => x.Name);
        }

        private static bool GetterOrSetterIsVisible(PropertyInfo p)
        {
            return p.GetMethod?.IsPublic == true
                   || p.GetMethod?.IsFamily == true
                   || p.SetMethod?.IsPublic == true
                   || p.SetMethod?.IsFamily == true;
        }

        private static void WriteConstraints(IndentedTextWriter w, Type[] getGenericArguments)
        {
            w.Indent++;
            foreach (var constraint in getGenericArguments.Constraints())
            {
                w.WriteLine();
                w.Write(constraint);
            }
            w.Indent--;
        }

        private static void WriteEnumShape(this Type e, IndentedTextWriter w)
        {
            if (!e.IsEnum) throw new ArgumentOutOfRangeException(nameof(e));
            if (e.HasAttribute<FlagsAttribute>()) w.WriteLine("[Flags]");
            w.WriteLine($"enum {e.CSharpName()} : {Enum.GetUnderlyingType(e).CSharpName()}");
            w.WriteLine("{");
            w.Indent++;
            foreach (var fieldInfo in e.GetFields().OrderBy(t => t.Name))
                if (!fieldInfo.IsSpecialName)
                    w.WriteLine($"{fieldInfo.Name} = {(int)fieldInfo.GetValue(null)}");
            w.Indent--;
            w.WriteLine("}");
        }

        private static void WriteShape(this FieldInfo f, IndentedTextWriter w)
        {
            if (f.IsLiteral) w.Write("const ");
            else if (f.IsStatic) w.Write("static ");
            if (f.IsInitOnly) w.Write("readonly ");
            w.WriteLine($"{f.FieldType.CSharpName()} {f.Name};");
        }

        private static void WriteShape(this PropertyInfo p, IndentedTextWriter w)
        {
            var hasGetter = p.GetMethod?.IsPublic == true || p.GetMethod?.IsFamily == true;
            var hasSetter = p.SetMethod?.IsPublic == true || p.SetMethod?.IsFamily == true;
            if (!hasGetter && !hasSetter) return;
            var protectedGet = p.GetMethod?.IsFamily != false;
            var protectedSet = p.SetMethod?.IsFamily != false;
            var protectedProp = protectedGet && protectedSet;
            if (protectedProp) w.Write("protected ");
            w.Write($"{p.PropertyType.CSharpName()} {p.Name} {{ ");

            if (hasGetter)
            {
                if (protectedGet && !protectedProp)
                    w.Write("protected ");
                w.Write("get; ");
            }
            if (hasSetter)
            {
                if (protectedSet && !protectedProp)
                    w.Write("protected ");
                w.Write("set; ");
            }
            w.WriteLine("}");
        }

        private static void WriteShape(this ConstructorInfo m, IndentedTextWriter w)
        {
            if (m.IsVirtual) w.Write("virtual ");
            if (m.IsAbstract) w.Write("abstract ");
            w.Write("constructor");
            WriteParameters(w, m.GetParameters());
            w.WriteLine(";");
        }

        private static void WriteShape(this MethodInfo m, IndentedTextWriter w)
        {
            Debug.Assert(m.DeclaringType != null, "m.DeclaringType != null");
            if (!m.DeclaringType.IsInterface)
            {
                if (m.IsVirtual) w.Write("virtual ");
                if (m.IsAbstract) w.Write("abstract ");
            }

            w.Write(m.ReturnType.CSharpName());
            w.Write(" ");
            w.Write(m.Name);
            if (m.IsGenericMethod)
            {
                w.Write("<" + m.GetGenericArguments().Join(a => a.CSharpName()) + ">");
            }
            WriteParameters(w, m.GetParameters());
            WriteConstraints(w, m.GetGenericArguments());
            w.WriteLine(";");
        }
        private static void WriteDelegateShape(this Type e, IndentedTextWriter w)
        {
            var m = e.GetMethod(nameof(Action.Invoke));
            w.Write("delegate ");
            w.Write(m.ReturnType.CSharpName());
            w.Write(" ");
            w.Write(e.TypeDeclarationName());
            WriteParameters(w, m.GetParameters());
            WriteConstraints(w, e.GetGenericArguments());
            w.WriteLine(";");
        }
        private static void WriteShape(this EventInfo e, IndentedTextWriter w)
        {
            w.WriteLine($"event {e.EventHandlerType.CSharpName()} {e.Name};");
        }

        private static void WriteParameters(IndentedTextWriter w, ParameterInfo[] parameterInfos)
        {
            var args = parameterInfos
                .OrderBy(t => t.Name)
                .Join(Parameter);
            w.Write($"({args})");
        }

        private static string Parameter(ParameterInfo parameterInfo)
        {
            var sb = new StringBuilder();
            if (parameterInfo.IsOut) sb.Append("out ");
            else if (parameterInfo.ParameterType.IsByRef) sb.Append("ref ");
            if (parameterInfo.IsDefined(typeof(ParamArrayAttribute)))
                sb.Append("params ");
            
            sb.Append(parameterInfo.ParameterType.CSharpName());
            sb.Append(" ");
            sb.Append(parameterInfo.Name);
            if (parameterInfo.HasDefaultValue)
                sb.Append($" = {parameterInfo.DefaultValue ?? $"default({parameterInfo.ParameterType.CSharpName()})"}");
            return sb.ToString();
        }

        private static IEnumerable<string> GenericConstraints(this Type a)
        {
            if (a.GenericParameterAttributes.HasFlag(ReferenceTypeConstraint))
                yield return "class";
            if (a.GenericParameterAttributes.HasFlag(NotNullableValueTypeConstraint))
                yield return "struct";
            else
            {
                foreach (var c in a.GetGenericParameterConstraints())
                    yield return c.CSharpName();
                if (a.GenericParameterAttributes.HasFlag(DefaultConstructorConstraint))
                    yield return "new()";
            }
        }
        private static string TypeDeclarationName(this Type c)
        {
            return c.FormatTypeName((t, s) =>
            {
                if (!t.IsGenericParameter) return s;
                if (t.GenericParameterAttributes.HasFlag(Covariant)) return "out " + s;
                if (t.GenericParameterAttributes.HasFlag(Contravariant)) return "in " + s;
                return s;
            });
        }
    }
}