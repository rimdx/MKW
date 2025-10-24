// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.Cryptography;
using MKW.Cryptography.Loader;
using MKW.Storage.MKPG;
using MKW.Storage.MKPG.BlobStore;
using MKW.Storage.MKPG.FileSystem;
using NUnit.Framework.Legacy;

namespace MKW.Storage.Tests
{
    public class Database3Tests
    {
        [Test]
        public void SimpleTest()
        {
            MKPGSerializer serializer = new MKPGSerializer();
            using MemoryEditorFactory editor = new MemoryEditorFactory();
            using DatabaseBlobStorageSingleFile store = new DatabaseBlobStorageSingleFile(editor);
            MDatabase database = new MDatabase(serializer, store);

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
