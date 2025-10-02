using MKW.Core;

namespace MKW.GUI
{
    public class EntryPayloadEditorViewModel : ViewModelBase
    {
        private readonly EntryPayload payload;

        public EntryPayloadEditorViewModel(EntryPayload payload)
        {
            this.payload = payload;
        }

        public string Notes
        {
            get => payload.Notes ?? string.Empty;
            set => payload.Notes = value;
        }

        public EntryPayload GetPayload()
        {
            return payload;
        }
    }
}
