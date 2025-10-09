using MKW.Core;
using System.Collections.ObjectModel;

namespace MKW.GUI.Model
{
    public sealed class CommonEntryPropertiesModel
    {
        public ObservableCollection<CustomPropertyInfo> CommonCustomPropertyNames { get; }

        public ObservableCollection<IPropertyInfo> CommonProperties { get; }

        public CommonEntryPropertiesModel(IEnumerable<EntryPayloadEditorModel> entries)
        {
            CommonCustomPropertyNames = [];

            CommonProperties = [
                Title,
                Username,
                Password,
                Url,
                Notes,
            ];

            foreach (EntryPayloadEditorModel entry in entries)
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

                if (!CommonProperties.Contains(property))
                {
                    CommonProperties.Add(property);
                }
            }
        }

        public readonly static EntryPayloadKey DefaultNamespace = new EntryPayloadKey("mkw");
        public readonly static EntryPayloadKey CustomPropertyNamespace = DefaultNamespace.Branch("custom");

        public readonly static PropertyInfo Title =
            new PropertyInfo(DefaultNamespace.Branch("title"), "Title");

        public readonly static PropertyInfo Username =
            new PropertyInfo(DefaultNamespace.Branch("username"), "Username");

        public readonly static PropertyInfo Password =
            new PropertyInfo(DefaultNamespace.Branch("password"), "Password");

        public readonly static PropertyInfo Url =
            new PropertyInfo(DefaultNamespace.Branch("url"), "URL");

        public readonly static PropertyInfo Notes =
            new PropertyInfo(DefaultNamespace.Branch("notes"), "Notes");
    }
}
