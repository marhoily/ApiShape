using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using static System.Reflection.GenericParameterAttributes;

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
                select $"where {a.Name} : {cs.Join(c => c.CSharpName())}";
        }

        private static IEnumerable<string> Derives(this Type c)
        {
            if (c.BaseType != null && c.BaseType != typeof(object) && !c.IsValueType)
                yield return c.BaseType.CSharpName();
            var minimalInterfaces = c.GetInterfaces()
                .Except(c.GetInterfaces().SelectMany(t => t.GetInterfaces()))
                .Where(ifc => ifc.IsPublic)
                .OrderBy(t => t.FullName);
            foreach (var ifc in minimalInterfaces)
                yield return ifc.CSharpName();
        }

        private static void WriteClassShape(this Type c, IndentedTextWriter w)
        {
            if (c.BaseType == typeof(Delegate) || c.BaseType == typeof(MulticastDelegate))
                throw new Exception();
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

            w.Write(c.GenericArgName());

            var derives = c.Derives().ToList();
            if (derives.Count <= 0) w.WriteLine();
            else w.WriteLine($" : {derives.Join()}");
            var constraints = c.GetGenericArguments().Constraints();
            w.Indent++;
            foreach (var constraint in constraints)
                w.WriteLine(constraint);
            w.Indent--;
            w.WriteLine("{");
            w.Indent++;
            foreach (var fieldInfo in c.GetFields().OrderBy(t => t.Name)) fieldInfo.WriteShape(w);
            foreach (var propertyInfo in c.GetProperties().OrderBy(t => t.Name))
                if (propertyInfo.DeclaringType == c)
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

        private static void WriteEnumShape(this Type e, IndentedTextWriter w)
        {
            if (!e.IsEnum) throw new ArgumentOutOfRangeException(nameof(e));
            if (e.HasAttribute<FlagsAttribute>()) w.WriteLine("[Flags]");
            w.WriteLine($"enum {e.CSharpName()} : {Enum.GetUnderlyingType(e).CSharpName()}");
            w.WriteLine("{");
            w.Indent++;
            foreach (var fieldInfo in e.GetFields().OrderBy(t => t.Name))
                if (!fieldInfo.IsSpecialName)
                    w.WriteLine($"{fieldInfo.Name} = {(int) fieldInfo.GetValue(null)}");
            w.Indent--;
            w.WriteLine("}");
        }

        private static void WriteDelegateShape(this Type e, IndentedTextWriter w)
        {
            var m = e.GetMethod(nameof(Action.Invoke));
            w.WriteLine(m.ReturnType.CSharpName() +
                        $" delegate {e.GenericArgName()}(" +
                        m.GetParameters().Join(p => $"{p.ParameterType.CSharpName()} {p.Name}") +
                        ");");
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

        private static string GenericArgName(this Type c)
        {
            return c.FormatTypeName(
                (t, s) =>
                {
                    if (!t.IsGenericParameter) return s;
                    if ((t.GenericParameterAttributes & Covariant) != 0)
                        return "out " + s;
                    if ((t.GenericParameterAttributes & Contravariant) != 0)
                        return "in " + s;
                    return s;
                });
        }
    }
}