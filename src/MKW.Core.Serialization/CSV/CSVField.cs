// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization.CSV
{
    public sealed record class CSVField
    {
        public string Value { get; }

        public CSVField(string value)
        {
            Value = value;
        }
    }
}
