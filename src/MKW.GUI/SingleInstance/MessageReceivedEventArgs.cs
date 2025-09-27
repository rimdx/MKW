namespace MKW.GUI.SingleInstance
{
    internal class MessageReceivedEventArgs(uint messageId, byte[] data) : EventArgs
    {
        public uint MessageId { get; } = messageId;
        public byte[] Data { get; } = data;
    }
}
