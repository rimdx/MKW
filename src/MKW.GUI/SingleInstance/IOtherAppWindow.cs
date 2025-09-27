namespace MKW.GUI.SingleInstance
{
    internal interface IOtherAppWindow
    {
        bool SendDataMessage(IntPtr messageId, byte[] data);
    }
}
