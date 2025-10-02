using System.Text;

namespace MKW.Common
{
    public sealed class ASCIIStream : Stream
    {
        private readonly TextWriter writer;

        public ASCIIStream(TextWriter writer)
        {
            this.writer = writer;
        }

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Flush()
        {
            writer.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            string str = Encoding.ASCII.GetString(buffer, offset, count);
            writer.Write(str);
        }

        protected override void Dispose(bool disposing)
        {
            // TODO: close writer
        }
    }
}
