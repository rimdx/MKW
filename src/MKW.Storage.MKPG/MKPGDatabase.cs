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
        public Blob OpenEntry2(BlobId id)
        {
            return entries.Open(BlobId.From(id));
        }

        public void CreateEntry2(Blob entry)
        {
            entries.Create(new PgpBlobEntry
            {
                Id = entry.Id,
                Data = entry.Data,
                Type = MKPGConstants.ArmourTypeHeaders.Entry,
            });
        }

        public void UpdateEntry2(Blob entry)
        {
            entries.Update(new PgpBlobEntry
            {
                Id = entry.Id,
                Data = entry.Data,
                Type = MKPGConstants.ArmourTypeHeaders.Entry,
            });
        }

        public IEnumerable<Blob> EnumerateEntries2()
        {
            foreach (PgpBlobEntry blob in entries.Enumerate())
            {
                yield return blob;
            }
        }

        public Blob SerializeEntry2(DatabaseEntry entry)
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

        public DatabaseEntry DeserializeEntry2(Blob blob)
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
        public Blob OpenUser2(BlobId id)
        {
            return users.Open(BlobId.From(id));
        }

        public void CreateUser2(Blob blob)
        {
            users.Create(new PgpBlobEntry
            {
                Id = blob.Id,
                Data = blob.Data,
                Type = MKPGConstants.ArmourTypeHeaders.User,
            });
        }

        public void UpdateUser2(Blob blob)
        {
            users.Update(new PgpBlobEntry
            {
                Id = blob.Id,
                Data = blob.Data,
                Type = MKPGConstants.ArmourTypeHeaders.User,
            });
        }

        public IEnumerable<Blob> EnumerateUsers2()
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
        public Blob SerializeUser2(DatabaseUser user)
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

        public DatabaseUser DeserializeUser2(Blob blob)
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
        public ReadOnlyMemory<byte> SerializeProtectedData(DatabaseUserProtectedData obj)
        {
            throw new NotImplementedException();
        }

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
