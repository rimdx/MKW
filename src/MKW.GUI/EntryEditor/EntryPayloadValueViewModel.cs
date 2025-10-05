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

        public string DisplayKey => Key.ToString();

        public string DisplayValue => Value;

        public string PropertyName => EntryPayloadKey.RelativeName(
            EntryPayloadCommonProperties.CustomPropertyNamespace, Key);

        public string Value
        {
            get => payload.GetPropertyOrEmpty(Key);
            set
            {
                payload.SetProperty(Key, value);
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(DisplayValue));
            }
        }
    }
}
