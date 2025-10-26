// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Common;

namespace MKW.Storage
{
    public static class BlobExtensions
    {
        public static IBufferReader<byte> CreateReader(this Blob blob)
        {
            return new ArrayBufferReader<byte>(blob.Data);
        }
    }
}
