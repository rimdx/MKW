namespace MKW.GUI.SingleInstance
{
    public interface ISingleInstanceApplication
    {
        void InvokeExternalInstance(string[] args);
    }
}
