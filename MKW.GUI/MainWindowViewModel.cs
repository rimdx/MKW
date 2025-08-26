using MKW.GUI.Model;
using System.ComponentModel;

namespace MKW.GUI
{
    public class MainWindowViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string Title => "Multi-Key Wallet";

        private DatabaseViewModel? _database;
        public DatabaseViewModel? Database
        {
            get => _database;
            set
            {
                _database?.Dispose();
                _database = value;
                OnPropertyChanged(nameof(Database));
                OnPropertyChanged(nameof(IsDatabaseAttached));
                OnPropertyChanged(nameof(IsEntrySelected));

                if (_database != null)
                {
                    _database.PropertyChanged += Database_PropertyChanged;
                }
            }
        }

        private void Database_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_database.SelectedEntry))
            {
                OnPropertyChanged(nameof(SelectedEntry));
                OnPropertyChanged(nameof(IsEntrySelected));
            }
        }

        public bool IsDatabaseAttached => _database != null;

        public DatabaseEntryModel? SelectedEntry => _database?.SelectedEntry;
        public bool IsEntrySelected => SelectedEntry != null;

        public DatabaseModel GetDatabase()
        {
            if (_database == null)
            {
                throw new Exception("No database is attached.");
            }

            return _database.Database;
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void Dispose()
        {
            Database?.Dispose();
        }
    }
}
