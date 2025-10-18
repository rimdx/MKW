// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization.CSV
{
    public abstract record class CSVToken
    {
        public string Data { get; }

        protected CSVToken(string data)
        {
            Data = data;
        }
    }
}
