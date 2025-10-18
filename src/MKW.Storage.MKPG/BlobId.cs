// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;

namespace MKW.Storage.MKPG
{
    internal sealed class BlobId : IdBase
    {
        private BlobId(ReadOnlyMemory<byte> data)
            : base(data.ToArray(), data.Length)
        {
        }

        public override string ToString()
        {
            return data.ToString();
        }

        public static BlobId From(Guid id)
        {
            return new BlobId(id.ToByteArray());
        }

        public static BlobId From(ReadOnlyMemory<byte> data)
        {
            return new BlobId(data);
        }

        public static BlobId From(EntryId entryId)
        {
            return new BlobId(entryId.GetBytes());
        }

        public static BlobId From(UserId userId)
        {
            return new BlobId(userId.GetGuid().ToByteArray());
        }
    }
}
