// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;

namespace MKW.OpenPgp
{
    public sealed class PgpKeyId : IdBase
    {
        private const int size = 16;

        private PgpKeyId(byte[] data) : base(data, size)
        {
        }

        public static PgpKeyId FromBytes(ReadOnlyMemory<byte> data)
        {
            return new PgpKeyId(data.ToArray());
        }
    }
}
