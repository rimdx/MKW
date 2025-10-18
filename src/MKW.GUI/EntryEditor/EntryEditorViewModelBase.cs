// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.GUI.Model;

namespace MKW.GUI.EntryEditor
{
    public abstract class EntryEditorViewModelBase : ViewModelBase
    {
        public EntryEditorModel Payload { get; }
        private readonly CommonEntryPropertiesModel commonPropertiesModel;

        private EntryValueModel? selectedCustomProperty;

        public EntryEditorViewModelBase(EntryEditorModel payload,
                                        CommonEntryPropertiesModel commonPropertiesModel)
        {
            Payload = payload;
            this.commonPropertiesModel = commonPropertiesModel;
        }

        public EntryValueModel? SelectedCustomProperty
        {
            get => selectedCustomProperty;
            set => SetProperty(ref selectedCustomProperty, value);
        }

        public void OnOK()
        {
            SaveEntry(Payload.GetPayload());
        }

        public NewCustomPropertyViewModel NewCustomProperty()
        {
            return new NewCustomPropertyViewModel(Payload.Properties, commonPropertiesModel);
        }

        public EditCustomPropertyViewModel EditCustomProperty(EntryValueModel property)
        {
            return new EditCustomPropertyViewModel(Payload.Properties, commonPropertiesModel,
                                                   property.Key, property.Value);
        }

        protected abstract void SaveEntry(EntryPayload payload);
    }
}
