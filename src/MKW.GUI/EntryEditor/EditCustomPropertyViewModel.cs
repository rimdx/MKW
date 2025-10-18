// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.GUI.Model;

namespace MKW.GUI.EntryEditor
{
    public class EditCustomPropertyViewModel : CustomPropertyEditorViewModelBase
    {
        private readonly EntryEditorPropertiesModel properties;
        private readonly EntryPayloadKey key;

        public CommonEntryPropertiesModel CommonPropertiesModel { get; }

        public EditCustomPropertyViewModel(EntryEditorPropertiesModel properties,
                                           CommonEntryPropertiesModel commonPropertiesModel,
                                           EntryPayloadKey key, string content)
            : base(GetName(key), content)
        {
            this.properties = properties;
            CommonPropertiesModel = commonPropertiesModel;
            this.key = key;
        }

        private static string GetName(EntryPayloadKey key)
        {
            return EntryPayloadKey.RelativeName(CommonEntryPropertiesModel.CustomPropertyNamespace, key);
        }

        public override void OnOK()
        {
            properties.SetCustomProperty(key, Name, Content);
        }
    }
}
