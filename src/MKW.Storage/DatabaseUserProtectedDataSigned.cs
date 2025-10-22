// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Storage
{
    public record class DatabaseUserProtectedDataSigned : DatabaseUserProtectedData
    {
        public ReadOnlyMemory<byte> Signature { get; init; }
    }
}
