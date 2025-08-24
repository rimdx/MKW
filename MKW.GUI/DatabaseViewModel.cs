using MKW.Core.Client;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MKW.GUI
{
    public class DatabaseViewModel : INotifyPropertyChanged, IDisposable
    {
        public DatabaseModel Database { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public DatabaseViewModel(DatabaseModel database)
        {
            Database = database;
            Database.OnEntriesChanged += Database_OnEntriesChanged;
            Entries = [];
            RefreshEntries();
        }

        private void Database_OnEntriesChanged(object? sender, EventArgs e)
        {
            RefreshEntries();
        }

        public ObservableCollection<DatabaseEntryModel> Entries { get; private set; }

        private DatabaseEntryModel? _selectedEntry;
        public DatabaseEntryModel? SelectedEntry
        {
            get => _selectedEntry;
            set
            {
                _selectedEntry = value;
                OnPropertyChanged(nameof(SelectedEntry));
                OnPropertyChanged(nameof(IsEntrySelected));
            }
        }

        public bool IsEntrySelected => _selectedEntry != null;

        public void RefreshEntries()
        {
            Entries.Clear();

            foreach (UserEntry entry in Database.User!.EnumerateEntries())
            {
                Entries.Add(new DatabaseEntryModel
                {
                    Id = entry.Id,
                    Payload = entry.OpenPayload()?.ToString()
                });
            }
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
            Database.Dispose();
        }
    }
}
