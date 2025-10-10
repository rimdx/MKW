using MKW.Core;
using System.Windows.Data;

namespace MKW.GUI.Model
{
    public class EntryEditorModel : ViewModelBase
    {
        private readonly EntryPayload payload;
        private readonly CommonEntryPropertiesModel commonPropertiesModel;

        public EntryId Id { get; }

        public EntryEditorPropertiesModel Properties { get; }

        public EntryEditorModel(EntryId id, EntryPayload payload,
                                CommonEntryPropertiesModel commonPropertiesModel)
        {
            Id = id;
            this.payload = payload;
            this.commonPropertiesModel = commonPropertiesModel;

            Properties = new EntryEditorPropertiesModel(payload, commonPropertiesModel);

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
            EntryValueModel value = (EntryValueModel)item;

            return EntryPayloadKey.IsInstance(CommonEntryPropertiesModel.CustomPropertyNamespace, value.Key);
        }

        public ListCollectionView CustomProperties { get; }
    }
}
