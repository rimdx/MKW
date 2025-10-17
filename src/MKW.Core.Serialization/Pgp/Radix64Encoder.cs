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

        public int TransformBlock(byte[] inputBuffer,
                                  int inputOffset,
                                  int inputCount,
                                  byte[] outputBuffer,
                                  int outputOffset)
        {
            ReadOnlySpan<byte> inputSpan = new ReadOnlySpan<byte>(inputBuffer, inputOffset, inputCount);
            Span<byte> outputSpan = new Span<byte>(outputBuffer, outputOffset, OutputBlockSize);

            Radix64BitConvert.EncodeFullBlock(inputSpan, outputSpan);

            return outputBuffer.Length;
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer,
                                          int inputOffset,
                                          int inputCount)
        {
            ReadOnlySpan<byte> inputSpan = new ReadOnlySpan<byte>(inputBuffer, inputOffset, inputCount);
            byte[] outputBuffer = new byte[4];

            Radix64BitConvert.EncodeFinalBlock(inputSpan, outputBuffer);

            return outputBuffer;
        }

        public void Dispose()
        {
        }
    }
}
