using NUnit.Framework.Legacy;

namespace MKW.Core.Common.Tests
{
    public class ResourceTests
    {
        private interface ITestResource : IDisposable
        {
            public int Disposed { get; }
        }

        private class TestResource : ITestResource
        {
            public int Disposed { get; private set; }

            public void Dispose()
            {
                Disposed++;
            }
        }

        [Test]
        public void SimpleTest()
        {
            Resource<ITestResource> resource = Resource<ITestResource>.Attach(new TestResource());

            ClassicAssert.AreEqual(0, resource.Value.Disposed);

            resource.Dispose();

            ClassicAssert.AreEqual(1, resource.Value.Disposed);
        }

        [Test]
        public void SimpleMoveTest()
        {
            Resource<ITestResource> resource = Resource<ITestResource>.Attach(new TestResource());
            ClassicAssert.AreEqual(0, resource.Value.Disposed);

            Resource<ITestResource> moved = resource.Move();

            ClassicAssert.AreEqual(0, resource.Value.Disposed);
            ClassicAssert.AreEqual(0, moved.Value.Disposed);

            moved.Dispose();

            ClassicAssert.AreEqual(1, resource.Value.Disposed);
            ClassicAssert.AreEqual(1, moved.Value.Disposed);

            resource.Dispose();
            ClassicAssert.AreEqual(1, resource.Value.Disposed);
            ClassicAssert.AreEqual(1, moved.Value.Disposed);

            moved.Dispose();
            resource.Dispose();

            ClassicAssert.AreEqual(1, resource.Value.Disposed);
            ClassicAssert.AreEqual(1, moved.Value.Disposed);
        }

        [Test]
        public void ExceptionistTests()
        {
            Resource<ITestResource> resource = Resource<ITestResource>.Attach(new TestResource());

            Resource<ITestResource> reference = resource.Reference();
            Resource<ITestResource> owned = resource.Move();

            Assert.Throws<ResourceNotOwnedException>(() => reference.Move());

            owned.Dispose();
            Assert.Throws<ResourceDisposedException>(() => owned.Move());

            Assert.Throws<ResourceNotOwnedException>(() => resource.Move());
        }
    }
}
