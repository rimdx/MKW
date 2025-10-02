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

        public string Title
        {
            get => payload.Title ?? string.Empty;
            set => payload.Title = value;
        }

        public string Username
        {
            get => payload.Username ?? string.Empty;
            set => payload.Username = value;
        }

        public string Password
        {
            get => payload.Password ?? string.Empty;
            set => payload.Password = value;
        }

        public string Url
        {
            get => payload.Url ?? string.Empty;
            set => payload.Url = value;
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
