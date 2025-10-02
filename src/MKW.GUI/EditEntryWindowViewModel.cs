using MKW.Core;
using MKW.GUI.Model;

namespace MKW.GUI
{
    public class EditEntryWindowViewModel : ViewModelBase, IDisposable
    {
        private readonly DatabaseUnlockedModel database;
        private readonly IEntrySession entry;

        private readonly EntryPayload payload;

        public EditEntryWindowViewModel(DatabaseUnlockedModel database, IEntrySession entry)
        {
            this.database = database;
            this.entry = entry;

            EntryPayload? payload = entry.OpenPayload();

            if (payload != null)
            {
                this.payload = payload;
            }
            else
            {
                throw new Exception("Can't open entry content.");
            }
        }

        public string Notes
        {
            get => payload.Notes ?? string.Empty;
            set => payload.Notes = value;
        }

        public bool OnOK()
        {
            database.UpdateEntry(entry, payload);
            return true;
        }

        public void Dispose()
        {
            entry.Dispose();
        }
    }
}
