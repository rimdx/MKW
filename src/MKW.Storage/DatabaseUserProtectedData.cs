// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage
{
    public record class DatabaseUserProtectedData
    {
        public ReadOnlyMemory<byte> PublicKey { get; init; }
        public ReadOnlyMemory<byte> Metadata { get; init; }
    }
}
