using MKW.Core;

namespace MKW.GUI.EntryEditor
{
    public class EntryPayloadEditorPropertiesViewModel : ViewModelBase
    {
        private readonly Dictionary<EntryPayloadKey, EntryPayloadValueViewModel> values;

        private readonly EntryPayload payload;

        public EntryPayloadEditorPropertiesViewModel(EntryPayload payload)
        {
            this.payload = payload;

            values = [];
            foreach (KeyValuePair<EntryPayloadKey, string> item in payload)
            {
                values[item.Key] = new EntryPayloadValueViewModel(payload, item.Key);
            }
        }

        public EntryPayloadValueViewModel this[string keyStr]
        {
            get
            {
                EntryPayloadKey key = new EntryPayloadKey(keyStr);

                if (values.TryGetValue(key, out EntryPayloadValueViewModel value))
                {
                    return value;
                }
                else
                {
                    EntryPayloadValueViewModel result = new EntryPayloadValueViewModel(payload, key);
                    values.Add(key, result);
                    return result;
                }
            }
        }
    }
}
