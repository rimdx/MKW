namespace MKW.Core.Serialization.Pgp
{
    internal sealed class PartialInputStream : Stream
    {
        private readonly Stream proxy;
        private uint dataLength;

        public PartialInputStream(Stream proxy, uint dataLength)
        {
            this.proxy = proxy;
            this.dataLength = dataLength;
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;

        public override long Length => throw new NotImplementedException();
        public override long Position
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override void Flush()
        {
            proxy.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (dataLength > 0U)
            {
                int readLen = Math.Min(count, (int)dataLength);
                int len = proxy.Read(buffer, offset, readLen);

                if (len < 1)
                {
                    throw new EndOfStreamException("Premature end of stream in PartialInputStream");
                }
                else
                {
                    dataLength -= (uint)len;
                    return len;
                }
            }
            else
            {
                return 0;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
