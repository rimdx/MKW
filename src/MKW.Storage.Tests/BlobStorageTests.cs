// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.Storage.MKPG;
using NUnit.Framework.Legacy;
using System.Text;

namespace MKW.Storage.Tests
{
    public class BlobStorageTests
    {
        [Test]
        public void SimpleMemoryTest()
        {
            Random random = new Random(42);

            BlobStorageMemory storage = new BlobStorageMemory();
            BlobId id1 = BlobId.From(UserId.Create());
            BlobId id2 = BlobId.From(UserId.Create());

            byte[] data1 = new byte[1024];
            random.NextBytes(data1);

            byte[] data2 = new byte[512];
            random.NextBytes(data2);

            storage.Create(new BlobEntry(id1, MKPGConstants.ArmourTypeHeaders.User, data1));
            storage.Create(new BlobEntry(id2, MKPGConstants.ArmourTypeHeaders.User, data2));

            BlobEntry e1 = storage.Open(id1);
            BlobEntry e2 = storage.Open(id2);

            CollectionAssert.AreEqual(data1, e1.Data.ToArray());
            CollectionAssert.AreEqual(data2, e2.Data.ToArray());

            ClassicAssert.AreEqual(2, storage.Enumerate().Count());

            //Assert.Throws<Exception>(() => storage.Create(new BlobEntry(id1, MKPGConstants.ArmourTypeHeaders.User, data1)));
            //Assert.Throws<Exception>(() => storage.Delete(BlobId.From(UserId.Create())));

            ClassicAssert.AreEqual(true, storage.Exists(id1));
            ClassicAssert.AreEqual(true, storage.Exists(id2));
            ClassicAssert.AreEqual(false, storage.Exists(BlobId.From(UserId.Create())));
        }

        [Test]
        public void SimpleFileTest()
        {
            Random random = new Random(42);

            MemoryEditorFactory editor = new MemoryEditorFactory();
            BlobStorageSingleFile storage = new BlobStorageSingleFile(editor);
            BlobId id1 = BlobId.From(UserId.Create());
            BlobId id2 = BlobId.From(UserId.Create());

            byte[] data1 = new byte[32];
            random.NextBytes(data1);

            byte[] data2 = new byte[42];
            random.NextBytes(data2);

            storage.Create(new BlobEntry(id1, MKPGConstants.ArmourTypeHeaders.User, data1));
            storage.Create(new BlobEntry(id2, MKPGConstants.ArmourTypeHeaders.User, data2));

            BlobEntry e1 = storage.Open(id1);
            BlobEntry e2 = storage.Open(id2);

            CollectionAssert.AreEqual(data1, e1.Data.ToArray());
            CollectionAssert.AreEqual(data2, e2.Data.ToArray());

            ClassicAssert.AreEqual(2, storage.Enumerate().Count());

            Assert.Throws<Exception>(() => storage.Create(new BlobEntry(id1, MKPGConstants.ArmourTypeHeaders.User, data1)));
            // Assert.Throws<Exception>(() => storage.Delete(BlobId.From(UserId.Create())));

            // ClassicAssert.AreEqual(true, storage.Exists(id1));
            // ClassicAssert.AreEqual(true, storage.Exists(id2));
            // ClassicAssert.AreEqual(false, storage.Exists(BlobId.From(UserId.Create())));

            Console.WriteLine(Encoding.ASCII.GetString(editor.ToArray()));
        }

        [Test]
        public void FilterTests()
        {
            Random random = new Random(42);

            MemoryEditorFactory editor = new MemoryEditorFactory();
            BlobStorageSingleFile storage = new BlobStorageSingleFile(editor);

            BlobStorageFiltered users = new BlobStorageFiltered(storage, MKPGConstants.ArmourTypeHeaders.User);
            BlobStorageFiltered entries = new BlobStorageFiltered(storage, MKPGConstants.ArmourTypeHeaders.Entry);

            BlobId id1 = BlobId.From(UserId.Create());
            BlobId id2 = BlobId.From(EntryId.Create());
            BlobId id3 = BlobId.From(UserId.Create());

            byte[] data1 = new byte[32];
            random.NextBytes(data1);

            byte[] data2 = new byte[42];
            random.NextBytes(data2);

            byte[] data3 = new byte[42];
            random.NextBytes(data3);

            users.Create(new BlobEntry(id1, MKPGConstants.ArmourTypeHeaders.User, data1));
            entries.Create(new BlobEntry(id2, MKPGConstants.ArmourTypeHeaders.Entry, data2));
            storage.Create(new BlobEntry(id3, "PGP JUNK", data3));

            Assert.Throws<Exception>(() => users.Create(new BlobEntry(id3, MKPGConstants.ArmourTypeHeaders.User, data1)));

            ClassicAssert.AreEqual(1, users.Enumerate().Count());
            ClassicAssert.AreEqual(1, entries.Enumerate().Count());
            ClassicAssert.AreEqual(3, storage.Enumerate().Count());

            Console.WriteLine(Encoding.ASCII.GetString(editor.ToArray()));
        }
    }
}
