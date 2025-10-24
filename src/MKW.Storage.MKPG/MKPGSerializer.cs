// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using System.Buffers;

namespace MKW.Storage.MKPG
{
    public sealed class MKPGSerializer
        : IDatabaseSerializer
    {
        public MKPGSerializer()
        {
        }

        // Entry
        public DatabaseEntry DeserializeEntry(BlobSecretEntry blob)
        {
            return EntrySerializer.Deserialize(blob.CreateReader(),
                                               EntryId.FromBytes(blob.Id.GetBytes().Span));
        }

        public BlobSecretEntry SerializeEntry(DatabaseEntry entry)
        {
            ArrayBufferWriter<byte> writer = new ArrayBufferWriter<byte>();
            EntrySerializer.Serialize(writer, entry);

            return new BlobSecretEntry
            {
                Id = BlobId.From(entry.Id),
                Data = writer.WrittenMemory,
            };
        }

        // User
        public DatabaseUser DeserializeUser(BlobUser blob)
        {
            return UserSerializer.Deserialize(blob.CreateReader(),
                                              UserId.FromBytes(blob.Id.GetBytes()));
        }

        public BlobUser SerializeUser(DatabaseUser user)
        {
            ArrayBufferWriter<byte> writer = new ArrayBufferWriter<byte>();
            UserSerializer.Serialize(writer, user);

            return new BlobUser
            {
                Id = BlobId.From(user.Id),
                Data = writer.WrittenMemory,
            };
        }

        // Payloads
        public ReadOnlyMemory<byte> SerializeProtectedData(DatabaseUserProtectedData obj)
        {
            throw new NotImplementedException();
        }
    }
}
