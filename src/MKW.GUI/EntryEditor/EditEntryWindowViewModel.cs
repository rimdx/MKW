// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.GUI.Model;

namespace MKW.GUI.EntryEditor
{
    public class EditEntryWindowViewModel : EntryEditorViewModelBase
    {
        private readonly DatabaseUnlockedModel database;
        private readonly EntryId entryId;

        public EditEntryWindowViewModel(DatabaseUnlockedModel database, EntryId entryId)
            : base(GetEditor(database, entryId), database.CommonPropertiesModel)
        {
            this.database = database;
            this.entryId = entryId;
        }

        private static EntryEditorModel GetEditor(DatabaseUnlockedModel database, EntryId entryId)
        {
            EntryPayload? payload = database.OpenEntry(entryId);

            if (payload != null)
            {
                return new EntryEditorModel(entryId, payload, database.CommonPropertiesModel);
            }
            else
            {
                throw new Exception("Can't open entry content.");
            }
        }

        protected override void SaveEntry(EntryPayload payload)
        {
            database.UpdateEntry(entryId, payload);
        }
    }
}
