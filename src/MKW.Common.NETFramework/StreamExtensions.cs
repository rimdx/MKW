// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Buffers;
using System.Runtime.InteropServices;

namespace MKW.Common
{
    public static class StreamExtensions
    {
        public static int Read(this Stream stream, Span<byte> buffer)
        {
            byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);

            try
            {
                int numRead = stream.Read(sharedBuffer, 0, buffer.Length);

                Span<byte> span = new Span<byte>(sharedBuffer);
                span.CopyTo(buffer);

                return span.Length;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(sharedBuffer);
            }
        }

        public static void Write(this Stream stream, ReadOnlySpan<byte> buffer)
        {
            byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);

            try
            {
                buffer.CopyTo(sharedBuffer);
                stream.Write(sharedBuffer, 0, buffer.Length);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(sharedBuffer);
            }
        }

        public static async Task WriteAsync(this Stream stream,
                                            ReadOnlyMemory<byte> buffer,
                                            CancellationToken cancellationToken = default)
        {
            if (MemoryMarshal.TryGetArray(buffer, out ArraySegment<byte> array))
            {
                await stream.WriteAsync(array.Array, array.Offset, array.Count, cancellationToken);
            }
            else
            {
                byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);

                try
                {
                    buffer.Span.CopyTo(sharedBuffer);
                    await stream.WriteAsync(sharedBuffer, 0, buffer.Length, cancellationToken);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(sharedBuffer);
                }
            }
        }

        public static async Task<int> ReadAsync(this Stream stream,
                                                Memory<byte> buffer,
                                                CancellationToken cancellationToken = default)
        {
            if (MemoryMarshal.TryGetArray(buffer, out ArraySegment<byte> array))
            {
                return await stream.ReadAsync(array.Array, array.Offset, array.Count, cancellationToken);
            }
            else
            {
                byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);

                try
                {
                    int numRead = await stream.ReadAsync(sharedBuffer, 0, buffer.Length, cancellationToken);

                    Span<byte> span = new Span<byte>(sharedBuffer);
                    span.CopyTo(buffer.Span);

                    return span.Length;
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(sharedBuffer);
                }
            }
        }
    }
}
