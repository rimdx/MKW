// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization.OpenPgp.Packets
{
    public abstract record class StringToKey
    {
        public required StringToKeyTag Tag { get; init; }
    }
}
