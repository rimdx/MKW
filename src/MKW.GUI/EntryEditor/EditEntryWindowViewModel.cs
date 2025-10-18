// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.GUI.Model;

namespace MKW.GUI.EntryEditor
{
    public class EditEntryWindowViewModel : EntryEditorViewModelBase, IDisposable
    {
        private readonly DatabaseUnlockedModel database;
        private readonly IEntrySession entry;

        public EditEntryWindowViewModel(DatabaseUnlockedModel database, IEntrySession entry)
            : base(GetEditor(database, entry), database.CommonPropertiesModel)
        {
            this.database = database;
            this.entry = entry;
        }

        private static EntryEditorModel GetEditor(DatabaseUnlockedModel database, IEntrySession entry)
        {
            EntryPayload? payload = entry.OpenPayload();

            if (payload != null)
            {
                return new EntryEditorModel(entry.Id, payload, database.CommonPropertiesModel);
            }
            else
            {
                throw new Exception("Can't open entry content.");
            }
        }

        protected override void SaveEntry(EntryPayload payload)
        {
            database.UpdateEntry(entry, payload);
        }

        public void Dispose()
        {
            entry.Dispose();
        }
    }
}
