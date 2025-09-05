namespace MKW.Core.Common
{
    public class Resource<T> : IDisposable where T : IDisposable
    {
        public T Value { get; }
        private ResourceState state;

        private Resource(T value, ResourceState state)
        {
            Value = value;
            this.state = state;
        }

        public static Resource<T> Attach(T value)
        {
            return new Resource<T>(value, ResourceState.Original);
        }


        public static implicit operator T(Resource<T> resource)
        {
            return resource.Value;
        }


        public Resource<T> Reference()
        {
            return new Resource<T>(Value, ResourceState.Reference);
        }

        public Resource<T> Move()
        {
            state = ResourceState.OriginalMovedOut;
            return new Resource<T>(Value, ResourceState.ReferenceOwned);
        }

        public void Dispose()
        {
            if (state == ResourceState.Original || state == ResourceState.ReferenceOwned)
            {
                state = ResourceState.Disposed;
                Value.Dispose();
            }
        }
    }
}
