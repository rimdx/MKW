using MKW.GUI.SingleInstance;
using NUnit.Framework.Legacy;

namespace MKW.GUI.Tests
{
    [TestFixture]
    public class SingleInstanceTests
    {
        private class MockApp : ISingleInstanceApplication
        {
            public List<string> Actions { get; }

            public MockApp()
            {
                Actions = [];
            }

            public void InvokeExternalInstance(string[] args)
            {
                Actions.Add($"InvokeExternalInstance({string.Join(", ", args)})");
                Thread.Sleep(250);
            }

            public void InvokeMainInstance(string[] args)
            {
                Actions.Add($"InvokeMainInstance({string.Join(", ", args)})");
                Thread.Sleep(250);
            }
        }

        [Test]
        [NonParallelizable]
        public async Task SimpleTest()
        {
            MockApp appMain = new MockApp();
            SingleInstanceApplicationManager managerMain = new SingleInstanceApplicationManager(appMain);

            MockApp appSecond = new MockApp();
            SingleInstanceApplicationManager managerSecond = new SingleInstanceApplicationManager(appSecond);

            Task task1 = Task.Run(() =>
            {
                managerMain.Run(["123"]);
            });

            Task task2 = Task.Run(() =>
            {
                managerSecond.Run(["456"]);
            });

            await Task.WhenAll(task1, task2);

            CollectionAssert.AreEqual(
                new[]
                {
                    "InvokeMainInstance(123)",
                    "InvokeExternalInstance(456)",
                },
                appMain.Actions);

            CollectionAssert.IsEmpty(appSecond.Actions);
        }
    }
}
