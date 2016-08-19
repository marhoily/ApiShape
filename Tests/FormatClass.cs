using System;
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
    public interface I1 { }
    public interface I2 : I1 { }
    public interface I3<T> { }
    public interface I4<T> where T : I1 { }
    public interface I5 : I2 { }
    public class C5 : I1 { }
    public sealed class C6 : C5 { }
    public sealed class C7<T> { }
    public sealed class C8<T> where T : I1 { }
    public sealed class C9 : C5, I1 { }

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
        [Fact] public void I1() => C(typeof(I1));
        [Fact] public void I2() => C(typeof(I2)) ;
        [Fact] public void I3() => C(typeof(I3<>)) ;
        [Fact] public void I4() => C(typeof(I4<>)) ;
        [Fact] public void I5() => C(typeof(I5)) ;

        [Fact] public void M1() => M(1);
        [Fact] public void M2() => M(2);
        [Fact] public void M3() => M(3);
        [Fact] public void M4() => M(4);
        [Fact] public void M5() => M(5);
        [Fact] public void M6() => M(6);
        [Fact] public void M7() => M(7);
        [Fact] public void M8() => M(8);

        private static void C(Type t) => Approvals.Verify(t.GetShape());
        private static void M(int number) => Approvals.Verify(typeof(M).GetMethod("M" + number).GetShape());
    }
}