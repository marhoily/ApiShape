using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using ApiShape;
using ApprovalTests;
using ApprovalTests.Reporters;
using Xunit;
using static System.Reflection.BindingFlags;

namespace Tests
{
    public class C1 { }
    public sealed class C2 { }
    public abstract class C3 { }
    public sealed class C4 : C3 { }
    public class C5 : I1 { }
    public sealed class C6 : C5 { }
    public sealed class C7<T> { }
    public sealed class C8<T> where T : I1 { }
    public sealed class C9 : C5, I1 { }
    public static class C10 { }

    public interface I1 { }
    public interface I2 : I1 { }
    public interface I3<T> { }
    public interface I4<T> where T : I1 { }
    public interface I5 : I2 { }
    public interface I6<in T> { }
    public interface I7<out T> { }

    public struct S1 { }
    public struct S2 : I1 { }
    public struct S3 : I3<int> { }
    public struct S4<T> : I3<T> { }
    public struct S5<T> : I4<T> where T : I1 { }
    public struct S
    {
        public S(int a1) { F1 = 0; F2 = 0; P1 = 0; P2 = 0;P4 = 0; P5 = 0; }

        public const int X1 = 0;

        public int F1;
        public readonly int F2;
        public static int F4;

        public int P1 { get; }
        public int P2 { get; set; }
        public int P4 { get; private set; }
        public int P5 { private get; set; }
        public static int P13 { get; }

        public string this[int index] { get { return ""; } set { } }
        public string this[float index] { private get { return ""; } set { } }
        public string this[sbyte index] { get { return ""; } private set { } }
        public string this[double index] { set { } }
        public string this[byte index] => "";

        public void M1() { }
        public void M2(int a1) { }
        public void M3(int a1, int a2) { }
        public int M4() => 3;
        public int M5<T>() => 3;
        public int M6<T>() where T : I1 => 3;
        public static void M18() { }

        public static implicit operator S(int x) => new S();
        public static explicit operator int(S x) => 5;
        public static int operator  +(S x, int i) => 5;
    }

    public enum E1 { }
    public enum E2 : short { }
    public enum E3: byte { }
    public enum E4 : ulong { }
    [Flags] public enum E5 { }
    public enum E6 { V0, V1, V2 }
    [Flags] public enum E7
    {
        V0 = 0x1,
        V1 = 0x10,
        V2 = 0x100,
        V3 = 0x1000
    }

    public abstract class MM
    {
        private MM() { }
        private int F1;
        private const int X1 = 0;
        private string this[int index] => "";
        private void M27(params string[] arg) { }
        private event Action E5;

    }
    public abstract class M
    {
        public const int X1 = 0;
        public const string X2 = "blah";
        public const string X3 = null;
        public const double X4 = 0.0;
        public const double X5 = 15e9;

        public int F1;
        public readonly int F2;
        protected int F3;
        public static int F4;

        public int P1 { get; }
        public int P2 { get; set; }
        public int P3 { get; protected set; }
        public int P4 { get; private set; }
        public int P5 { private get; set; }
        public int P6 { protected get; set; }
        protected int P7 { get; }
        protected int P8 { get; set; }
        protected int P9 { private get; set; }
        protected int P10 { get; private set; }
        public abstract int P11 { get; }
        public virtual int P12 { get; }
        public static int P13 { get; }

        public string this[int index] { get { return ""; } set { } }
        protected string this[string index] { get { return ""; } set { } }
        public abstract string this[double index] { get; set; }
        public virtual string this[double index, int arg2] { get { return ""; } set { } }
        public abstract string this[byte i] { protected get; set; }
        public virtual string this[short i] { private get { return ""; } set { } }
        public virtual string this[ushort i] => "";

        protected M() { }
        public M(int a1) { }

        public abstract void M1();
        public abstract void M2(int a1);
        public abstract void M3(int a1, int a2);
        public abstract int M4();
        public abstract int M5<T>();
        public abstract int M6<T>() where T:I1;
        public virtual void M7() { }
        public void M8() { }

        protected abstract void M9();
        protected abstract void M10(int a1);
        protected abstract void M11(int a1, int a2);
        protected abstract int M12();
        protected abstract int M13<T>();
        protected abstract int M14<T>() where T:I1;
        protected virtual void M15() { }
        protected void M16() { }
        public abstract void M17(ref int a1, out int a2);
        public static void M18() { }
        public abstract int M19<T>() where T: class;
        public abstract int M20<T>() where T: struct;
        public abstract int M21<T>() where T: new();
        public abstract int M22<TLongLongLongLongLongArg1, TLongLongLongLongLongArg2, TLongLongLongLongLongArg3>();
        public abstract int M23<TLongLongLongLongLongArg1, TLongLongLongLongLongArg2, TLongLongLongLongLongArg3>(TLongLongLongLongLongArg1 a1, TLongLongLongLongLongArg2 a2, TLongLongLongLongLongArg3 a3);
        public abstract int M24(string longLongLongName, Dictionary<string, List<Uri>> anotherLongName, string notLongEnough);
        public abstract int M25(string arg = null);
        public abstract int M26(params string[] arg);

        public event EventHandler E1;
        public event EventHandler<NetworkAvailabilityEventArgs> E2 { add {} remove {} }
        public event Action E3;
        protected event Action E4;
    }

    public interface IM
    {
        int P1 { get; }
        int P2 { get; set; }
        int P3 { set; }

        void M1();
        void M2(int a1);
        void M3(int a1, int a2);
        int M4();
        int M5<T>();
        int M6<T>() where T : I1;
    }
    [UseReporter(typeof(VisualStudioReporter))]
    public sealed class FormatClass
    {
        [Fact]public void All() => Approvals.Verify(typeof(FormatClass).Assembly.GetShape());
    }
}