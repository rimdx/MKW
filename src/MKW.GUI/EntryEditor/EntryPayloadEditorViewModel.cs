using MKW.Core;
using MKW.GUI.Model;
using System.Windows.Data;

namespace MKW.GUI.EntryEditor
{
    public class EntryPayloadEditorViewModel : ViewModelBase
    {
        private readonly EntryPayload payload;
        private readonly CommonEntryPropertiesModel commonPropertiesModel;

        public EntryId Id { get; }

        public EntryPayloadEditorPropertiesViewModel Properties { get; }

        public EntryPayloadEditorViewModel(EntryId id, EntryPayload payload,
                                           CommonEntryPropertiesModel commonPropertiesModel)
        {
            Id = id;
            this.payload = payload;
            this.commonPropertiesModel = commonPropertiesModel;

            Properties = new EntryPayloadEditorPropertiesViewModel(payload, commonPropertiesModel);

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

            return EntryPayloadKey.IsInstance(CommonEntryPropertiesModel.CustomPropertyNamespace, value.Key);
        }

        public NewCustomPropertyViewModel NewCustomProperty()
        {
            return new NewCustomPropertyViewModel(Properties, commonPropertiesModel);
        }

        public EditCustomPropertyViewModel EditCustomProperty(EntryPayloadValueViewModel property)
        {
            return new EditCustomPropertyViewModel(Properties, commonPropertiesModel,
                                                   property.Key, property.Value);
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
