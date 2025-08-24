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
            Entries = [];
            RefreshEntries();
        }

        public ObservableCollection<DatabaseEntryModel> Entries { get; private set; }

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

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
