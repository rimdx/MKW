using MKW.Core;

namespace MKW.GUI.Model
{
    public class EntryPayloadValueModel : ViewModelBase
    {
        private readonly EntryPayload payload;

        public EntryPayloadKey Key { get; }

        public EntryPayloadValueModel(EntryPayload payload, EntryPayloadKey key)
        {
            this.payload = payload;
            Key = key;
        }

        public string DisplayKey => Key.ToString();

        public string DisplayValue => Value;

        public string PropertyName => EntryPayloadKey.RelativeName(
            CommonEntryPropertiesModel.CustomPropertyNamespace, Key);

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
