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
    public class C5 : I1 { }
    public sealed class C6 : C5 { }
    public sealed class C7<T> { }
    public sealed class C8<T> where T : I1 { }

    [UseReporter(typeof(VisualStudioReporter))]
    public sealed class FormatClass
    {
        [Fact] public void C1() => Format(typeof(C1));
        [Fact] public void C2() => Format(typeof(C2));
        [Fact] public void C3() => Format(typeof(C3));
        [Fact] public void C4() => Format(typeof(C4));
        [Fact] public void C5() => Format(typeof(C5));
        [Fact] public void C6() => Format(typeof(C6));
        [Fact] public void C7() => Format(typeof(C7<>));
        [Fact] public void C8() => Format(typeof(C8<>));
        [Fact] public void I1() => Format(typeof(I1));
        [Fact] public void I2() => Format(typeof(I2)) ;

        private static void Format(Type t) => Approvals.Verify(t.GetShape());
    }
}