// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Common
{
    public sealed class ArrayBufferReader<T> : IBufferReader<T>
    {
        private ReadOnlyMemory<T> Data;

        public int RemainingBytes => Data.Length;

        public ArrayBufferReader(ReadOnlyMemory<T> data)
        {
            Data = data;
        }

        private void Advance(int count)
        {
            Data = Data.Slice(count);
        }

        public ReadOnlyMemory<T> ReadBytes(int count)
        {
            ReadOnlyMemory<T> slice = Data.Slice(0, count);
            Advance(count);
            return slice;
        }

        public ReadOnlyMemory<T> ReadAll()
        {
            ReadOnlyMemory<T> slice = Data;
            Advance(slice.Length);
            return slice;
        }

        public T ReadByte()
        {
            T b = Data.Span[0];
            Advance(1);
            return b;
        }
    }
}
