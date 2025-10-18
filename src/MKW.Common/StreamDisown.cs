// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.Common
{
    /// <summary>
    /// Similar to Subversion's svn_stream_disown().
    /// </summary>
    public class StreamDisown : Stream
    {
        private readonly Stream proxy;

        public StreamDisown(Stream proxy)
        {
            this.proxy = proxy;
        }

        public override bool CanRead => proxy.CanRead;
        public override bool CanSeek => proxy.CanSeek;
        public override bool CanWrite => proxy.CanWrite;
        public override long Length => proxy.Length;

        public override long Position
        {
            get => proxy.Position;
            set => proxy.Position = value;
        }

        public override void Flush()
        {
            proxy.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return proxy.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return proxy.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            proxy.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            proxy.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            /* no-op */
        }
    }
}
