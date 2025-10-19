// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.Storage.MKPG;
using NUnit.Framework.Legacy;
using System.Text;

namespace MKW.Storage.Tests
{
    [TestFixture(BackendType.Memory)]
    [TestFixture(BackendType.MemoryStreamSingleFile)]
    [TestFixture(BackendType.FileStreamSingleFile)]
    public class BlobStorageTests(BlobStorageTests.BackendType type)
    {
        public enum BackendType
        {
            Memory,
            MemoryStreamSingleFile,
            FileStreamSingleFile,
        }

        private IFileEditorFactory? editor;
        private IBlobStorage backend = default!;

        [SetUp]
        public void Setup()
        {
            if (type == BackendType.Memory)
            {
                editor = null;
                backend = new BlobStorageMemory();
            }
            else if (type == BackendType.MemoryStreamSingleFile)
            {
                editor = new MemoryEditorFactory();
                backend = new BlobStorageSingleFile(editor);
            }
            else if (type == BackendType.FileStreamSingleFile)
            {
                editor = new FileSystemEditorFactory(Path.GetTempFileName());
                backend = new BlobStorageSingleFile(editor);
            }
        }

        [TearDown]
        public void TearDown()
        {
            if (editor is MemoryEditorFactory memory)
            {
                Console.WriteLine(Encoding.ASCII.GetString(memory.ToArray()));
            }
            else if (editor is FileSystemEditorFactory fileSystem)
            {
                Console.WriteLine($"Path: {fileSystem.Path}");
                Console.WriteLine(File.ReadAllText(fileSystem.Path));
            }

            editor?.Dispose();
        }

        [Test]
        public void SimpleMemoryTest()
        {
            Random random = new Random(42);

            BlobId id1 = BlobId.From(UserId.Create());
            BlobId id2 = BlobId.From(UserId.Create());

            byte[] data1 = new byte[1024];
            random.NextBytes(data1);

            byte[] data2 = new byte[512];
            random.NextBytes(data2);

            backend.Create(new BlobEntry(id1, MKPGConstants.ArmourTypeHeaders.User, data1));
            backend.Create(new BlobEntry(id2, MKPGConstants.ArmourTypeHeaders.User, data2));

            BlobEntry e1 = backend.Open(id1);
            BlobEntry e2 = backend.Open(id2);

            CollectionAssert.AreEqual(data1, e1.Data.ToArray());
            CollectionAssert.AreEqual(data2, e2.Data.ToArray());

            ClassicAssert.AreEqual(2, backend.Enumerate().Count());

            Assert.Throws<Exception>(() => backend.Create(new BlobEntry(id1, MKPGConstants.ArmourTypeHeaders.User, data1)));
            //Assert.Throws<Exception>(() => storage.Delete(BlobId.From(UserId.Create())));

            //ClassicAssert.AreEqual(true, backend.Exists(id1));
            //ClassicAssert.AreEqual(true, backend.Exists(id2));
            //ClassicAssert.AreEqual(false, backend.Exists(BlobId.From(UserId.Create())));
        }

        [Test]
        public void FilterTests()
        {
            Random random = new Random(42);

            BlobStorageFiltered users = new BlobStorageFiltered(backend, MKPGConstants.ArmourTypeHeaders.User);
            BlobStorageFiltered entries = new BlobStorageFiltered(backend, MKPGConstants.ArmourTypeHeaders.Entry);

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
            backend.Create(new BlobEntry(id3, "PGP JUNK", data3));

            //Assert.Throws<Exception>(() => users.Create(new BlobEntry(id3, MKPGConstants.ArmourTypeHeaders.User, data1)));

            ClassicAssert.AreEqual(1, users.Enumerate().Count());
            ClassicAssert.AreEqual(1, entries.Enumerate().Count());
            ClassicAssert.AreEqual(3, backend.Enumerate().Count());
        }
    }
}
