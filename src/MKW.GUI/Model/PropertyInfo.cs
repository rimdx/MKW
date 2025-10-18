// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;

namespace MKW.GUI.Model
{
    public sealed class PropertyInfo : ViewModelBase, IPropertyInfo
    {
        public EntryPayloadKey Key { get; }

        public string Name { get; }

        public PropertyInfo(EntryPayloadKey key, string name)
        {
            Key = key;
            Name = name;
        }

        public override bool Equals(object? obj)
        {
            return obj is IPropertyInfo info && Key.Equals(info.Key);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }
}
