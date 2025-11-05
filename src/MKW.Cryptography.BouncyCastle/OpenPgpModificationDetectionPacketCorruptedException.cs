// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Cryptography.BouncyCastle
{
    internal sealed class OpenPgpModificationDetectionPacketCorruptedException(string reason)
        : Exception($"Modification detection packet corrupted: {reason}");
}
