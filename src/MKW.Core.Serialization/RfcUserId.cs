// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization
{
    public sealed record class RfcUserId
    {
        public required string Name { get; init; }

        public string? Email { get; init; }
        public string? Comment { get; init; }
    }
}
