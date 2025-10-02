using MKW.Core;
using MKW.GUI.Model;

namespace MKW.GUI
{
    public class NewEntryWindowViewModel : ViewModelBase
    {
        private readonly DatabaseUnlockedModel database;
        private readonly EntryPayload payload;

        public NewEntryWindowViewModel(DatabaseUnlockedModel database)
        {
            this.database = database;
            payload = new EntryPayload();
        }

        public string Notes
        {
            get => payload.Notes ?? string.Empty;
            set => payload.Notes = value;
        }

        public bool OnOK()
        {
            database.CreateEntry(payload);
            return true;
        }
    }
}
