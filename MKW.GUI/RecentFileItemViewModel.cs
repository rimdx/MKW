using System.IO;

namespace MKW.GUI
{
    public class RecentFileItemViewModel
    {
        public string FullPath { get; }
        public string FileName => Path.GetFileName(FullPath);

        public string FileNameWithFullPath => $"{FileName} ({FullPath})";

        public RecentFileItemViewModel(string fullPath)
        {
            FullPath = fullPath;
        }
    }
}
