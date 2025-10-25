// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.GUI.Model;

namespace MKW.GUI.Database
{
    public class EntryListColumn : ViewModelBase
    {
        private string header;
        private double width;
        private EntryPayloadKey propertyKey;
        private bool hideValue;

        public EntryListColumn(string header, int width, PropertyInfo property)
        {
            this.header = header;
            this.width = width;
            Property = property;
            this.propertyKey = property.Key;
        }

        public string Header
        {
            get => header;
            set => SetProperty(ref header, value);
        }

        public double Width
        {
            get => width;
            set => SetProperty(ref width, value);
        }

        public EntryPayloadKey PropertyKey
        {
            get => propertyKey;
            set => SetProperty(ref propertyKey, value);
        }

        public bool HideValue
        {
            get => hideValue;
            set => SetProperty(ref hideValue, value);
        }

        public PropertyInfo Property { get; }
    }
}
