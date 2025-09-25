using MKW.GUI.Database;
using MKW.GUI.Images;
using MKW.GUI.Model;
using System.ComponentModel;
using System.IO;
using System.Windows.Controls;

namespace MKW.GUI
{
    public class DatabaseTabItemViewModel : ViewModelBase
    {
        private readonly DatabaseViewModel databaseViewModel;
        private ImageMoniker icon;

        public string Header { get; }
        public string Tooltip { get; }
        public ContentControl Content { get; }
        public ImageMoniker Icon
        {
            get => icon;
            private set => SetProperty(ref icon, value);
        }

        public DatabaseTabItemViewModel(DatabaseViewModel databaseViewModel)
        {
            this.databaseViewModel = databaseViewModel;

            Header = Path.GetFileNameWithoutExtension(databaseViewModel.Database.Path);
            Tooltip = databaseViewModel.Database.Path;
            Content = new DatabasePage(databaseViewModel);
            Icon = GetIcon(databaseViewModel.Database);

            databaseViewModel.Database.PropertyChanged += Database_PropertyChanged;
        }

        private void Database_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.MatchProperty(nameof(DatabaseModel.User)))
            {
                Icon = GetIcon(databaseViewModel.Database);
            }
        }

        private static ImageMoniker GetIcon(DatabaseModel database)
        {
            if (database.User == null)
            {
                return ImageMoniker.ReadOnlyDatabase;
            }
            else
            {
                return ImageMoniker.Database;
            }
        }

        public DatabaseViewModel DatabaseViewModel => databaseViewModel;

        public void OnClose()
        {
            databaseViewModel.Dispose();
        }
    }
}
