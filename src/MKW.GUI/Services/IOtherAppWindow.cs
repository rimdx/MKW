// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.GUI.Services
{
    internal interface IOtherAppWindow
    {
        bool SendDataMessage(IntPtr messageId, byte[] data);
    }
}
