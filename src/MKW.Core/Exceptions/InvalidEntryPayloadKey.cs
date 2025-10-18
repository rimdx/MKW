// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Exceptions
{
    public sealed class InvalidEntryPayloadKey(string reason)
        : Exception($"Invalid entry payload key: {reason}")
    {
        public string Reason => reason;
    }
}
