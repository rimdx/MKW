// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;

namespace MKW.GUI.Model
{
    public interface IPropertyInfo
    {
        EntryPayloadKey Key { get; }
        string Name { get; }
    }
}
