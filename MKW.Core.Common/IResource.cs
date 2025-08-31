namespace MKW.Core.Common
{
    public interface IResource<T>
        : IDisposable
        where T : class, IDisposable
    {
        public T Value { get; }

        IResource<T> Reference();
        IResource<T> Move();

        void Dispose(bool disposing);
    }
}
