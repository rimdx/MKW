using System.Collections.ObjectModel;

namespace MKW.GUI
{
    public class RecentFilesCollectionViewModel : ObservableCollection<RecentFileItemViewModel>, IDisposable
    {
        private readonly RecentFilesService recentFilesService;

        public RecentFilesCollectionViewModel(RecentFilesService recentFilesService)
        {
            this.recentFilesService = recentFilesService;
            this.recentFilesService.RecentFilesChanged += RecentFilesChanged;
            RefreshRecentFiles();
        }

        private void RecentFilesChanged(object? sender, EventArgs e)
        {
            RefreshRecentFiles();
        }

        private void RefreshRecentFiles()
        {
            Clear();

            foreach (string file in recentFilesService.EnumerateRecentFiles())
            {
                Add(new RecentFileItemViewModel(file));
            }
        }

        public void Dispose()
        {
            recentFilesService.RecentFilesChanged -= RecentFilesChanged;
        }
    }
}
