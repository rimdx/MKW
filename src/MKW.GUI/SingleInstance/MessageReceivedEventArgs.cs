namespace MKW.GUI.SingleInstance
{
    internal class MessageReceivedEventArgs(uint messageId, string data) : EventArgs
    {
        public uint MessageId { get; } = messageId;
        public string Data { get; } = data;
    }
}
