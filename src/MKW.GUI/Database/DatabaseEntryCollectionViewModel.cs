using MKW.Core;
using MKW.GUI.Model;
using System.Collections.ObjectModel;

namespace MKW.GUI.Database
{
    public class DatabaseEntryCollectionViewModel : ObservableCollection<DatabaseEntryModel>, IDisposable
    {
        private readonly DatabaseUnlockedModel database;

        public DatabaseEntryCollectionViewModel(DatabaseUnlockedModel database)
        {
            this.database = database;

            database.OnEntriesChanged += Database_OnEntriesChanged;
            RefreshEntries();
        }

        private void Database_OnEntriesChanged(object? sender, EventArgs e)
        {
            RefreshEntries();
        }

        public void RefreshEntries()
        {
            Clear();

            foreach (IEntrySession entry in database.User!.EnumerateEntries())
            {
                Add(new DatabaseEntryModel
                {
                    Id = entry.Id,
                    Payload = entry.OpenPayload()?.ToString()
                });
            }
        }

        public void Dispose()
        {
            database.OnEntriesChanged -= Database_OnEntriesChanged;
        }
    }
}
