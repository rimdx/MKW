using MKW.Common;
using MKW.GUI.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MKW.GUI.Database
{
    public class DatabaseEntryCollectionViewModel : ObservableCollection<EntryEditorModel>, IDisposable
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
            foreach (LeftRightPair<EntryEditorModel> pair in CollectionHelpers.Merge(this, database.Entries, value => value.Id))
            {
                if (pair.Left == null && pair.Right != null)
                {
                    Add(pair.Right);
                }
                else if (pair.Left != null && pair.Right == null)
                {
                    Remove(pair.Left);
                }
                else if (pair.Left != null && pair.Right != null)
                {
                    SetItem(IndexOf(pair.Left), pair.Right);
                }
            }
        }

        public void Dispose()
        {
            database.PropertyChanged -= Database_PropertyChanged;
        }
    }
}
