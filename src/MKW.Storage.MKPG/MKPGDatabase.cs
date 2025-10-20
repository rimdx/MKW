// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.Cryptography;
using MKW.Storage.MKPG.BlobStore;
using MKW.Storage.MKPG.FileSystem;
using System.Buffers;

namespace MKW.Storage.MKPG
{
    public sealed partial class MKPGDatabase : IDatabase, IDatabase2, IDisposable
    {
        private readonly IBlobStorage database;
        private readonly IBlobStorage entries;
        private readonly IBlobStorage users;

        public MKPGDatabase()
            : this(new BlobStorageSingleFile(new MemoryEditorFactory()))
        {
        }

        private MKPGDatabase(IBlobStorage database)
        {
            this.database = database;

            entries = new BlobStorageFiltered(database, MKPGConstants.ArmourTypeHeaders.Entry);
            users = new BlobStorageFiltered(database, MKPGConstants.ArmourTypeHeaders.User);
        }

        public static MKPGDatabase Open(string path)
        {
            FileSystemEditorFactory file = new FileSystemEditorFactory(path);
            BlobStorageSingleFile store = new BlobStorageSingleFile(file);
            return new MKPGDatabase(store);
        }

        public static MKPGDatabase Create(string path)
        {
            using (FileStream file = File.Create(path))
            {
            }

            return Open(path);
        }

        // Entry
        public BlobEntry OpenEntry2(BlobId id)
        {
            return entries.Open(BlobId.From(id));
        }

        public void CreateEntry2(BlobEntry entry)
        {
            entries.Create(new PgpBlobEntry
            {
                Id = entry.Id,
                Data = entry.Data,
                Type = MKPGConstants.ArmourTypeHeaders.Entry,
            });
        }

        public void UpdateEntry2(BlobEntry entry)
        {
            entries.Update(new PgpBlobEntry
            {
                Id = entry.Id,
                Data = entry.Data,
                Type = MKPGConstants.ArmourTypeHeaders.Entry,
            });
        }

        public IEnumerable<BlobEntry> EnumerateEntries2()
        {
            foreach (PgpBlobEntry blob in entries.Enumerate())
            {
                yield return blob;
            }
        }

        public BlobEntry SerializeEntry(DatabaseEntry entry)
        {
            ArrayBufferWriter<byte> writer = new ArrayBufferWriter<byte>();
            EntrySerializer.Serialize(writer, entry);

            return new PgpBlobEntry
            {
                Id = BlobId.From(entry.Id),
                Type = MKPGConstants.ArmourTypeHeaders.Entry,
                Data = writer.WrittenMemory,
            };
        }

        public DatabaseEntry DeserializeEntry(BlobEntry blob)
        {
            return EntrySerializer.Deserialize(blob.CreateReader(),
                                               EntryId.FromBytes(blob.Id.GetBytes().Span));
        }

        public bool DeleteEntry(EntryId id)
        {
            throw new NotImplementedException();
        }

        public bool HasEntry(EntryId id)
        {
            throw new NotImplementedException();
        }

        // User
        public BlobEntry OpenUser2(BlobId id)
        {
            return users.Open(BlobId.From(id));
        }

        public void CreateUser2(BlobEntry blob)
        {
            users.Create(new PgpBlobEntry
            {
                Id = blob.Id,
                Data = blob.Data,
                Type = MKPGConstants.ArmourTypeHeaders.User,
            });
        }

        public void UpdateUser2(BlobEntry blob)
        {
            users.Update(new PgpBlobEntry
            {
                Id = blob.Id,
                Data = blob.Data,
                Type = MKPGConstants.ArmourTypeHeaders.User,
            });
        }

        public IEnumerable<BlobEntry> EnumerateUsers2()
        {
            foreach (PgpBlobEntry blob in users.Enumerate())
            {
                yield return blob;
            }
        }

        public bool DeleteUser(UserId id)
        {
            return users.Delete(BlobId.From(id));
        }

        public bool HasUser(UserId id)
        {
            return users.Exists(BlobId.From(id));
        }

        // User serialize/deserialize
        public BlobEntry SerializeUser(DatabaseUser user)
        {
            ArrayBufferWriter<byte> writer = new ArrayBufferWriter<byte>();
            UserSerializer.Serialize(writer, user);

            return new PgpBlobEntry
            {
                Id = BlobId.From(user.Id),
                Type = MKPGConstants.ArmourTypeHeaders.User,
                Data = writer.WrittenMemory,
            };
        }

        public DatabaseUser DeserializeUser(BlobEntry blob)
        {
            return UserSerializer.Deserialize(blob.CreateReader(), UserId.FromBytes(blob.Id.GetBytes()));
        }

        // Trust Signatures
        public void AddTrustSignature(DatabaseTrustSignature signature)
        {
            throw new NotImplementedException();
        }

        public void DeleteTrustSignature(DatabaseTrustSignature signature)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DatabaseTrustSignature> EnumerateTrustSignatures()
        {
            throw new NotImplementedException();
        }

        // Misc
        public DatabaseConfiguration GetConfiguration()
        {
            return new DatabaseConfiguration
            {
                PreferredSymmetricAlgorithm = CommonCryptographyAlgorithms.Aes128OpenPgpCfb,
                PreferredPublicKeyAlgorithm = CommonCryptographyAlgorithms.Rsa2048,
                PreferredStringToKeyAlgorithm = CommonCryptographyAlgorithms.OpenPgpStringToKey,
            };
        }

        public void ReloadDatabaseFile()
        {
            throw new NotImplementedException();
        }

        public Task<bool> WaitForDatabaseChangesAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
