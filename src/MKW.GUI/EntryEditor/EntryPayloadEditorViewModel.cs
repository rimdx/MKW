using MKW.Core;
using System.Collections.ObjectModel;

namespace MKW.GUI.EntryEditor
{
    public class EntryPayloadEditorViewModel : ViewModelBase
    {
        private readonly EntryPayload payload;

        public EntryId Id { get; }

        public EntryPayloadEditorPropertiesViewModel Properties { get; }

        public EntryPayloadEditorViewModel(EntryId id, EntryPayload payload)
        {
            Id = id;
            this.payload = payload;

            Properties = new EntryPayloadEditorPropertiesViewModel(payload);

            CustomProperties = [];
            RefreshCustomProperties();
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
