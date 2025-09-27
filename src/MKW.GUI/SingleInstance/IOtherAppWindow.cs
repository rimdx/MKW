namespace MKW.GUI.SingleInstance
{
    internal interface IOtherAppWindow
    {
        bool SendDataMessage(uint messageId, byte[] data);
    }
}
