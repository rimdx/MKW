namespace MKW.Common
{
    public class TempFile : Stream
    {
        private bool owns = true;
        private readonly string tempPath;
        private readonly string newPath;

        private readonly FileStream proxy;

        private TempFile(FileStream proxy, string tempPath, string newPath)
        {
            this.proxy = proxy;
            this.tempPath = tempPath;
            this.newPath = newPath;
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

        public override void Write(byte[] buffer, int offset, int count)
        {
            proxy.Write(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return proxy.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            proxy.SetLength(value);
        }

        private static string GenerateTempFileName(string path)
        {
            if (File.Exists(path))
            {
                // TODO: properly generate file name
                return path + ".tmp";
            }
            else
            {
                return path;
            }
        }

        public static TempFile Create(string path)
        {
            string tempPath = GenerateTempFileName(path);

            FileStream file = new FileStream(tempPath,
                                             FileMode.CreateNew,
                                             FileAccess.Write);

            try
            {
                return new TempFile(file, tempPath, path);
            }
            catch (Exception)
            {
                file.Dispose();
                throw;
            }
        }

        public void Accept()
        {
            if (owns)
            {
                proxy.Flush(true);
                proxy.Close();

                if (tempPath != newPath)
                {
                    File.Replace(tempPath, newPath, null);
                }

                owns = false;
            }
        }

        public void Reject()
        {
            if (owns)
            {
                proxy.Close();
                File.Delete(tempPath);
                owns = false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            Reject();
        }
    }
}
