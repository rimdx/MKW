using MKW.Core;
using MKW.GUI.EntryEditor;
using MKW.GUI.Model;

namespace MKW.GUI
{
    public class NewEntryWindowViewModel : ViewModelBase
    {
        private readonly DatabaseUnlockedModel database;

        public EntryPayloadEditorViewModel Payload { get; }

        public NewEntryWindowViewModel(DatabaseUnlockedModel database)
        {
            this.database = database;
            Payload = new EntryPayloadEditorViewModel(EntryId.Create(), new EntryPayload());
        }

        public bool OnOK()
        {
            database.CreateEntry(Payload.Id, Payload.GetPayload());
            return true;
        }
    }
}
