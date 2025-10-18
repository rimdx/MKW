// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Common
{
    public interface IBufferReader<T>
    {
        int RemainingBytes { get; }

        T ReadByte();

        ReadOnlyMemory<T> ReadBytes(int count);
        ReadOnlyMemory<T> ReadAll();
    }
}
