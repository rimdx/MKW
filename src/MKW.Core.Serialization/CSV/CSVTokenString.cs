// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization.CSV
{
    public sealed record class CSVTokenString : CSVToken
    {
        public CSVTokenString(string data) : base(data)
        {
        }
    }
}
