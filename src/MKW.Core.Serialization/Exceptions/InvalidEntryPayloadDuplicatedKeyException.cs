// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization.Exceptions
{
    internal sealed class InvalidEntryPayloadDuplicatedKeyException(EntryPayloadKey key)
        : InvalidEntryPayload($"Duplicated key '{key}'.")
    {
    }
}
