using MKW.Core;
using MKW.GUI.Model;

namespace MKW.GUI
{
    public class NewEntryWindowViewModel : ViewModelBase
    {
        private readonly DatabaseUnlockedModel database;

        public EntryPayloadEditorModel Payload { get; }

        public NewEntryWindowViewModel(DatabaseUnlockedModel database)
        {
            this.database = database;

            Payload = new EntryPayloadEditorModel(EntryId.Create(), new EntryPayload(),
                                                      database.CommonPropertiesModel);
        }

        public bool OnOK()
        {
            database.CreateEntry(Payload.Id, Payload.GetPayload());
            return true;
        }
    }
}
