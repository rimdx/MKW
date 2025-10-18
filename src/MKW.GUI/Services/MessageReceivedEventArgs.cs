// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

namespace MKW.GUI.Services
{
    internal class MessageReceivedEventArgs(IntPtr messageId, byte[] data) : EventArgs
    {
        public IntPtr MessageId { get; } = messageId;
        public byte[] Data { get; } = data;
    }
}
