// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization
{
    internal sealed class Asn1BadSequenceLengthException(int count)
        : Exception($"Bad sequence length: {count}")
    {
    }
}
