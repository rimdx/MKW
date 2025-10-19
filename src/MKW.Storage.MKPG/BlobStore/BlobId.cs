// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;

namespace MKW.Storage.MKPG.BlobStore
{
    internal sealed class BlobId : IdBase
    {
        private BlobId(ReadOnlyMemory<byte> data)
            : base(data.ToArray(), data.Length)
        {
        }

        public static BlobId From(Guid id)
        {
            return new BlobId(id.ToByteArray());
        }

        public static BlobId From(ReadOnlyMemory<byte> data)
        {
            return new BlobId(data);
        }

        public static BlobId From(IdBase id)
        {
            return new BlobId(id.GetBytes());
        }
    }
}
