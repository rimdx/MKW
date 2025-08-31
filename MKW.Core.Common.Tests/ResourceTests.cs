using NUnit.Framework.Legacy;

namespace MKW.Core.Common.Tests
{
    public class ResourceTests
    {
        private interface ITestResource : IResource<ITestResource>
        {
            public int Disposed { get; }
        }

        private class TestResource : Resource<ITestResource>, ITestResource
        {
            public int Disposed { get; private set; }

            public override void Dispose(bool disposing)
            {
                Disposed++;
            }
        }

        [Test]
        public void SimpleTest()
        {
            TestResource resource = new TestResource();

            ClassicAssert.AreEqual(0, resource.Disposed);

            resource.Dispose();

            ClassicAssert.AreEqual(1, resource.Disposed);
        }

        [Test]
        public void SimpleMoveTest()
        {
            TestResource resource = new TestResource();
            ClassicAssert.AreEqual(0, resource.Disposed);

            IResource<ITestResource> moved = resource.Move();

            ClassicAssert.AreEqual(0, resource.Disposed);
            ClassicAssert.AreEqual(0, moved.Value.Disposed);

            moved.Dispose();

            ClassicAssert.AreEqual(1, resource.Disposed);
            ClassicAssert.AreEqual(1, moved.Value.Disposed);

            resource.Dispose();
            ClassicAssert.AreEqual(1, resource.Disposed);
            ClassicAssert.AreEqual(1, moved.Value.Disposed);

            moved.Dispose();
            resource.Dispose();

            ClassicAssert.AreEqual(1, resource.Disposed);
            ClassicAssert.AreEqual(1, moved.Value.Disposed);
        }
    }
}
