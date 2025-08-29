using System.Collections;
using System.Security.Cryptography;

namespace MKW.Core.Common
{
    public class Base32EncodeTransformer : ICryptoTransform
    {
        private readonly BitArray bits;

        public bool CanReuseTransform => true;
        public bool CanTransformMultipleBlocks => false;

        public int InputBlockSize => Base32BitConvert.InputChunkSize;
        public int OutputBlockSize => Base32BitConvert.OutputChunkSize;

        public Base32EncodeTransformer()
        {
            bits = new BitArray(InputBlockSize * OutputBlockSize);
        }

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount,
                                  byte[] outputBuffer, int outputOffset)
        {
            // convert raw args to spans
            ReadOnlySpan<byte> inputSpan = new ReadOnlySpan<byte>(inputBuffer, inputOffset, inputCount);
            Span<byte> outputSpan = new Span<byte>(outputBuffer, outputOffset, OutputBlockSize);

            return EncodeChunk(inputSpan, outputSpan);
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            ReadOnlySpan<byte> inputSpan = new ReadOnlySpan<byte>(inputBuffer, inputOffset, inputCount);

            if (inputCount == 0)
            {
                // base32 doesn't need final flush for empty buffer
                return [];
            }
            else
            {
                byte[] outputBuffer = new byte[OutputBlockSize];
                Span<byte> outputSpan = new Span<byte>(outputBuffer);

                int count = EncodeChunk(inputSpan, outputSpan);
                // fill the rest with padding
                Base32BitConvert.WritePadding(outputSpan.Slice(count));

                return outputBuffer;
            }
        }

        private int EncodeChunk(ReadOnlySpan<byte> inputSpan, Span<byte> outputSpan)
        {
            Base32BitConvert.BufferToBits(inputSpan, bits);

            int count = Base32BitConvert.EncodeChunk(bits, inputSpan.Length * 8, outputSpan);

            return count;
        }

        public void Dispose()
        {
            /* no-op */
        }
    }
}
