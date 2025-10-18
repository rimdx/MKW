// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;

namespace MKW.GUI.Model
{
    public sealed class CustomPropertyInfo : IPropertyInfo
    {
        public EntryPayloadKey Key { get; }
        public string Name { get; }

        public CustomPropertyInfo(string name)
        {
            Name = name;
            Key = CommonEntryPropertiesModel.CustomPropertyNamespace.Branch(name);
        }

        public override bool Equals(object? obj)
        {
            return obj is IPropertyInfo info && Key.Equals(info.Key);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
