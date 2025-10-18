// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.Storage;
using MKW.Storage.JSON;
using MKW.Testing.Client;
using NUnit.Framework.Legacy;
using System.Diagnostics;

namespace MKW.Tests
{
    [TestFixture]
    public class DatabaseTests
    {
        [Test]
        [Timeout(2000)]
        public async Task WatcherTests()
        {
            using ClientSandBox sbox = new ClientSandBox(false);

            string basedir = Path.Combine(Path.GetTempPath(), "mkw-tests");
            Directory.CreateDirectory(basedir);
            string db1path = Path.Combine(basedir, "db1.mkw");
            string db2path = Path.Combine(basedir, "db2.mkw");

            using JSONDatabaseSession db1 = JSONDatabaseSession.Create(db1path);

            EntryId entryId = EntryId.Create();

            _ = Task.Run(async () =>
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

            await db1.WaitForDatabaseChangesAsync(default);

            //ClassicAssert.AreEqual(250, timer.ElapsedMilliseconds, 50);
            ClassicAssert.GreaterOrEqual(timer.ElapsedMilliseconds, 250);
            ClassicAssert.AreEqual(0, db1.EnumerateEntries().Count());

            db1.ReloadDatabaseFile();

            ClassicAssert.AreEqual(1, db1.EnumerateEntries().Count());
        }

        [Test]
        [Timeout(500)]
        public async Task WatcherIgnoreOwnChangesTest()
        {
            using ClientSandBox sbox = new ClientSandBox(false);
            using JSONDatabaseSession db = JSONDatabaseSession.Create(sbox.DatabasePath);

            CancellationTokenSource source = new CancellationTokenSource();

            Task assertTask = Task.Run(() =>
            {
                Assert.ThrowsAsync<TaskCanceledException>(async () =>
                {
                    await db.WaitForDatabaseChangesAsync(source.Token);
                });
            });

            EntryId entryId = EntryId.Create();
            db.CreateEntry(entryId, new DatabaseEntry
            {
                Id = entryId,
                Data = ReadOnlyMemory<byte>.Empty,
                Salt = ReadOnlyMemory<byte>.Empty,
                Keys = new Dictionary<UserId, ReadOnlyMemory<byte>>(),
            });

            await Task.Delay(100);

            source.Cancel();

            await assertTask;
        }

        [Test]
        [Timeout(500)]
        public async Task WatcherIgnoreOwnChangesSingleThreadTest()
        {
            using ClientSandBox sbox = new ClientSandBox(false);
            using JSONDatabaseSession db = JSONDatabaseSession.Create(sbox.DatabasePath);

            CancellationTokenSource source = new CancellationTokenSource();

            async Task WaitForChanges()
            {
                try
                {
                    await db.WaitForDatabaseChangesAsync(source.Token);
                }
                catch (TaskCanceledException)
                {
                    // expected
                    return;
                }

                Assert.Fail("WaitForDatabaseChangesAsync must fail");
            }

            Task task = WaitForChanges();

            EntryId entryId = EntryId.Create();
            db.CreateEntry(entryId, new DatabaseEntry
            {
                Id = entryId,
                Data = ReadOnlyMemory<byte>.Empty,
                Salt = ReadOnlyMemory<byte>.Empty,
                Keys = new Dictionary<UserId, ReadOnlyMemory<byte>>(),
            });

            await Task.Delay(100);

            source.Cancel();

            await task;
        }
    }
}
