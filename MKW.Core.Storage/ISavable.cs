namespace MKW.Core.Storage
{
    public interface ISavable : IDisposable
    {
        void Save();

        void IDisposable.Dispose() => Save();
    }
}
