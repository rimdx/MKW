using MKW.Core;
using MKW.GUI.Model;

namespace MKW.GUI
{
    public class EditEntryWindowViewModel : ViewModelBase, IDisposable
    {
        private readonly DatabaseModel database;
        private readonly IEntrySession entry;

        public EditEntryWindowViewModel(DatabaseModel database, IEntrySession entry)
        {
            this.database = database;
            this.entry = entry;

            string? payload = entry.OpenPayload()?.ToString();

            if (payload != null)
            {
                Payload = payload;
            }
        }

        private string _payload = "";
        public string Payload
        {
            get => _payload;
            set => SetProperty(ref _payload, value);
        }

        public bool OnOK()
        {
            database.UpdateEntry(entry, Payload);
            return true;
        }

        public void Dispose()
        {
            entry.Dispose();
        }
    }
}
