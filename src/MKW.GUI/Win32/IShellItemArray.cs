// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Runtime.InteropServices;

namespace MKW.GUI.Win32
{
    [ComImport, Guid("b63ea76d-1f85-456f-a19c-48159efa858b"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IShellItemArray
    {
        [PreserveSig] int BindToHandler();  // not fully defined
        [PreserveSig] int GetPropertyStore();  // not fully defined
        [PreserveSig] int GetPropertyDescriptionList();  // not fully defined
        [PreserveSig] int GetAttributes();  // not fully defined
        [PreserveSig] int GetCount(out int pdwNumItems);
        [PreserveSig] int GetItemAt(int dwIndex, out IShellItem ppsi);
        [PreserveSig] int EnumItems();  // not fully defined
    }
}
