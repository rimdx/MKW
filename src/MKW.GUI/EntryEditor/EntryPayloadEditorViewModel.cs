using MKW.Core;
using System.Collections.ObjectModel;

namespace MKW.GUI.EntryEditor
{
    public class EntryPayloadEditorViewModel : ViewModelBase
    {
        private readonly EntryPayload payload;

        public EntryId Id { get; }

        public EntryPayloadEditorViewModel(EntryId id, EntryPayload payload)
        {
            Id = id;
            this.payload = payload;

            CustomProperties = [];
            RefreshCustomProperties();
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

        private void RefreshCustomProperties()
        {
            CustomProperties.Clear();
            foreach (KeyValuePair<EntryPayloadKey, string> item in payload)
            {
                CustomProperties.Add(item);
            }
        }

        public ObservableCollection<KeyValuePair<EntryPayloadKey, string>> CustomProperties { get; }
    }
}
