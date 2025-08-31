namespace MKW.Core.Common
{
    public class Resource<T> : IResource, IDisposable where T : IResource
    {
        private ResourceState state;

        protected Resource()
        {
            state = ResourceState.Original;
        }

        private Resource(T value, ResourceState state)
        {
            this.value = value;
            this.state = state;
        }

        private readonly T? value;

        public T Value
        {
            get
            {
                if (value == null)
                {
                    return (T)(object)this;
                }
                else
                {
                    return value;
                }
            }
        }

        IResource IResource.Value => Value;

        public static implicit operator T(Resource<T> resource)
        {
            return resource.As<T>();
        }

        public C As<C>() where C : T, IResource
        {
            return (C)Value;
        }

        IResource IResource.Reference()
        {
            return Reference<T>();
        }

        public Resource<C> Reference<C>() where C : T, IResource
        {
            return new Resource<C>(As<C>(), ResourceState.Reference);
        }

        IResource IResource.Move()
        {
            return Move<T>();
        }

        public Resource<C> Move<C>() where C : T
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
                Value.Dispose(true);
            }
        }
    }
}
