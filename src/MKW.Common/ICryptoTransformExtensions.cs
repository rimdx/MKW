// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Buffers;
using System.Security.Cryptography;

namespace MKW.Common
{
    public static class ICryptoTransformExtensions
    {
        public static ReadOnlyMemory<byte> Transform(this ICryptoTransform transform, ReadOnlySpan<byte> input)
        {
            byte[]? inBuffer = null;
            byte[]? outBuffer = null;

            try
            {
                int inputBlockSize = transform.InputBlockSize;
                inBuffer = ArrayPool<byte>.Shared.Rent(inputBlockSize);

                int outputBlockSize = transform.OutputBlockSize;
                outBuffer = ArrayPool<byte>.Shared.Rent(transform.OutputBlockSize);

                ArrayBufferWriter<byte> writer = new ArrayBufferWriter<byte>();

                while (input.Length >= inputBlockSize)
                {
                    input.Slice(0, inputBlockSize).CopyTo(inBuffer.AsSpan());
                    input = input.Slice(inputBlockSize);

                    int written = transform.TransformBlock(inBuffer, 0, inputBlockSize, outBuffer, 0);
                    writer.Write(outBuffer.AsSpan().Slice(0, written));
                }

                input.CopyTo(inBuffer.AsSpan());
                byte[] final = transform.TransformFinalBlock(inBuffer, 0, input.Length);
                writer.Write(final.AsSpan());

                return writer.WrittenMemory;
            }
            finally
            {
                if (inBuffer != null)
                {
                    ArrayPool<byte>.Shared.Return(inBuffer, true);
                }

                if (outBuffer != null)
                {
                    ArrayPool<byte>.Shared.Return(outBuffer, true);
                }
            }
        }
    }
}
