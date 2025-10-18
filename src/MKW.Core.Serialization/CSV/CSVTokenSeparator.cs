// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization.CSV
{
    public sealed record class CSVTokenSeparator : CSVToken
    {
        internal static CSVTokenSeparator Instance = new CSVTokenSeparator();

        public CSVTokenSeparator() : base(",")
        {
        }
    }
}
