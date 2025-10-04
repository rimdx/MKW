using MKW.Core;
using MKW.GUI.EntryEditor;
using MKW.GUI.Model;

namespace MKW.GUI
{
    public class EditEntryWindowViewModel : ViewModelBase, IDisposable
    {
        private readonly DatabaseUnlockedModel database;
        private readonly IEntrySession entry;

        public EntryPayloadEditorViewModel Payload { get; }

        public EditEntryWindowViewModel(DatabaseUnlockedModel database, IEntrySession entry)
        {
            this.database = database;
            this.entry = entry;

            EntryPayload? payload = entry.OpenPayload();

            if (payload != null)
            {
                Payload = new EntryPayloadEditorViewModel(entry.Id, payload);
            }
            else
            {
                throw new Exception("Can't open entry content.");
            }
        }

        public bool OnOK()
        {
            database.UpdateEntry(entry, Payload.GetPayload());
            return true;
        }

        public void Dispose()
        {
            entry.Dispose();
        }
    }
}
