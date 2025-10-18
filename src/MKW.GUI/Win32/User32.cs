// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Runtime.InteropServices;

namespace MKW.GUI.Win32
{
    public static class User32
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint RegisterWindowMessage(string lpString);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageTimeout(IntPtr hWnd,
                                                       uint Msg,
                                                       IntPtr wParam,
                                                       IntPtr lParam,
                                                       SendMessageTimeoutFlags fuFlags,
                                                       uint uTimeout,
                                                       out IntPtr lpdwResult);
    }
}
