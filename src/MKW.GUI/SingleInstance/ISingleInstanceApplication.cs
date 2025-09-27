namespace MKW.GUI.SingleInstance
{
    public interface ISingleInstanceApplication
    {
        void InvokeExternalInstance(RunRequest request);
    }
}
