using MKW.Core;

namespace MKW.GUI.EntryEditor
{
    public class EntryPayloadValueViewModel : ViewModelBase
    {
        private readonly EntryPayload payload;
        private readonly EntryPayloadKey key;

        public EntryPayloadValueViewModel(EntryPayload payload, EntryPayloadKey key)
        {
            this.payload = payload;
            this.key = key;
        }

        public string DisplayValue => Value;

        public string Value
        {
            get => payload.GetPropertyOrEmpty(key);
            set => payload.SetProperty(key, value);
        }
    }
}
