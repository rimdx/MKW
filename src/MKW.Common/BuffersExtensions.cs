// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Buffers;

namespace MKW.Common
{
    public static class BuffersExtensions
    {
        public static void Write<T>(this IBufferWriter<T> writer,
                                    T value)
        {
            Span<T> span = [
                value,
            ];

            writer.Write(span);
        }
    }
}
