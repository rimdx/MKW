// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization.OpenPgp.MDP
{
    public sealed class OpenPgpModificationDetectionPacketCorruptedException(string reason)
        : Exception($"Modification detection packet corrupted: {reason}");
}
