using MKW.GUI.Win32;

namespace MKW.GUI.SingleInstance
{
    internal class SingleInstanceConstants
    {
        public const string ApplicationMagic = "MKW.{A2AF56DE-DDB8-4FC6-86BA-666ACC77EC98}";

        public const string SingleInstanceMutexName = $"{ApplicationMagic}.SingleInstanceMutex";
        public const string OpenFileMessageName = $"{ApplicationMagic}.OpenFile";

        public static uint OpenFileMessageId = User32.RegisterWindowMessage(OpenFileMessageName);
    }
}
