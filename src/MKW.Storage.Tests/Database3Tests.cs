// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.Cryptography;
using MKW.Cryptography.Loader;
using MKW.Storage.JSON;
using MKW.Storage.MKPG;
using MKW.Storage.MKPG.BlobStore;
using MKW.Storage.MKPG.FileSystem;
using NUnit.Framework.Legacy;

namespace MKW.Storage.Tests
{
    [TestFixture(BackendType.MKPGMemory)]
    [TestFixture(BackendType.MKPGMemoryStreamSingleFile)]
    [TestFixture(BackendType.MKPGFileStreamSingleFile)]
    [TestFixture(BackendType.JsonMemory)]
    [TestFixture(BackendType.JsonFile)]
    public class Database3Tests(Database3Tests.BackendType type)
    {
        public enum BackendType
        {
            MKPGMemory,
            MKPGMemoryStreamSingleFile,
            MKPGFileStreamSingleFile,
            JsonMemory,
            JsonFile,
        }

        private IDatabase3 database = default!;

        [SetUp]
        public void Setup()
        {
            if (type == BackendType.MKPGMemory)
            {
                database = new MDatabase(new MKPGSerializer(),
                                         new DatabaseBlobStorageMemory());
            }
            else if (type == BackendType.MKPGMemoryStreamSingleFile)
            {
                database = new MDatabase(new MKPGSerializer(),
                                         new DatabaseBlobStorageSingleFile(new MemoryEditorFactory()));
            }
            else if (type == BackendType.MKPGFileStreamSingleFile)
            {
                database = new MDatabase(new MKPGSerializer(),
                                         new DatabaseBlobStorageSingleFile(new FileSystemEditorFactory(Path.GetTempFileName())));
            }
            else if (type == BackendType.JsonMemory)
            {
                database = new MemoryDatabaseSession();
            }
            else if (type == BackendType.JsonFile)
            {
                database = JSONDatabaseSession.Create(Path.GetTempFileName());
            }
        }

        [TearDown]
        public void TearDown()
        {
            database.Dispose();
        }

        [Test]
        public void SimpleTest()
        {
            ICryptographyProvider crypto = BouncyCastleLoader.GetProvider();
            IRandomGenerator random = crypto.CreateRandomGenerator();

            AsymmetricPrivateKey keypair = crypto.CreateAsymmetricKey(CommonCryptographyAlgorithms.Rsa2048);
            ReadOnlyMemory<byte> pubkey = crypto.EncodePkcsPublicKey(keypair.GetPublicKey());
            ReadOnlyMemory<byte> fakeseckey = random.NextBytes(432);
            ReadOnlyMemory<byte> metadata = random.NextBytes(34);

            DatabaseUser user = new DatabaseUser
            {
                Id = UserId.Create(),
                Salt = random.NextBytes(8),
                PrivateKey = new SecretPayload(fakeseckey),
                ProtectedData = new DatabaseUserProtectedDataSigned
                {
                    PublicKey = pubkey,
                    Metadata = metadata,
                    Signature = new byte[32],
                },
            };
            DatabaseEntry entry = new DatabaseEntry
            {
                Id = EntryId.Create(),
                Data = random.NextBytes(128),
                Salt = random.NextBytes(16),
                Keys = new Dictionary<UserId, ReadOnlyMemory<byte>>(),
            };

            using (IDatabase3.ITransaction transaction = database.BeginTransaction())
            {
                transaction.CreateEntry(entry);
                transaction.CreateUser(user);

                ClassicAssert.AreEqual(1, transaction.EnumerateEntries().Count());
                ClassicAssert.AreEqual(1, transaction.EnumerateUsers().Count());

                transaction.Commit();
            }

            using (IDatabase3.ISnapshot snapshot = database.CreateSnapshot())
            {
                ClassicAssert.AreEqual(1, snapshot.EnumerateEntries().Count());
                ClassicAssert.AreEqual(1, snapshot.EnumerateUsers().Count());
            }
        }
    }
}
