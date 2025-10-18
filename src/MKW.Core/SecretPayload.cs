// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core
{
    public record class SecretPayload(
        ReadOnlyMemory<byte> EncryptedPayload
    );
}
