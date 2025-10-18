// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization.Asn1
{
    internal sealed class Asn1VersionMismatch(int expected, int actual)
        : Exception($"Asn1 version mismatch: expected {expected} but was {actual}.")
    {
    }
}
