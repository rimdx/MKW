// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using System.Collections.ObjectModel;

namespace MKW.GUI.Model
{
    public sealed class CommonEntryPropertiesModel
    {
        public ObservableCollection<CustomPropertyInfo> CommonCustomPropertyNames { get; }

        public ObservableCollection<IPropertyInfo> CommonProperties { get; }

        public CommonEntryPropertiesModel(IEnumerable<EntryEditorModel> entries)
        {
            CommonCustomPropertyNames = [];

            CommonProperties = [
                Title,
                Username,
                Password,
                Url,
                Notes,
            ];

            foreach (EntryEditorModel entry in entries)
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

        public static readonly EntryPayloadKey DefaultNamespace = new EntryPayloadKey("mkw");
        public static readonly EntryPayloadKey CustomPropertyNamespace = DefaultNamespace.Branch("custom");

        public static readonly PropertyInfo Title =
            new PropertyInfo(DefaultNamespace.Branch("title"), "Title");

        public static readonly PropertyInfo Username =
            new PropertyInfo(DefaultNamespace.Branch("username"), "Username");

        public static readonly PropertyInfo Password =
            new PropertyInfo(DefaultNamespace.Branch("password"), "Password");

        public static readonly PropertyInfo Url =
            new PropertyInfo(DefaultNamespace.Branch("url"), "URL");

        public static readonly PropertyInfo Notes =
            new PropertyInfo(DefaultNamespace.Branch("notes"), "Notes");
    }
}
