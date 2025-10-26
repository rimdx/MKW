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

        [Test]
        public void SimpleEntryTest()
        {
            Random random = new Random(42);

            byte[] data = new byte[128];
            random.NextBytes(data);

            DatabaseEntry dbEntry = new DatabaseEntry
            {
                // Id = EntryId.FromGuid(new Guid("{00000000-0000-0000-A2E4-51CAEA3D7ABD}")),
                Id = EntryId.Create(),
                Data = data,
                Salt = null,
                Keys = new Dictionary<UserId, ReadOnlyMemory<byte>>(),
            };

            using (IDatabase3.ITransaction transaction = database.BeginTransaction())
            {
                transaction.CreateEntry(dbEntry);

                DatabaseEntry read = transaction.OpenEntry(dbEntry.Id);

                ClassicAssert.AreEqual(dbEntry.Id, read.Id);
                CollectionAssert.AreEqual(dbEntry.Data.ToArray(), read.Data.ToArray());
                CollectionAssert.AreEqual(dbEntry.Keys, read.Keys);

                transaction.CreateEntry(dbEntry with { Id = EntryId.Create() });
                transaction.CreateEntry(dbEntry with { Id = EntryId.Create() });
                transaction.CreateEntry(dbEntry with { Id = EntryId.Create() });

                transaction.Commit();
            }

            DatabaseEntry[] entries = database.CreateSnapshot().EnumerateEntries().ToArray();

            ClassicAssert.AreEqual(4, entries.Length);
        }

        [Test]
        public void SimpleUpdateEntryTest()
        {
            Random random = new Random(42);

            byte[] data = new byte[128];
            random.NextBytes(data);

            byte[] data2 = new byte[128];
            random.NextBytes(data2);

            DatabaseEntry dbEntry = new DatabaseEntry
            {
                Id = EntryId.Create(),
                Data = data,
                Salt = null,
                Keys = new Dictionary<UserId, ReadOnlyMemory<byte>>(),
            };

            EntryId theid = EntryId.Create();
            EntryId someid = EntryId.Create();

            using (IDatabase3.ITransaction transaction = database.BeginTransaction())
            {
                transaction.CreateEntry(dbEntry with { Id = EntryId.Create() });
                transaction.CreateEntry(dbEntry with { Id = someid });
                transaction.CreateEntry(dbEntry with { Id = theid });
                transaction.CreateEntry(dbEntry with { Id = EntryId.Create() });

                transaction.UpdateEntry(dbEntry with
                {
                    Id = theid,
                    Data = data2,
                });

                transaction.Commit();
            }

            CollectionAssert.AreEqual(data2, database.CreateSnapshot().OpenEntry(theid).Data.ToArray());
            CollectionAssert.AreEqual(data, database.CreateSnapshot().OpenEntry(someid).Data.ToArray());
        }

        [Test]
        public void SimpleUserTest()
        {
            ICryptographyProvider crypto = BouncyCastleLoader.GetProvider();
            IRandomGenerator random = crypto.CreateRandomGenerator();

            AsymmetricPrivateKey keypair = crypto.CreateAsymmetricKey(CommonCryptographyAlgorithms.Rsa2048);
            ReadOnlyMemory<byte> pubkey = crypto.EncodePkcsPublicKey(keypair.GetPublicKey());
            ReadOnlyMemory<byte> fakeseckey = random.NextBytes(432);

            DatabaseUser user = new DatabaseUser
            {
                Id = UserId.Create(),
                Salt = random.NextBytes(8),
                PrivateKey = new SecretPayload(fakeseckey),
                ProtectedData = new DatabaseUserProtectedDataSigned
                {
                    PublicKey = pubkey,
                    Metadata = random.NextBytes(34),
                    Signature = random.NextBytes(239),
                },
            };

            using (IDatabase3.ITransaction transaction = database.BeginTransaction())
            {
                transaction.CreateUser(user);
                DatabaseUser decoded = transaction.OpenUser(user.Id);

                CollectionAssert.AreEqual(user.Salt.ToArray(), decoded.Salt.ToArray());
                CollectionAssert.AreEqual(user.PrivateKey.EncryptedPayload.ToArray(), decoded.PrivateKey.EncryptedPayload.ToArray());
                CollectionAssert.AreEqual(user.ProtectedData.PublicKey.ToArray(), decoded.ProtectedData.PublicKey.ToArray());
                CollectionAssert.AreEqual(user.ProtectedData.Signature.ToArray(), decoded.ProtectedData.Signature.ToArray());
                CollectionAssert.AreEqual(user.ProtectedData.Metadata.ToArray(), decoded.ProtectedData.Metadata.ToArray());

                transaction.CreateUser(user with { Id = UserId.Create() });
                transaction.CreateUser(user with { Id = UserId.Create() });

                transaction.Commit();
            }
            DatabaseUser[] users = database.CreateSnapshot().EnumerateUsers().ToArray();
            ClassicAssert.AreEqual(3, users.Length);
        }

        [Test]
        public void UsersAndEntriesSameFile()
        {
            ICryptographyProvider crypto = BouncyCastleLoader.GetProvider();
            IRandomGenerator random = crypto.CreateRandomGenerator();

            DatabaseEntry entry = new DatabaseEntry
            {
                Id = EntryId.Create(),
                Data = random.NextBytes(42),
                Salt = random.NextBytes(42),
                Keys = new Dictionary<UserId, ReadOnlyMemory<byte>>(),
            };

            AsymmetricPrivateKey keypair = crypto.CreateAsymmetricKey(CommonCryptographyAlgorithms.Rsa2048);
            ReadOnlyMemory<byte> pubkey = crypto.EncodePkcsPublicKey(keypair.GetPublicKey());
            ReadOnlyMemory<byte> fakeseckey = random.NextBytes(432);

            DatabaseUser user = new DatabaseUser
            {
                Id = UserId.Create(),
                Salt = random.NextBytes(8),
                PrivateKey = new SecretPayload(fakeseckey),
                ProtectedData = new DatabaseUserProtectedDataSigned
                {
                    PublicKey = pubkey,
                    Metadata = random.NextBytes(34),
                    Signature = random.NextBytes(239),
                },
            };

            using (IDatabase3.ITransaction transaction = database.BeginTransaction())
            {
                transaction.CreateEntry(entry with { Id = EntryId.Create() });
                transaction.CreateUser(user with { Id = UserId.Create() });

                ClassicAssert.AreEqual(1, transaction.EnumerateEntries().Count());
                ClassicAssert.AreEqual(1, transaction.EnumerateUsers().Count());

                transaction.CreateEntry(entry with { Id = EntryId.Create() });
                transaction.CreateEntry(entry with { Id = EntryId.Create() });
                transaction.CreateEntry(entry with { Id = EntryId.Create() });
                transaction.CreateUser(user with { Id = UserId.Create() });
                transaction.CreateUser(user with { Id = UserId.Create() });

                transaction.Commit();
            }

            ClassicAssert.AreEqual(4, database.CreateSnapshot().EnumerateEntries().Count());
            ClassicAssert.AreEqual(3, database.CreateSnapshot().EnumerateUsers().Count());
        }
    }
}
