namespace MKW.GUI
{
    public class RecentFileItemViewModel
    {
        public string FullPath { get; }

        public RecentFileItemViewModel(string fullPath)
        {
            FullPath = fullPath;
        }
    }
}
