using MKW.Core;
using MKW.GUI.Model;

namespace MKW.GUI.EntryEditor
{
    public class NewEntryWindowViewModel : EntryEditorViewModelBase
    {
        private readonly DatabaseUnlockedModel database;

        public NewEntryWindowViewModel(DatabaseUnlockedModel database)
            : base(GetEditor(database))
        {
            this.database = database;
        }

        private static EntryEditorModel GetEditor(DatabaseUnlockedModel database)
        {
            return new EntryEditorModel(EntryId.Create(), new EntryPayload(),
                                        database.CommonPropertiesModel);
        }

        protected override void SaveEntry(EntryPayload payload)
        {
            database.CreateEntry(Payload.Id, Payload.GetPayload());
        }
    }
}
