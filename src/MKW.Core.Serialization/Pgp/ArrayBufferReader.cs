// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Core.Serialization.Pgp
{
    public sealed class ArrayBufferReader
    {
        private ReadOnlyMemory<byte> Data;

        public int RemainingBytes => Data.Length;

        public ArrayBufferReader(ReadOnlyMemory<byte> data)
        {
            Data = data;
        }

        private void Advance(int count)
        {
            Data = Data.Slice(count);
        }

        public ReadOnlyMemory<byte> ReadBytes(int count)
        {
            ReadOnlyMemory<byte> slice = Data.Slice(0, count);
            Advance(count);
            return slice;
        }

        public ReadOnlyMemory<byte> ReadAll()
        {
            ReadOnlyMemory<byte> slice = Data;
            Advance(slice.Length);
            return slice;
        }

        public byte ReadByte()
        {
            byte b = Data.Span[0];
            Advance(1);
            return b;
        }
    }
}
