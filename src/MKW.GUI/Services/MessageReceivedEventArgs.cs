namespace MKW.GUI.Services
{
    internal class MessageReceivedEventArgs(IntPtr messageId, byte[] data) : EventArgs
    {
        public IntPtr MessageId { get; } = messageId;
        public byte[] Data { get; } = data;
    }
}
