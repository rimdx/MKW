// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization.CSV
{
    public sealed record class CSVTokenNewLine : CSVToken
    {
        internal static CSVTokenNewLine Instance = new CSVTokenNewLine();

        public CSVTokenNewLine() : base("\n")
        {
        }
    }
}
