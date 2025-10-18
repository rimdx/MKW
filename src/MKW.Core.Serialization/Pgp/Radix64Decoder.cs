using System.Security.Cryptography;

namespace MKW.Core.Serialization.Pgp
{
    public sealed class Radix64Decoder : ICryptoTransform
    {
        public bool CanReuseTransform => true;
        public bool CanTransformMultipleBlocks => false;

        public int InputBlockSize => 4;
        public int OutputBlockSize => 3;

        public int TransformBlock(byte[] inputBuffer,
                                  int inputOffset,
                                  int inputCount,
                                  byte[] outputBuffer,
                                  int outputOffset)
        {
            ReadOnlySpan<byte> inputSpan = new ReadOnlySpan<byte>(inputBuffer, inputOffset, inputCount);
            Span<byte> outputSpan = new Span<byte>(outputBuffer, outputOffset, OutputBlockSize);

            return Radix64BitConvert.DecodeBlock(inputSpan, outputSpan);
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer,
                                          int inputOffset,
                                          int inputCount)
        {
            return [];
        }

        public void Dispose()
        {
        }
    }
}
