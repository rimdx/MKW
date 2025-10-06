using MKW.Core;
using MKW.GUI.EntryEditor;
using System.Collections.ObjectModel;

namespace MKW.GUI.Model
{
    public sealed class CommonEntryPropertiesModel
    {
        public ObservableCollection<CustomPropertyInfo> CommonCustomPropertyNames { get; }

        public CommonEntryPropertiesModel(IEnumerable<EntryPayloadEditorViewModel> entries)
        {
            CommonCustomPropertyNames = [];

            foreach (EntryPayloadEditorViewModel entry in entries)
            {
                ReceivedEntry(entry.GetPayload());
            }
        }

        public void ReceivedEntry(EntryPayload entry)
        {
            foreach (KeyValuePair<EntryPayloadKey, string> item in entry)
            {
                ReceiveProperty(item.Key);
            }
        }

        public void ReceiveProperty(EntryPayloadKey key)
        {
            if (EntryPayloadKey.IsInstance(CustomPropertyNamespace, key))
            {
                string name = EntryPayloadKey.RelativeName(CustomPropertyNamespace, key);

                CustomPropertyInfo property = new CustomPropertyInfo(name);

                CommonCustomPropertyNames.Remove(property);
                CommonCustomPropertyNames.Insert(0, property);
            }
        }

        public readonly static EntryPayloadKey DefaultNamespace = new EntryPayloadKey("mkw");

        public readonly static EntryPayloadKey Title = DefaultNamespace.Branch("title");
        public readonly static EntryPayloadKey Username = DefaultNamespace.Branch("username");
        public readonly static EntryPayloadKey Password = DefaultNamespace.Branch("password");
        public readonly static EntryPayloadKey Url = DefaultNamespace.Branch("url");
        public readonly static EntryPayloadKey Notes = DefaultNamespace.Branch("notes");

        public readonly static EntryPayloadKey CustomPropertyNamespace = DefaultNamespace.Branch("custom");
    }
}
