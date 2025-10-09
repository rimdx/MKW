using MKW.Core;
using MKW.Core.Exceptions;
using System.Collections.ObjectModel;

namespace MKW.GUI.Model
{
    public class EntryPayloadEditorPropertiesModel : ViewModelBase
    {
        public ObservableCollection<EntryPayloadValueModel> Collection { get; }

        private readonly Dictionary<EntryPayloadKey, EntryPayloadValueModel> values;

        private readonly EntryPayload payload;
        private readonly CommonEntryPropertiesModel commonPropertiesModel;

        public EntryPayloadEditorPropertiesModel(EntryPayload payload,
                                                 CommonEntryPropertiesModel commonPropertiesModel)
        {
            this.payload = payload;
            this.commonPropertiesModel = commonPropertiesModel;

            Collection = [];
            values = [];

            foreach (KeyValuePair<EntryPayloadKey, string> item in payload)
            {
                EntryPayloadValueModel value = new EntryPayloadValueModel(payload, item.Key);

                values.Add(item.Key, value);
                Collection.Add(value);
            }
        }

        public EntryPayloadValueModel this[string keyStr]
        {
            get
            {
                return GetEditor(new EntryPayloadKey(keyStr));
            }
        }

        public EntryPayloadValueModel this[EntryPayloadKey key]
        {
            get
            {
                return GetEditor(key);
            }
        }

        private EntryPayloadValueModel GetEditor(EntryPayloadKey key)
        {
            if (values.TryGetValue(key, out EntryPayloadValueModel value))
            {
                return value;
            }
            else
            {
                EntryPayloadValueModel result = new EntryPayloadValueModel(payload, key);
                values.Add(key, result);
                Collection.Add(result);
                return result;
            }
        }

        public void SetCustomProperty(EntryPayloadKey? oldKey, string name, string value)
        {
            EntryPayloadKey newKey;

            try
            {
                newKey = CommonEntryPropertiesModel.CustomPropertyNamespace.Branch(name);
            }
            catch (InvalidEntryPayloadKey ex)
            {
                throw new Exception($"Property name is invalid: {ex.Reason}", ex);
            }

            commonPropertiesModel.ReceiveProperty(newKey);

            if (oldKey == null)
            {
                // new

                EntryPayloadValueModel valueModel = GetEditor(newKey);
                valueModel.Value = value;
            }
            else if (oldKey.Equals(newKey))
            {
                // edit

                EntryPayloadValueModel valueModel = GetEditor(newKey);
                valueModel.Value = value;
            }
            else
            {
                // rename

                throw new NotImplementedException();
            }
        }
    }
}
