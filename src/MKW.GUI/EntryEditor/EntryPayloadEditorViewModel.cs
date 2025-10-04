using MKW.Core;

namespace MKW.GUI.EntryEditor
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
            get => payload.GetPropertyOrEmpty(EntryPayloadCommonProperties.Title);
            set => payload.SetProperty(EntryPayloadCommonProperties.Title, value);
        }

        public string Username
        {
            get => payload.GetPropertyOrEmpty(EntryPayloadCommonProperties.Username);
            set => payload.SetProperty(EntryPayloadCommonProperties.Username, value);
        }

        public string Password
        {
            get => payload.GetPropertyOrEmpty(EntryPayloadCommonProperties.Password);
            set => payload.SetProperty(EntryPayloadCommonProperties.Password, value);
        }

        public string Url
        {
            get => payload.GetPropertyOrEmpty(EntryPayloadCommonProperties.Url);
            set => payload.SetProperty(EntryPayloadCommonProperties.Url, value);
        }

        public string Notes
        {
            get => payload.GetPropertyOrEmpty(EntryPayloadCommonProperties.Notes);
            set => payload.SetProperty(EntryPayloadCommonProperties.Notes, value);
        }

        public EntryPayload GetPayload()
        {
            return payload;
        }
    }
}
