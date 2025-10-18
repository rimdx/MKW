// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Runtime.InteropServices;

namespace MKW.GUI.Win32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct COPYDATASTRUCT
    {
        public IntPtr dwData;

        public int cbData;

        public IntPtr lpData;
    }
}
