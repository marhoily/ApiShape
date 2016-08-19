using System;
using System.Collections.Generic;
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
        [Fact] public void C1() => T(typeof(C1));
        [Fact] public void C2() => T(typeof(C2));
        [Fact] public void C3() => T(typeof(C3));
        [Fact] public void C4() => T(typeof(C4));
        [Fact] public void C5() => T(typeof(C5));
        [Fact] public void C6() => T(typeof(C6));
        [Fact] public void C7() => T(typeof(C7<>));
        [Fact] public void C8() => T(typeof(C8<>));
        [Fact] public void C9() => T(typeof(C9));
        [Fact] public void C10() => T(typeof(C10));

        [Fact] public void I1() => T(typeof(I1));
        [Fact] public void I2() => T(typeof(I2)) ;
        [Fact] public void I3() => T(typeof(I3<>)) ;
        [Fact] public void I4() => T(typeof(I4<>)) ;
        [Fact] public void I5() => T(typeof(I5)) ;
        [Fact] public void I6() => T(typeof(I6<>)) ;
        [Fact] public void I7() => T(typeof(I7<>)) ;

        [Fact] public void S1() => T(typeof(S1));
        [Fact] public void S2() => T(typeof(S2));
        [Fact] public void S3() => T(typeof(S3));
        [Fact] public void S4() => T(typeof(S4<>));
        [Fact] public void S5() => T(typeof(S5<>));

        [Fact] public void E1() => T(typeof(E1));
        [Fact] public void E2() => T(typeof(E2));
        [Fact] public void E3() => T(typeof(E3));
        [Fact] public void E4() => T(typeof(E4));
        [Fact] public void E5() => T(typeof(E5));
        [Fact] public void E6() => T(typeof(E6));
        [Fact] public void E7() => T(typeof(E7));

        [Fact] public void X1() => X(1);
        [Fact] public void X2() =>X(2);
        [Fact] public void X3() =>X(3);
        [Fact] public void X4() =>X(4);
        [Fact] public void X5() =>X(5);
        
        [Fact] public void F1() => F(1);
        [Fact] public void F2() =>F(2);
        [Fact] public void F3() =>F(3);
        [Fact] public void F4() =>F(4);
        
        [Fact] public void P1() =>  P(1);
        [Fact] public void P2() => P(2);
        [Fact] public void P3() => P(3);
        [Fact] public void P4() => P(4);
        [Fact] public void P5() => P(5);
        [Fact] public void P6() => P(6);
        [Fact] public void P7() => P(7);
        [Fact] public void P8() => P(8);
        [Fact] public void P9() => P(9);
        [Fact] public void P10() => P(10);


        [Fact] public void Im1() => Im(1);
        [Fact] public void Im2() =>Im(2);
        [Fact] public void Im3() =>Im(3);
        [Fact] public void Im4() =>Im(4);
        [Fact] public void Im5() =>Im(5);
        [Fact] public void Im6() =>Im(6);

        [Fact] public void Ip1() => Ip(1);
        [Fact] public void Ip2() =>Ip(2);
        [Fact] public void Ip3() =>Ip(3);

        [Fact] public void M1() => M(1);
        [Fact] public void M2() => M(2);
        [Fact] public void M3() => M(3);
        [Fact] public void M4() => M(4);
        [Fact] public void M5() => M(5);
        [Fact] public void M6() => M(6);
        [Fact] public void M7() => M(7);
        [Fact] public void M8() => M(8);
        [Fact] public void M9() => M(9);
        [Fact] public void M10() => M(10);
        [Fact] public void M11() => M(11);
        [Fact] public void M12() => M(12);
        [Fact] public void M13() => M(13);
        [Fact] public void M14() => M(14);
        [Fact] public void M15() => M(15);
        [Fact] public void M16() => M(16);
        [Fact] public void M17() => M(17);

        private static void T(Type t) => Approvals.Verify(t.GetShape());
        private static void M(int number) => Approvals.Verify(typeof(M).GetMethod("M" + number, Public | NonPublic | Instance).GetShape());
        private static void P(int number) => Approvals.Verify(typeof(M).GetProperty("P" + number, Public | NonPublic | Instance).GetShape());
        private static void F(int number) => Approvals.Verify(typeof(M).GetField("F" + number, Public | NonPublic | Instance | Static).GetShape());
        private static void X(int number) => Approvals.Verify(typeof(M).GetField("X" + number, Public | NonPublic | Static).GetShape());
        private static void Im(int number) => Approvals.Verify(typeof(IM).GetMethod("M" + number, Public | NonPublic | Instance).GetShape());
        private static void Ip(int number) => Approvals.Verify(typeof(IM).GetProperty("P" + number, Public | NonPublic | Instance).GetShape());
    }
}