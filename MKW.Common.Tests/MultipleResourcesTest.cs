using NUnit.Framework.Legacy;

namespace MKW.Core.Common.Tests
{
    public class MultipleResourcesTest
    {
        private interface IDisposeCounter
        {
            int Disposed { get; }
        }

        private interface IA : IDisposeCounter, IDisposable
        {
            int PropA { get; }
        }

        private interface IB : IDisposeCounter, IDisposable
        {
            int PropB { get; }
        }

        private class A : IA, IDisposeCounter, IDisposable
        {
            public int PropA => 11;
            public int Disposed { get; private set; }

            public void Dispose()
            {
                Disposed++;
            }
        }

        private class B : IB, IDisposeCounter, IDisposable
        {
            public int PropB => 22;
            public int Disposed { get; private set; }

            public void Dispose()
            {
                Disposed++;
            }
        }

        private class Complex : IA, IB, IDisposeCounter, IDisposable
        {
            private readonly Resource<IA> a;
            private readonly Resource<IB> b;

            public Complex(Resource<IA> a, Resource<IB> b)
            {
                this.a = a;
                this.b = b;
            }

            public int Disposed { get; private set; }

            public int PropA => a.Value.PropA;
            public int PropB => b.Value.PropB;

            public void Dispose()
            {
                a.Dispose();
                b.Dispose();
                Disposed++;
            }
        }

        [Test]
        public void Test()
        {
            Resource<IA> a = Resource<IA>.Attach(new A());
            Resource<IB> b = Resource<IB>.Attach(new B());

            ClassicAssert.AreEqual(11, a.Value.PropA);
            ClassicAssert.AreEqual(22, b.Value.PropB);

            Complex complex = new Complex(a.Move(), b.Reference());

            ClassicAssert.AreEqual(11, complex.PropA);
            ClassicAssert.AreEqual(22, complex.PropB);

            complex.Dispose();

            ClassicAssert.AreEqual(1, a.Value.Disposed);
            ClassicAssert.AreEqual(0, b.Value.Disposed);

            ClassicAssert.AreEqual(11, complex.PropA); // todo: disposed exception
            ClassicAssert.AreEqual(22, complex.PropB); // todo: disposed exception

            ClassicAssert.AreEqual(11, a.Value.PropA); // todo: exception
            ClassicAssert.AreEqual(22, b.Value.PropB); // we are fine since it was passed as a reference

            a.Dispose();
            b.Dispose();

            ClassicAssert.AreEqual(1, a.Value.Disposed);
            ClassicAssert.AreEqual(1, b.Value.Disposed);

            ClassicAssert.AreEqual(11, a.Value.PropA); // todo: exception
            ClassicAssert.AreEqual(22, b.Value.PropB); // todo: exception
        }
    }
}
