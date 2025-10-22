// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.Cryptography;
using MKW.Cryptography.Loader;
using MKW.Storage.MKPG;
using NUnit.Framework.Legacy;

namespace MKW.Storage.Tests
{
    public class MKPGDatabaseTests
    {
        [Test]
        public void SimpleEntryTest()
        {
            Random random = new Random(42);

            using MKPGDatabase database = new MKPGDatabase();

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

            database.CreateEntry(null, dbEntry);

            DatabaseEntry read = database.OpenEntry(dbEntry.Id);

            ClassicAssert.AreEqual(dbEntry.Id, read.Id);
            CollectionAssert.AreEqual(dbEntry.Data.ToArray(), read.Data.ToArray());
            CollectionAssert.AreEqual(dbEntry.Keys, read.Keys);

            database.CreateEntry(null, dbEntry with { Id = EntryId.Create() });
            database.CreateEntry(null, dbEntry with { Id = EntryId.Create() });
            database.CreateEntry(null, dbEntry with { Id = EntryId.Create() });

            DatabaseEntry[] entries = database.EnumerateEntries().ToArray();

            ClassicAssert.AreEqual(4, entries.Length);
        }

        [Test]
        public void SimpleUpdateEntryTest()
        {
            Random random = new Random(42);

            using MKPGDatabase database = new MKPGDatabase();

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

            database.CreateEntry(null, dbEntry with { Id = EntryId.Create() });
            database.CreateEntry(null, dbEntry with { Id = someid });
            database.CreateEntry(null, dbEntry with { Id = theid });
            database.CreateEntry(null, dbEntry with { Id = EntryId.Create() });

            database.UpdateEntry(null, dbEntry with
            {
                Id = theid,
                Data = data2,
            });

            CollectionAssert.AreEqual(data2, database.OpenEntry(theid).Data.ToArray());
            CollectionAssert.AreEqual(data, database.OpenEntry(someid).Data.ToArray());
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

            using MKPGDatabase database = new MKPGDatabase();

            database.CreateUser(null, user);
            DatabaseUser decoded = database.OpenUser(user.Id);

            CollectionAssert.AreEqual(user.Salt.ToArray(), decoded.Salt.ToArray());
            CollectionAssert.AreEqual(user.PrivateKey.EncryptedPayload.ToArray(), decoded.PrivateKey.EncryptedPayload.ToArray());
            CollectionAssert.AreEqual(user.ProtectedData.PublicKey.ToArray(), decoded.ProtectedData.PublicKey.ToArray());
            CollectionAssert.AreEqual(user.ProtectedData.Signature.ToArray(), decoded.ProtectedData.Signature.ToArray());
            CollectionAssert.AreEqual(user.ProtectedData.Metadata.ToArray(), decoded.ProtectedData.Metadata.ToArray());

            database.CreateUser(null, user with { Id = UserId.Create() });
            database.CreateUser(null, user with { Id = UserId.Create() });

            DatabaseUser[] users = database.EnumerateUsers().ToArray();
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

            using MKPGDatabase database = new MKPGDatabase();

            database.CreateEntry(null, entry with { Id = EntryId.Create() });
            database.CreateUser(null, user with { Id = UserId.Create() });

            ClassicAssert.AreEqual(1, database.EnumerateEntries().Count());
            ClassicAssert.AreEqual(1, database.EnumerateUsers().Count());

            database.CreateEntry(null, entry with { Id = EntryId.Create() });
            database.CreateEntry(null, entry with { Id = EntryId.Create() });
            database.CreateEntry(null, entry with { Id = EntryId.Create() });
            database.CreateUser(null, user with { Id = UserId.Create() });
            database.CreateUser(null, user with { Id = UserId.Create() });

            ClassicAssert.AreEqual(4, database.EnumerateEntries().Count());
            ClassicAssert.AreEqual(3, database.EnumerateUsers().Count());
        }
    }
}
