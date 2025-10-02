using System.Security.Cryptography;
using System.Text;

namespace MKW.Common
{
    public sealed class LineBreakTransform : ICryptoTransform, IDisposable
    {
        private readonly ReadOnlyMemory<byte> lineSeparator;
        private readonly int lineLength;

        public bool CanReuseTransform => true;
        public bool CanTransformMultipleBlocks => false;

        public int InputBlockSize => lineLength;
        public int OutputBlockSize => lineLength + lineSeparator.Length;

        public LineBreakTransform(int lineLength, string lineSeparator = "\n")
        {
            if (lineLength < 1)
            {
                throw new ArgumentException("Line length cannot be less than 1.", nameof(lineLength));
            }

            this.lineLength = lineLength;
            this.lineSeparator = Encoding.ASCII.GetBytes(lineSeparator);
        }

        private int Process(ReadOnlySpan<byte> inputSpan, Span<byte> outputSpan)
        {
            inputSpan.CopyTo(outputSpan);
            lineSeparator.Span.CopyTo(outputSpan.Slice(inputSpan.Length));

            return inputSpan.Length + lineSeparator.Length;
        }

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount,
                                  byte[] outputBuffer, int outputOffset)
        {
            ReadOnlySpan<byte> inputSpan = new ReadOnlySpan<byte>(inputBuffer, inputOffset, inputCount);
            Span<byte> outputSpan = new Span<byte>(outputBuffer, outputOffset, OutputBlockSize);

            return Process(inputSpan, outputSpan);
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            ReadOnlySpan<byte> inputSpan = new ReadOnlySpan<byte>(inputBuffer, inputOffset, inputCount);
            byte[] outputBuffer = new byte[inputSpan.Length + lineSeparator.Length];

            _ = Process(inputSpan, outputBuffer);

            return outputBuffer;
        }

        public void Dispose()
        {
            /* no-op */
        }
    }
}
