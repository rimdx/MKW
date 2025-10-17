using System.Collections;
using System.Security.Cryptography;

namespace MKW.Core.Serialization.Pgp
{
    public sealed class Radix64Encoder : ICryptoTransform
    {
        public bool CanReuseTransform => false;
        public bool CanTransformMultipleBlocks => false;

        public int InputBlockSize => 3;
        public int OutputBlockSize => 4;

        private readonly BitArray bits;

        public Radix64Encoder()
        {
            bits = new BitArray(24);
        }

        public int TransformBlock(byte[] inputBuffer,
                                  int inputOffset,
                                  int inputCount,
                                  byte[] outputBuffer,
                                  int outputOffset)
        {
            ReadOnlySpan<byte> inputSpan = new ReadOnlySpan<byte>(inputBuffer, inputOffset, inputCount);
            Span<byte> outputSpan = new Span<byte>(outputBuffer, outputOffset, OutputBlockSize);

            Radix64BitConvert.BufferToBits(inputSpan, bits);
            Radix64BitConvert.EncodeChunk(bits, inputSpan.Length * 8, outputSpan);

            return outputBuffer.Length;
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer,
                                          int inputOffset,
                                          int inputCount)
        {
            ReadOnlySpan<byte> inputSpan = new ReadOnlySpan<byte>(inputBuffer, inputOffset, inputCount);

            byte[] outputBuffer = new byte[OutputBlockSize * 2 + 1];
            Span<byte> outputSpan = new Span<byte>(outputBuffer);

            Radix64BitConvert.BufferToBits(inputSpan, bits);
            int count = Radix64BitConvert.EncodeChunk(bits, inputSpan.Length * 8, outputSpan);

            Radix64BitConvert.WritePadding(outputSpan.Slice(count));

            return outputBuffer;
        }

        public void Dispose()
        {
        }
    }
}
