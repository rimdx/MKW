namespace MKW.GUI
{
    public class RecentFilesService
    {
        private readonly RegistryService registryService;
        private readonly List<string> recentFiles;

        public int MaxCount { get; }

        public event EventHandler? RecentFilesChanged;

        public RecentFilesService(RegistryService registryService)
        {
            this.registryService = registryService;
            MaxCount = 5;

            recentFiles = [.. registryService.GetRecentFiles()];
        }

        public void OnFileOpened(string filename)
        {
            recentFiles.Remove(filename);
            recentFiles.Insert(0, filename);

            if (recentFiles.Count > MaxCount)
            {
                recentFiles.RemoveRange(MaxCount, recentFiles.Count - MaxCount);
            }

            registryService.SetRecentFiles([.. recentFiles]);

            RecentFilesChanged?.Invoke(this, new EventArgs());
        }

        public IEnumerable<string> EnumerateRecentFiles()
        {
            return recentFiles;
        }
    }
}
