// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.Storage.Exceptions;
using MKW.Storage.MKPG.BlobStore;
using MKW.Storage.MKPG.FileSystem;
using NUnit.Framework.Legacy;

namespace MKW.Storage.Tests
{
    [TestFixture(BackendType.Memory)]
    [TestFixture(BackendType.MemoryStreamSingleFile)]
    [TestFixture(BackendType.FileStreamSingleFile)]
    public class BlobStorageTests2(BlobStorageTests2.BackendType type)
    {
        public enum BackendType
        {
            Memory,
            MemoryStreamSingleFile,
            FileStreamSingleFile,
        }

        private IDatabaseBlobStore store = default!;

        [SetUp]
        public void Setup()
        {
            if (type == BackendType.Memory)
            {
                store = new DatabaseBlobStorageMemory();
            }
            else if (type == BackendType.MemoryStreamSingleFile)
            {
                store = new DatabaseBlobStorageSingleFile(new MemoryEditorFactory());
            }
            else if (type == BackendType.FileStreamSingleFile)
            {
                store = new DatabaseBlobStorageSingleFile(new FileSystemEditorFactory(Path.GetTempFileName()));
            }
        }

        [TearDown]
        public void TearDown()
        {
            store.Dispose();
        }

        [Test]
        public void SimpleTest()
        {
            Random random = new Random(42);

            BlobId id1 = BlobId.From(UserId.Create());
            BlobId id2 = BlobId.From(UserId.Create());

            byte[] data1 = new byte[1024];
            random.NextBytes(data1);

            byte[] data2 = new byte[512];
            random.NextBytes(data2);

            using (IDatabaseBlobStore.ITransaction transaction = store.BeginTransaction())
            {
                transaction.Create(new BlobUser
                {
                    Id = id1,
                    Data = data1
                });

                transaction.Create(new BlobUser
                {
                    Id = id2,
                    Data = data2
                });

                transaction.Commit();
            }

            {
                IDatabaseBlobStore.ISnapshot snapshot = store.CreateSnapshot();

                Blob[] entries = [.. snapshot.Enumerate()];

                ClassicAssert.AreEqual(2, entries.Length);
                CollectionAssert.AreEqual(data1, entries[0].Data.ToArray());
                CollectionAssert.AreEqual(data2, entries[1].Data.ToArray());
            }

            using (IDatabaseBlobStore.ITransaction transaction = store.BeginTransaction())
            {
                Assert.Throws<EntryAlreadyExistsException>(
                    () =>
                    {
                        transaction.Create(new BlobUser
                        {
                            Id = id1,
                            Data = data1
                        });
                    });
            }

            using (IDatabaseBlobStore.ITransaction transaction = store.BeginTransaction())
            {
                ClassicAssert.AreEqual(false, transaction.Delete(BlobId.From(UserId.Create())));
                ClassicAssert.AreEqual(true, transaction.Delete(id2));
                ClassicAssert.AreEqual(1, transaction.Enumerate().Count());
                transaction.Commit();
            }

            {
                IDatabaseBlobStore.ISnapshot snapshot = store.CreateSnapshot();
                ClassicAssert.AreEqual(1, snapshot.Enumerate().Count());
            }
        }
    }
}
