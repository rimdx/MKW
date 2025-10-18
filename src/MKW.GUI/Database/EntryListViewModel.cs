// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.GUI.Model;

namespace MKW.GUI.Database
{
    public class EntryListViewModel : ViewModelBase
    {
        private EntryEditorModel entryEditorModel;

        public EntryListViewModel(EntryEditorModel entryEditorModel)
        {
            this.entryEditorModel = entryEditorModel;
        }

        public EntryEditorModel EntryEditorModel
        {
            get => entryEditorModel;
            set => SetProperty(ref entryEditorModel, value);
        }

        public EntryId Id => EntryEditorModel.Id;
    }
}
