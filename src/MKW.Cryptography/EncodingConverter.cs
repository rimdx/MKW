// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Text;

namespace MKW.Cryptography
{
    public static class EncodingConverter
    {
        public static ReadOnlyMemory<byte> GetBytes(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

        public static string GetString(ReadOnlySpan<byte> data)
        {
            return Encoding.UTF8.GetString(data.ToArray());
        }
    }
}
