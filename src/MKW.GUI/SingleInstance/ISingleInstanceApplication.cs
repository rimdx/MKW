namespace MKW.GUI.SingleInstance
{
    public interface ISingleInstanceApplication
    {
        void InvokeMainInstance(string[] args);
        void InvokeExternalInstance(string[] args);
    }
}
