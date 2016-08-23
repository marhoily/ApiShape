using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ApiShape
{
    public static class ShapeFormatter
    {
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
                if (exportedType.IsClass) exportedType.WriteClassShape(w);
                else if (exportedType.IsEnum) exportedType.WriteEnumShape(w);
                else if (exportedType.IsInterface) exportedType.WriteClassShape(w);
                else exportedType.WriteClassShape(w);
            }
            return sb.ToString();
        }

        private static IEnumerable<string> Derives(this Type c)
        {
            if (c.BaseType != null && c.BaseType != typeof(object))
                yield return c.BaseType.CSharpName();
            foreach (var ifc in c.GetInterfaces().OrderBy(t => t.FullName))
                yield return ifc.CSharpName();
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
            w.Write(c.CSharpName());
            var enumerable = c.Derives().ToList();
            if (enumerable.Count > 0)
                w.WriteLine($" : {enumerable.Join()}");
            else w.WriteLine();
            w.WriteLine("{");
            w.Indent++;
            foreach (var fieldInfo in c.GetFields().OrderBy(t => t.Name)) fieldInfo.WriteShape(w);
            foreach (var propertyInfo in c.GetProperties().OrderBy(t => t.Name))
                if (propertyInfo.DeclaringType == c)
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
            w.Write($"{p.PropertyType.CSharpName()} {p.Name} {{ ");
            if (p.GetGetMethod()?.IsPublic == true) w.Write("get; ");
            if (p.GetSetMethod()?.IsPublic == true) w.Write("set; ");
            w.WriteLine("}");
        }
        private static void WriteShape(this ConstructorInfo m, IndentedTextWriter w)
        {
            if (m.IsVirtual) w.Write("virtual ");
            if (m.IsAbstract) w.Write("abstract ");

            w.Write("constructor");

            WriteParameters(w, m.GetParameters());
        }
        private static void WriteShape(this MethodInfo m, IndentedTextWriter w)
        {
            if (m.IsVirtual) w.Write("virtual ");
            if (m.IsAbstract) w.Write("abstract ");

            w.Write(m.ReturnType.CSharpName());
            w.Write(" ");
            w.Write(m.Name);
            if (m.IsGenericMethod)
            {
                WriteGenericParameters(w, m.GetGenericArguments());
            }
            WriteParameters(w, m.GetParameters());
        }
        private static void WriteShape(this EventInfo e, IndentedTextWriter w)
        {
            w.WriteLine($"event {e.EventHandlerType.CSharpName()} {e.Name};");
        }

        private static void WriteParameters(IndentedTextWriter w, ParameterInfo[] parameterInfos)
        {
            parameterInfos = parameterInfos.OrderBy(t => t.Name).ToArray();
            var oneLine = WriteParametersOneLine(parameterInfos);
            if (oneLine.Length < 50) w.WriteLine(oneLine);
            else WriteParametersMultiLine(w, parameterInfos);
        }
        private static string WriteParametersOneLine(ParameterInfo[] parameterInfos)
        {
            if (parameterInfos.Length == 0) return "();";
            var w = new StringBuilder();
            w.Append("(");
            foreach (var parameterInfo in parameterInfos)
            {
                if (parameterInfo.IsOut)
                    w.Append(parameterInfo.IsIn ? "ref " : "out ");
                w.Append($"{parameterInfo.ParameterType.CSharpName()} {parameterInfo.Name}");
                if (parameterInfo.HasDefaultValue)
                    w.Append($" = {parameterInfo.RawDefaultValue}");
                w.Append(", ");
            }
            w.Remove(w.Length - 2, 2);
            w.Append(");");
            return w.ToString();
        }
        private static void WriteParametersMultiLine(IndentedTextWriter w, ParameterInfo[] parameterInfos)
        {
            if (parameterInfos.Length == 0) w.WriteLine("();");
            else
            {
                w.WriteLine("(");
                w.Indent++;
                foreach (var parameterInfo in parameterInfos)
                {
                    if (parameterInfo.IsOut)
                        w.Write(parameterInfo.IsIn ? "ref " : "out ");
                    w.Write($"{parameterInfo.ParameterType.CSharpName()} {parameterInfo.Name}");
                    if (parameterInfo.HasDefaultValue)
                        w.Write($" = {parameterInfo.RawDefaultValue}");
                    w.WriteLine();
                }
                w.Indent--;
                w.WriteLine(");");
            }
        }

        private static void WriteGenericParameters(IndentedTextWriter w, Type[] args)
        {
            args = args.OrderBy(t => t.Name).ToArray();
            var oneLine = WriteGenericParametersOneLine(args);
            if (oneLine.Length < 50) w.Write(oneLine);
            else WriteGenericParametersMultiLine(w, args);

        }
        private static string WriteGenericParametersOneLine(Type[] args)
        {
            var w = new StringBuilder();
            w.Append("<");
            foreach (var genericArgument in args)
            {
                w.Append(genericArgument.Name);
                var constraints = genericArgument.GetGenericParameterConstraints();
                if (constraints.Length > 0)
                {
                    w.Append($" : {constraints.Join(c => c.CSharpName())}");
                }
                w.Append(", ");
            }
            w.Remove(w.Length - 2, 2);
            w.Append(">");
            return w.ToString();
        }
        private static void WriteGenericParametersMultiLine(IndentedTextWriter w, Type[] args)
        {
            w.WriteLine("<");
            w.Indent++;
            foreach (var genericArgument in args)
            {
                w.Write(genericArgument.Name);
                var constraints = genericArgument.GetGenericParameterConstraints();
                if (constraints.Length > 0)
                {
                    w.Write($" : {constraints.Join(c => c.CSharpName())}");
                }
                w.WriteLine();
            }
            w.Indent--;
            w.Write(">");
        }
    }
}