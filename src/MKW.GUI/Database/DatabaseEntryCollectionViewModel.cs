// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

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

        private void RefreshEntries()
        {
            SetItems(database.Entries);
        }

        public void Dispose()
        {
            database.PropertyChanged -= Database_PropertyChanged;
        }

        protected override EntryListViewModel CreateViewModel(EntryEditorModel item)
        {
            return new EntryListViewModel(item);
        }

        protected override void UpdateViewModel(EntryListViewModel viewModel, EntryEditorModel item)
        {
            viewModel.EntryEditorModel = item;
        }

        protected override object GetViewModelKey(EntryListViewModel viewModel)
        {
            return viewModel.EntryEditorModel.Id;
        }

        protected override object GetItemKey(EntryEditorModel item)
        {
            return item.Id;
        }
    }
}
