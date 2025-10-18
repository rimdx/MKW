// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.GUI.Model;

namespace MKW.GUI.EntryEditor
{
    public class NewCustomPropertyViewModel : CustomPropertyEditorViewModelBase
    {
        private readonly EntryEditorPropertiesModel properties;

        public CommonEntryPropertiesModel CommonPropertiesModel { get; }

        public NewCustomPropertyViewModel(EntryEditorPropertiesModel properties,
                                          CommonEntryPropertiesModel commonPropertiesModel)
            : base(string.Empty, string.Empty)
        {
            this.properties = properties;
            CommonPropertiesModel = commonPropertiesModel;
        }

        public override void OnOK()
        {
            properties.SetCustomProperty(null, Name, Content);
        }
    }
}
