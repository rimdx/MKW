using MKW.GUI.Model;
using System.ComponentModel;

namespace MKW.GUI.Database
{
    public class DatabaseEntryCollectionViewModel : TransformedObservableCollection<EntryListViewModel, EntryEditorModel>, IDisposable
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
            SetItems(database.Entries);
        }

        public void Dispose()
        {
            database.PropertyChanged -= Database_PropertyChanged;
        }

        protected override EntryListViewModel CreateViewModel(EntryEditorModel item) => new EntryListViewModel(item);
        protected override void UpdateViewModel(EntryListViewModel viewModel, EntryEditorModel item) => viewModel.EntryEditorModel = item;
        protected override object GetViewModelKey(EntryListViewModel viewModel) => viewModel.EntryEditorModel.Id;
        protected override object GetItemKey(EntryEditorModel item) => item.Id;
    }
}
