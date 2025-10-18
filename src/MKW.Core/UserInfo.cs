// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core
{
    public record class UserInfo
    {
        public required UserId Id { get; init; }
        public required ReadOnlyMemory<byte> PublicKey { get; init; }

        public required UserMetadata Metadata { get; init; }

        public Trust Trust { get; set; } = Trust.Unknown;

        public UserInfo()
        {
        }
    }
}
