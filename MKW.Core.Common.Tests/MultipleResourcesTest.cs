using NUnit.Framework.Legacy;

namespace MKW.Core.Common.Tests
{
    public class MultipleResourcesTest
    {
        private interface IA : IResource
        {
            int PropA { get; }
        }

        private interface IB : IResource
        {
            int PropB { get; }
        }

        private class A : Resource<A>, IA, IResource
        {
            public int PropA => 11;
            public int Disposed { get; private set; }

            public override void Dispose(bool disposing)
            {
                Disposed++;
            }
        }

        private class B : Resource<B>, IB, IResource
        {
            public int PropB => 22;
            public int Disposed { get; private set; }

            public override void Dispose(bool disposing)
            {
                Disposed++;
            }
        }

        private class Complex : Resource<Complex>, IA, IB, IResource
        {
            private readonly Resource<IA> a;
            private readonly Resource<IB> b;

            public Complex(Resource<IA> a, Resource<IB> b)
            {
                this.a = a;
                this.b = b;
            }

            public int PropA => a.Value.PropA;
            public int PropB => b.Value.PropB;

            public override void Dispose(bool disposing)
            {
                a.Dispose();
                b.Dispose();
            }
        }

        [Test]
        public void Test()
        {
            A a = new A();
            B b = new B();

            ClassicAssert.AreEqual(11, a.PropA);
            ClassicAssert.AreEqual(22, b.PropB);

            Complex complex = new Complex(a.Move<IA>(), b.Reference<IB>());

            ClassicAssert.AreEqual(11, complex.PropA);
            ClassicAssert.AreEqual(22, complex.PropB);

            complex.Dispose();

            ClassicAssert.AreEqual(1, a.Disposed);
            ClassicAssert.AreEqual(0, b.Disposed);

            ClassicAssert.AreEqual(11, complex.PropA); // todo: disposed exception
            ClassicAssert.AreEqual(22, complex.PropB); // todo: disposed exception

            ClassicAssert.AreEqual(11, a.PropA); // todo: exception
            ClassicAssert.AreEqual(22, b.PropB); // we are fine since it was passed as a reference

            a.Dispose();
            b.Dispose();

            ClassicAssert.AreEqual(1, a.Disposed);
            ClassicAssert.AreEqual(1, b.Disposed);

            ClassicAssert.AreEqual(11, a.PropA); // todo: exception
            ClassicAssert.AreEqual(22, b.PropB); // todo: exception
        }
    }
}
