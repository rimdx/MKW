using Microsoft.Win32.SafeHandles;

namespace MKW.Common
{
    public class TempFile : FileStream
    {
        private bool owns = true;
        private readonly string tempPath;
        private readonly string newPath;

        private TempFile(SafeFileHandle handle, string tempPath, string newPath)
            : base(handle, FileAccess.Write)
        {
            this.tempPath = tempPath;
            this.newPath = newPath;
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
                return new TempFile(file.SafeFileHandle, tempPath, path);
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
                Flush(true);
                base.Dispose(true);

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
                base.Dispose(true);
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
