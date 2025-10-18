// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace MKW.GUI.Win32
{
    public static class Shell32
    {
        [DllImport("shell32")]
        public static extern int SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath,
                                                             IBindCtx? pbc,
                                                             [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
                                                             out IShellItem ppv);
    }
}
