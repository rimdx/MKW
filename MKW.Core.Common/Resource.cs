namespace MKW.Core.Common
{
    public class Resource<T> : IResource<T>, IDisposable where T : class, IResource<T>
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

        public static implicit operator T(Resource<T> resource)
        {
            return resource.Value;
        }

        public IResource<T> Reference()
        {
            return new Resource<T>(Value, ResourceState.Reference);
        }

        public IResource<T> Move()
        {
            state = ResourceState.OriginalMovedOut;
            return new Resource<T>(Value, ResourceState.ReferenceOwned);
        }

        public virtual void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            if (state == ResourceState.Original || state == ResourceState.ReferenceOwned)
            {
                if (value != null)
                {
                    value.Dispose(true);
                }
                else
                {
                    Dispose(true);
                }
            }
        }
    }
}
