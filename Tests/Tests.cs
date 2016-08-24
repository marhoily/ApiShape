using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using ApiShape;
using ApprovalTests;
using ApprovalTests.Reporters;
using Xunit;

namespace Tests
{
    public abstract class Abstract : IInvisible
    {
    }
    [Flags]
    public enum BigFlags
    {
        V0 = 0x1,
        V1 = 0x10,
        V2 = 0x100,
        V3 = 0x1000
    }
    public enum Byte : byte
    {
    }
    public enum Default
    {
    }
    public sealed class DeriveAndImpl : Impl
    {
    }
    public sealed class Derived : Abstract
    {
    }
    public sealed class DeriveLong : Generic<List<List<List<List<List<List<List<List<int>>>>>>>>>
    {
    }
    [Flags]
    public enum Flags
    {
    }
    public class Generic<T>
    {
    }
    public delegate T1 GenericArgsAndReturnValue<out T1, in T2>(T2 a1 = default(T2));
    public delegate void GenericConstraints<T>()
        where T : Abstract, new();
    public struct GenericImplGeneric<T> : IGeneric<T>
    {
    }
    public sealed class GenericWithConstraint<T> 
        where T : IUsual
    {
    }
    public interface IContravariant<in T>
    {
    }
    public interface ICovariant<out T>
    {
    }
    public interface IDerived : IUsual
    {
    }
    public interface IGeneric<T>
    {
    }
    public interface IGenericWithConstraint<T> 
        where T : IUsual
    {
    }
    public interface IIndirectDerive : IDerived
    {
        void F();
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
        int M6<T>() 
            where T : IUsual;
    }
    public class Impl : IUsual
    {
    }
    public sealed class ImplAndDeriveLong : Generic<List<List<List<List<List<List<List<List<int>>>>>>>>>, IGeneric<List<List<List<List<List<List<List<List<int>>>>>>>>>
    {
    }
    public struct ImplGeneric : IGeneric<int>
    {
    }
    public sealed class ImplLong : IGeneric<List<List<List<List<List<List<List<List<int>>>>>>>>>
    {
    }
    public interface IMultiDerived : IDerived, IGeneric<int>
    {
    }
    public sealed class IndirectImpl : Impl
    {
    }
    public struct InheritConstraint<T> : IGenericWithConstraint<T>
        where T : IUsual
    {
    }
    public interface IUsual
    {
    }
    public abstract class M
    {
        public int F1;
        public readonly int F2;
        protected int F3;
        public static int F4;
        public Uri F5;
        public const int X1 = 0;
        public const string X2 = "blah";
        public const string X3 = null;
        public const double X4 = 0.0;
        public const double X5 = 15e9;
        public int P1 { get; }
        public int P2 { get; set; }
        public int P3 { get; protected set; }
        public Uri P4 { get; private set; }
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
        public abstract int M6<T>() where T : IUsual;
        public virtual void M7() { }
        public void M8() { }
        protected abstract void M9();
        protected abstract void M10(int a1);
        protected abstract void M11(Uri a1, int a2);
        protected abstract int M12();
        protected abstract int M13<T>();
        protected abstract int M14<T>() where T : IUsual;
        protected virtual void M15() { }
        protected void M16() { }
        public abstract void M17(ref int a1, out int a2);
        public abstract void M28(ref int longLongLongLongLongLongLongLongArg1, out int longLongLongLongLongLongLongLongArg2);
        public static void M18() { }
        public abstract int M19<T>() where T : class;
        public abstract int M20<T>() where T : struct;
        public abstract int M21<T>() where T : new();
        public abstract int M28<T>() 
            where T : class, IGeneric<int>, new();
        public abstract int M22<TLongLongLongLongLongArg1, TLongLongLongLongLongArg2, TLongLongLongLongLongArg3>();
        public abstract int M23<TLongLongLongLongLongArg1, TLongLongLongLongLongArg2, TLongLongLongLongLongArg3>(TLongLongLongLongLongArg1 a1, TLongLongLongLongLongArg2 a2, TLongLongLongLongLongArg3 a3);
        public abstract int M24(string longLongLongName, Dictionary<string, List<Uri>> anotherLongName, string notLongEnough);
        public abstract int M25(string arg = null);
        public abstract int M29(int a1, int a2, string longLongLongLongLongLongLongLongArg1 = null);
        public abstract int M26(params string[] arg);
        public abstract int M27<TLongLongLongLongLongArg1, TLongLongLongLongLongArg2, TLongLongLongLongLongArg3>()
            where TLongLongLongLongLongArg1 : ICloneable;
        public event EventHandler E1;
        public event EventHandler<NetworkAvailabilityEventArgs> E2 { add { } remove { } }
        public event Action E3;
        protected event Action E4;
        public sealed class NestedPublic
        {
        }
        protected class NestedProtected
        {
        }
        internal class NestedInternal
        {
        }
        private class NestedPrivate
        {
        }
    }
    public abstract class MM : M
    {
        private MM() { }
        private int F1;
        private const int X1 = 0;
        private string this[int index] => "";
        private void M27(params string[] arg) { }
        private event Action E5;

        protected MM(int f1) { F1 = f1; }
        protected MM(int a1, int f1) : base(a1) { F1 = f1; }
        public override int P11 { get; }
        public override int P12 { get; }
        public override string this[double index] { get { return ""; } set {  } }
        public override void M1() { }
    }
    public class NonSealed
    {
    }
    public delegate void OutAndRefAndParams(out int a1, ref string a2, params object[] a3);
    public delegate int ReturnValueAndArg(int a1, int opt = 0);
    public delegate void SampleDelegate();
    public sealed class Sealed
    {
    }
    public enum Seq
    {
        V0,
        V1,
        V2
    }
    public enum Short : short
    {
    }
    public static class Static
    {
    }
    public struct Struct
    {
    }
    public struct StructContainer : IIndirectDerive
    {
        public int F1;
        public readonly int F2;
        public static int F4;
        public const int X1 = 0;
        public int P1 { get; }
        public static int P13 { get; }
        public int P2 { get; set; }
        public int P4 { get; private set; }
        public int P5 { private get; set; }
        public StructContainer(int a1) { F1 = 0; F2 = 0; P1 = 0; P2 = 0; P4 = 0; P5 = 0; }
        public void F() { }
        public void M1() { }
        public static void M18() { }
        public void M2(int a1) { }
        public void M3(int a1, int a2) { }
        public int M4() => 3;
        public int M5<T>() => 3;
        public int M6<T>() 
			where T : IUsual => 3;
        public override string ToString() => "";
        public string this[int index] { get { return ""; } set { } }
        public string this[float index] { private get { return ""; } set { } }
        public string this[sbyte index] { get { return ""; } private set { } }
        public string this[double index] { set { } }
        public string this[byte index] => "";
        public static implicit operator StructContainer(int x) => new StructContainer();
        public static explicit operator int(StructContainer x) => 5;
        public static int operator +(StructContainer x, int i) => 5;
    }
    public struct StructImpl : IUsual
    {
    }
    public enum ULong : ulong
    {
    }
    [UseReporter(typeof(VisualStudioReporter))]
    public sealed class Tests
    {
        [Fact]
        public void All() => Approvals.Verify(typeof(Tests).Assembly.GetShape());
    }
    internal interface IInvisible { }
    internal struct Invisible { }
}