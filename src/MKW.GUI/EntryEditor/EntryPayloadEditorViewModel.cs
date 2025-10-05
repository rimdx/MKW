using MKW.Core;
using System.Windows.Data;

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

            CustomProperties = new ListCollectionView(Properties.Collection)
            {
                Filter = CustomPropertiesFilter,
            };
        }

        public EntryPayload GetPayload()
        {
            return payload;
        }

        private bool CustomPropertiesFilter(object item)
        {
            EntryPayloadValueViewModel value = (EntryPayloadValueViewModel)item;

            return EntryPayloadKey.IsInstance(EntryPayloadCommonProperties.CustomPropertyNamespace, value.Key);
        }

        public NewCustomPropertyViewModel NewCustomProperty()
        {
            return new NewCustomPropertyViewModel(Properties);
        }

        public EditCustomPropertyViewModel EditCustomProperty(EntryPayloadValueViewModel property)
        {
            return new EditCustomPropertyViewModel(Properties, property.Key, property.Value);
        }

        public ListCollectionView CustomProperties { get; }

        private EntryPayloadValueViewModel? selectedCustomProperty;
        public EntryPayloadValueViewModel? SelectedCustomProperty
        {
            get => selectedCustomProperty;
            set => SetProperty(ref selectedCustomProperty, value);
        }
    }
}
