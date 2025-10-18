// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Common
{
    public interface IBufferReader
    {
        int RemainingBytes { get; }

        byte ReadByte();

        ReadOnlyMemory<byte> ReadBytes(int count);
        ReadOnlyMemory<byte> ReadAll();
    }
}
