using MKW.GUI.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MKW.GUI.Database
{
    public class DatabaseEntryCollectionViewModel : ObservableCollection<DatabaseEntryModel>, IDisposable
    {
        private readonly DatabaseUnlockedModel database;

        public DatabaseEntryCollectionViewModel(DatabaseUnlockedModel database)
        {
            this.database = database;
            database.PropertyChanged += Database_PropertyChanged;
            RefreshEntries();
        }

        private void Database_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.MatchProperty(nameof(database.Entries)))
            {
                RefreshEntries();
            }
        }

        public void RefreshEntries()
        {
            Clear();

            foreach (DatabaseEntryModel entry in database.Entries)
            {
                Add(entry);
            }
        }

        public void Dispose()
        {
            database.PropertyChanged -= Database_PropertyChanged;
        }
    }
}
