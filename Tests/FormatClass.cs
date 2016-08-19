﻿using System;
using ApiShape;
using ApprovalTests;
using ApprovalTests.Reporters;
using Xunit;

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

    public abstract class M
    {
        public abstract void M1();
        public abstract void M2(int a1);
        public abstract void M3(int a1, int a2);
        public abstract int M4();
        public abstract int M5<T>();
        public abstract int M6<T>() where T:I1;
        public virtual void M7() { }
        public void M8() { }
    }

    public interface IM
    {
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
        [Fact] public void C1() => C(typeof(C1));
        [Fact] public void C2() => C(typeof(C2));
        [Fact] public void C3() => C(typeof(C3));
        [Fact] public void C4() => C(typeof(C4));
        [Fact] public void C5() => C(typeof(C5));
        [Fact] public void C6() => C(typeof(C6));
        [Fact] public void C7() => C(typeof(C7<>));
        [Fact] public void C8() => C(typeof(C8<>));
        [Fact] public void C9() => C(typeof(C9));

        [Fact] public void S1() => S(typeof(S1));
        [Fact] public void S2() => S(typeof(S2));
        [Fact] public void S3() => S(typeof(S3));
        [Fact] public void S4() => S(typeof(S4<>));
        [Fact] public void S5() => S(typeof(S5<>));

        [Fact] public void I1() => C(typeof(I1));
        [Fact] public void I2() => C(typeof(I2)) ;
        [Fact] public void I3() => C(typeof(I3<>)) ;
        [Fact] public void I4() => C(typeof(I4<>)) ;
        [Fact] public void I5() => C(typeof(I5)) ;
        [Fact] public void I6() => C(typeof(I6<>)) ;
        [Fact] public void I7() => C(typeof(I7<>)) ;

        [Fact] public void Im1() => Im(1);
        [Fact] public void Im2() =>Im(2);
        [Fact] public void Im3() =>Im(3);
        [Fact] public void Im4() =>Im(4);
        [Fact] public void Im5() =>Im(5);
        [Fact] public void Im6() =>Im(6);

        [Fact] public void M1() => M(1);
        [Fact] public void M2() => M(2);
        [Fact] public void M3() => M(3);
        [Fact] public void M4() => M(4);
        [Fact] public void M5() => M(5);
        [Fact] public void M6() => M(6);
        [Fact] public void M7() => M(7);
        [Fact] public void M8() => M(8);

        private static void C(Type t) => Approvals.Verify(t.GetShape());
        private static void S(Type t) => Approvals.Verify(t.GetShape());
        private static void M(int number) => Approvals.Verify(typeof(M).GetMethod("M" + number).GetShape());
        private static void Im(int number) => Approvals.Verify(typeof(IM).GetMethod("M" + number).GetShape());
    }
}