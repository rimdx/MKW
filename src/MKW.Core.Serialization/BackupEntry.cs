// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization
{
    public sealed record class BackupEntry
    {
        public IReadOnlyDictionary<string, string> Fields { get; }

        public BackupEntry(IReadOnlyDictionary<string, string> fields)
        {
            Fields = fields;
        }
    }
}
