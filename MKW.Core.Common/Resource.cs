namespace MKW.Core.Common
{
    public class Resource<T> : IResource, IDisposable where T : IResource
    {
        private ResourceState state;

        protected Resource()
        {
            _value = this;
            state = ResourceState.Original;
        }

        private Resource(T value, ResourceState state)
        {
            _value = value;
            this.state = state;
        }

        private readonly T _value;

        public T Value => _value;
        IResource IResource.Value => _value;

        public static implicit operator T(Resource<T> resource)
        {
            return resource.As<T>();
        }

        public C As<C>() where C : IResource
        {
            return (C)(object)Value;
        }

        IResource IResource.Reference()
        {
            return Reference<T>();
        }

        public Resource<C> Reference<C>() where C : IResource
        {
            return new Resource<C>(As<C>(), ResourceState.Reference);
        }

        IResource IResource.Move()
        {
            return Move<T>();
        }

        public Resource<C> Move<C>() where C : IResource
        {
            state = ResourceState.OriginalMovedOut;
            return new Resource<C>(As<C>(), ResourceState.ReferenceOwned);
        }

        public virtual void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            if (state == ResourceState.Original || state == ResourceState.ReferenceOwned)
            {
                state = ResourceState.Disposed;
                Value.Dispose(true);
            }
        }
    }
}
