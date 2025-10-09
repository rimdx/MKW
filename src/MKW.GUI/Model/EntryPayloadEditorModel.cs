using MKW.Core;
using MKW.GUI.EntryEditor; // todo: remove
using System.Windows.Data;

namespace MKW.GUI.Model
{
    public class EntryPayloadEditorModel : ViewModelBase
    {
        private readonly EntryPayload payload;
        private readonly CommonEntryPropertiesModel commonPropertiesModel;

        public EntryId Id { get; }

        public EntryPayloadEditorPropertiesModel Properties { get; }

        public EntryPayloadEditorModel(EntryId id, EntryPayload payload,
                                       CommonEntryPropertiesModel commonPropertiesModel)
        {
            Id = id;
            this.payload = payload;
            this.commonPropertiesModel = commonPropertiesModel;

            Properties = new EntryPayloadEditorPropertiesModel(payload, commonPropertiesModel);

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
            EntryPayloadValueModel value = (EntryPayloadValueModel)item;

            return EntryPayloadKey.IsInstance(CommonEntryPropertiesModel.CustomPropertyNamespace, value.Key);
        }

        public NewCustomPropertyViewModel NewCustomProperty()
        {
            return new NewCustomPropertyViewModel(Properties, commonPropertiesModel);
        }

        public EditCustomPropertyViewModel EditCustomProperty(EntryPayloadValueModel property)
        {
            return new EditCustomPropertyViewModel(Properties, commonPropertiesModel,
                                                   property.Key, property.Value);
        }

        public ListCollectionView CustomProperties { get; }

        private EntryPayloadValueModel? selectedCustomProperty;
        public EntryPayloadValueModel? SelectedCustomProperty
        {
            get => selectedCustomProperty;
            set => SetProperty(ref selectedCustomProperty, value);
        }
    }
}
