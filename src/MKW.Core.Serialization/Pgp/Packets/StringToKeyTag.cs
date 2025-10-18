// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization.Pgp.Packets
{
    public enum StringToKeyTag : byte
    {
        Simple = 0,
        Salted = 1,
        IteratedSalted = 3,
    }
}
