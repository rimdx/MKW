using MKW.Core;
using MKW.Core.Exceptions;
using System.Collections.ObjectModel;

namespace MKW.GUI.EntryEditor
{
    public class EntryPayloadEditorPropertiesViewModel : ViewModelBase
    {
        public ObservableCollection<EntryPayloadValueViewModel> Collection { get; }

        private readonly Dictionary<EntryPayloadKey, EntryPayloadValueViewModel> values;

        private readonly EntryPayload payload;

        public EntryPayloadEditorPropertiesViewModel(EntryPayload payload)
        {
            this.payload = payload;

            Collection = [];
            values = [];

            foreach (KeyValuePair<EntryPayloadKey, string> item in payload)
            {
                EntryPayloadValueViewModel value = new EntryPayloadValueViewModel(payload, item.Key);

                values.Add(item.Key, value);
                Collection.Add(value);
            }
        }

        public EntryPayloadValueViewModel this[string keyStr]
        {
            get
            {
                return GetEditor(new EntryPayloadKey(keyStr));
            }
        }

        private EntryPayloadValueViewModel GetEditor(EntryPayloadKey key)
        {
            if (values.TryGetValue(key, out EntryPayloadValueViewModel value))
            {
                return value;
            }
            else
            {
                EntryPayloadValueViewModel result = new EntryPayloadValueViewModel(payload, key);
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
                newKey = EntryPayloadCommonProperties.CustomPropertyNamespace.Branch(name);
            }
            catch (InvalidEntryPayloadKey ex)
            {
                throw new Exception($"Property name is invalid: {ex.Reason}", ex);
            }

            if (oldKey == null)
            {
                // new

                EntryPayloadValueViewModel valueModel = GetEditor(newKey);
                valueModel.Value = value;
            }
            else if (oldKey.Equals(newKey))
            {
                // edit

                EntryPayloadValueViewModel valueModel = GetEditor(newKey);
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
