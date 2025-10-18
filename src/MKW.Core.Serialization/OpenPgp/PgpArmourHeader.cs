// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization.OpenPgp
{
    public sealed record class PgpArmourHeader(
        string Key,
        string Value
    );
}
