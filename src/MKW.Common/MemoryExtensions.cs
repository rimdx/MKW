// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Common
{
    public static class MemoryExtensions
    {
        public static ReadOnlySpan<T> EnsureSize<T>(this ReadOnlySpan<T> span, int expected)
        {
            if (span.Length != expected)
            {
                throw new ArgumentException($"Expected array length: {expected}");
            }

            return span;
        }

        public static Span<T> EnsureSize<T>(this Span<T> span, int expected)
        {
            if (span.Length != expected)
            {
                throw new ArgumentException($"Expected array length: {expected}");
            }

            return span;
        }

        public static Memory<T> EnsureSize<T>(this Memory<T> memory, int expected)
        {
            if (memory.Length != expected)
            {
                throw new ArgumentException($"Expected array length: {expected}");
            }

            return memory;
        }

        public static ReadOnlyMemory<T> EnsureSize<T>(this ReadOnlyMemory<T> memory, int expected)
        {
            if (memory.Length != expected)
            {
                throw new ArgumentException($"Expected array length: {expected}");
            }

            return memory;
        }
    }
}
