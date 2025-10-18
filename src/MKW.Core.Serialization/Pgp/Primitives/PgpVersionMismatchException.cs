// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization.Pgp.Primitives
{
    public sealed class PgpVersionMismatchException(int expected, int actual)
        : Exception($"PGP version mismatch: expected {expected} but was {actual}.")
    {
    }
}
