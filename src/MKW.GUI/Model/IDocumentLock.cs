namespace MKW.GUI.Model
{
    public interface IDocumentLock : IDisposable
    {
        DatabaseModel Database { get; }
    }
}
