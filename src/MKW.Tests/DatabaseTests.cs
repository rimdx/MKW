using MKW.Core;
using MKW.Core.Storage;
using MKW.Core.Storage.JSON;
using MKW.Testing.Client;
using NUnit.Framework.Legacy;
using System.Diagnostics;

namespace MKW.Tests
{
    [TestFixture]
    public class DatabaseTests
    {
        [Test]
        [Timeout(500)]
        public void WatcherTests()
        {
            using ClientSandBox sbox = new ClientSandBox(false);

            string basedir = Path.Combine(Path.GetTempPath(), "mkw-tests");
            Directory.CreateDirectory(basedir);
            string db1path = Path.Combine(basedir, "db1.mkw");
            string db2path = Path.Combine(basedir, "db2.mkw");

            using JSONDatabaseSession db1 = JSONDatabaseSession.Create(db1path);

            // synchroniser
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);

            db1.DatabaseFileUpdated += (sender, e) =>
            {
                autoResetEvent.Set();
            };

            EntryId entryId = EntryId.Create();

            Task.Run(async () =>
            {
                await Task.Delay(250);

                using (JSONDatabaseSession db2 = JSONDatabaseSession.Create(db2path))
                {
                    db2.CreateEntry(entryId, new DatabaseEntry
                    {
                        Id = entryId,
                        Data = ReadOnlyMemory<byte>.Empty,
                        Salt = ReadOnlyMemory<byte>.Empty,
                        Keys = new Dictionary<UserId, ReadOnlyMemory<byte>>(),
                    });
                }

                File.Replace(db2path, db1path, null);
                //File.WriteAllBytes(db1path, File.ReadAllBytes(db2path));
            });

            Stopwatch timer = new Stopwatch();
            timer.Start();
            autoResetEvent.WaitOne();

            ClassicAssert.AreEqual(250, timer.ElapsedMilliseconds, 50);
            ClassicAssert.AreEqual(0, db1.EnumerateEntries().Count());

            db1.ReloadDatabaseFile();

            ClassicAssert.AreEqual(1, db1.EnumerateEntries().Count());
        }
    }
}
