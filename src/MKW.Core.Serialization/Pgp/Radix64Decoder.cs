using Org.BouncyCastle.Bcpg;
using System.Security.Cryptography;

namespace MKW.Core.Serialization.Pgp
{
    public sealed class Radix64Decoder : ICryptoTransform
    {
        private readonly Crc24 crc;

        public bool CanReuseTransform => false;
        public bool CanTransformMultipleBlocks => false;

        public int InputBlockSize => 4;
        public int OutputBlockSize => 3;

        public Radix64Decoder(Crc24 crc)
        {
            this.crc = crc;
        }

        public int TransformBlock(byte[] inputBuffer,
                                  int inputOffset,
                                  int inputCount,
                                  byte[] outputBuffer,
                                  int outputOffset)
        {
            ReadOnlySpan<byte> inputSpan = new ReadOnlySpan<byte>(inputBuffer, inputOffset, inputCount);
            Span<byte> outputSpan = new Span<byte>(outputBuffer, outputOffset, OutputBlockSize);

            int count = Radix64BitConvert.DecodeBlock(inputSpan, outputSpan);
            crc.Update(inputSpan.Slice(0, count));

            return count;
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
