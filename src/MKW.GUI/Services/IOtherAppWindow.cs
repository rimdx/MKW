namespace MKW.GUI.Services
{
    internal interface IOtherAppWindow
    {
        bool SendDataMessage(IntPtr messageId, byte[] data);
    }
}
