// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core
{
    public record class UserMetadata
    {
        public required string UserId { get; init; }
        public required string DisplayName { get; init; }
    }
}
