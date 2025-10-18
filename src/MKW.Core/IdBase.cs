namespace MKW.Core
{
    public abstract class IdBase : IComparable<IdBase>
    {
        protected byte[] data;

        protected IdBase(byte[] data, int size)
        {
            if (data.Length != size)
            {
                throw new Exception("Bad id size.");
            }

            this.data = data;
        }

        public ReadOnlyMemory<byte> GetBytes()
        {
            return data;
        }

        public int CompareTo(IdBase? other)
        {
            if (other == null)
            {
                return -1;
            }
            else
            {
                return data.AsSpan().SequenceCompareTo(other.data.AsSpan());
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is IdBase id &&
                   data.AsSpan().SequenceEqual(id.data.AsSpan());
        }

        public override int GetHashCode()
        {
            return 42;
        }
    }
}
