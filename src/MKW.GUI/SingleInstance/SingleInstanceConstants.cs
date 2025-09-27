namespace MKW.GUI.SingleInstance
{
    internal class SingleInstanceConstants
    {
        public const string ApplicationMagic = "MKW.{A2AF56DE-DDB8-4FC6-86BA-666ACC77EC98}";

        public const string SingleInstanceMutexName = $"{ApplicationMagic}.SingleInstanceMutex";

        public static class DataMessageId
        {
            public const nint RunRequest = 1;
        }
    }
}
