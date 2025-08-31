namespace MKW.Core.Common
{
    public interface IResource : IDisposable
    {
        public IResource Value { get; }

        IResource Reference();
        IResource Move();

        void Dispose(bool disposing);
    }
}
