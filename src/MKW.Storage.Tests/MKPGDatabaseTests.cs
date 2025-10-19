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

            database.CreateEntry(dbEntry.Id, dbEntry);

            DatabaseEntry read = database.OpenEntry(dbEntry.Id);

            ClassicAssert.AreEqual(dbEntry.Id, read.Id);
            CollectionAssert.AreEqual(dbEntry.Data.ToArray(), read.Data.ToArray());
            CollectionAssert.AreEqual(dbEntry.Keys, read.Keys);

            database.CreateEntry(EntryId.Create(), dbEntry);
            database.CreateEntry(EntryId.Create(), dbEntry);
            database.CreateEntry(EntryId.Create(), dbEntry);

            DatabaseEntry[] entries = database.EnumerateEntries().ToArray();

            ClassicAssert.AreEqual(4, entries.Length);
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
                PublicKey = new SignedPayload(pubkey, new byte[32]),
                AdminSignature = null,
                Metadata = null,
            };

            using MKPGDatabase database = new MKPGDatabase();

            database.CreateUser(user.Id, user);
            DatabaseUser decoded = database.OpenUser(user.Id);

            CollectionAssert.AreEqual(user.Salt.ToArray(), decoded.Salt.ToArray());
            CollectionAssert.AreEqual(user.PrivateKey.EncryptedPayload.ToArray(), decoded.PrivateKey.EncryptedPayload.ToArray());
            CollectionAssert.AreEqual(user.PublicKey.Payload.ToArray(), decoded.PublicKey.Payload.ToArray());
            CollectionAssert.AreEqual(user.PublicKey.Signature.ToArray(), decoded.PublicKey.Signature.ToArray());

            database.CreateUser(UserId.Create(), user);
            database.CreateUser(UserId.Create(), user);

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
                PublicKey = new SignedPayload(pubkey, new byte[32]),
                AdminSignature = null,
                Metadata = null,
            };

            using MKPGDatabase database = new MKPGDatabase();

            database.CreateEntry(EntryId.Create(), entry);
            database.CreateUser(UserId.Create(), user);

            ClassicAssert.AreEqual(1, database.EnumerateEntries().Count());
            ClassicAssert.AreEqual(1, database.EnumerateUsers().Count());

            database.CreateEntry(EntryId.Create(), entry);
            database.CreateEntry(EntryId.Create(), entry);
            database.CreateEntry(EntryId.Create(), entry);
            database.CreateUser(UserId.Create(), user);
            database.CreateUser(UserId.Create(), user);

            ClassicAssert.AreEqual(4, database.EnumerateEntries().Count());
            ClassicAssert.AreEqual(3, database.EnumerateUsers().Count());
        }
    }
}
