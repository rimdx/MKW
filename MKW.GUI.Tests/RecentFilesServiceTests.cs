using NUnit.Framework.Legacy;

namespace MKW.GUI.Tests
{
    [TestFixture]
    public class RecentFilesServiceTests
    {
        [Test]
        public void SimpleTest()
        {
            using SandBox sbox = new SandBox();
            RecentFilesService recentFilesService = new RecentFilesService(sbox.RegistryService);

            CollectionAssert.IsEmpty(recentFilesService.EnumerateRecentFiles());

            recentFilesService.OnFileOpened(@"F:\path\to\file1.txt");
            recentFilesService.OnFileOpened(@"F:\path\to\file2.txt");

            CollectionAssert.AreEqual(
                new[]
                {
                    @"F:\path\to\file2.txt",
                    @"F:\path\to\file1.txt",
                },
                recentFilesService.EnumerateRecentFiles());
        }

        [Test]
        public void OverloadTest()
        {
            using SandBox sbox = new SandBox();
            RecentFilesService recentFilesService = new RecentFilesService(sbox.RegistryService);

            recentFilesService.OnFileOpened(@"F:\path\to\file1.txt");
            recentFilesService.OnFileOpened(@"F:\path\to\file2.txt");
            recentFilesService.OnFileOpened(@"F:\path\to\file3.txt");
            recentFilesService.OnFileOpened(@"F:\path\to\file4.txt");
            recentFilesService.OnFileOpened(@"F:\path\to\file5.txt");
            recentFilesService.OnFileOpened(@"F:\path\to\file6.txt");
            recentFilesService.OnFileOpened(@"F:\path\to\file7.txt");
            recentFilesService.OnFileOpened(@"F:\path\to\file8.txt");

            CollectionAssert.AreEqual(
                new[]
                {
                    @"F:\path\to\file8.txt",
                    @"F:\path\to\file7.txt",
                    @"F:\path\to\file6.txt",
                    @"F:\path\to\file5.txt",
                    @"F:\path\to\file4.txt",
                },
                recentFilesService.EnumerateRecentFiles());
        }
    }
}
