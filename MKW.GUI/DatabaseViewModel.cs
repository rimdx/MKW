using MKW.Core.Client;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MKW.GUI
{
    public class DatabaseViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly DatabaseModel database;

        public event PropertyChangedEventHandler? PropertyChanged;

        public DatabaseViewModel(DatabaseModel database)
        {
            Entries = [];

            foreach (UserEntry entry in database.User!.EnumerateEntries())
            {
                Entries.Add(new DatabaseEntryModel
                {
                    Id = entry.Id,
                    Payload = entry.OpenPayload()?.ToString()
                });
            }

            this.database = database;
        }

        public ObservableCollection<DatabaseEntryModel> Entries { get; private set; }

        public void Dispose()
        {
            database.Dispose();
        }
    }
}
