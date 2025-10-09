using MKW.Core;
using MKW.Core.Exceptions;
using System.Collections.ObjectModel;

namespace MKW.GUI.Model
{
    public class EntryEditorPropertiesModel : ViewModelBase
    {
        public ObservableCollection<EntryValueModel> Collection { get; }

        private readonly Dictionary<EntryPayloadKey, EntryValueModel> values;

        private readonly EntryPayload payload;
        private readonly CommonEntryPropertiesModel commonPropertiesModel;

        public EntryEditorPropertiesModel(EntryPayload payload,
                                                 CommonEntryPropertiesModel commonPropertiesModel)
        {
            this.payload = payload;
            this.commonPropertiesModel = commonPropertiesModel;

            Collection = [];
            values = [];

            foreach (KeyValuePair<EntryPayloadKey, string> item in payload)
            {
                EntryValueModel value = new EntryValueModel(payload, item.Key);

                values.Add(item.Key, value);
                Collection.Add(value);
            }
        }

        public EntryValueModel this[string keyStr]
        {
            get
            {
                return GetEditor(new EntryPayloadKey(keyStr));
            }
        }

        public EntryValueModel this[EntryPayloadKey key]
        {
            get
            {
                return GetEditor(key);
            }
        }

        private EntryValueModel GetEditor(EntryPayloadKey key)
        {
            if (values.TryGetValue(key, out EntryValueModel value))
            {
                return value;
            }
            else
            {
                EntryValueModel result = new EntryValueModel(payload, key);
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

                EntryValueModel valueModel = GetEditor(newKey);
                valueModel.Value = value;
            }
            else if (oldKey.Equals(newKey))
            {
                // edit

                EntryValueModel valueModel = GetEditor(newKey);
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
