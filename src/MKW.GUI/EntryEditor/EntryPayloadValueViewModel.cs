using MKW.Core;

namespace MKW.GUI.EntryEditor
{
    public class EntryPayloadValueViewModel : ViewModelBase
    {
        private readonly EntryPayload payload;

        public EntryPayloadKey Key { get; }

        public EntryPayloadValueViewModel(EntryPayload payload, EntryPayloadKey key)
        {
            this.payload = payload;
            Key = key;
        }

        public string DisplayValue => Value;

        public string Value
        {
            get => payload.GetPropertyOrEmpty(Key);
            set => payload.SetProperty(Key, value);
        }
    }
}
