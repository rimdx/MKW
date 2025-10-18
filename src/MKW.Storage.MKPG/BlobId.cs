using MKW.Core;

namespace MKW.Storage.MKPG
{
    internal sealed class BlobId : IComparable<BlobId>
    {
        private readonly ReadOnlyMemory<byte> data;

        private BlobId(ReadOnlyMemory<byte> data)
        {
            this.data = data;
        }

        public override string ToString()
        {
            return data.ToString();
        }

        public ReadOnlyMemory<byte> GetBytes()
        {
            return data;
        }

        public static BlobId From(Guid id)
        {
            return new BlobId(id.ToByteArray());
        }

        public static BlobId From(ReadOnlyMemory<byte> data)
        {
            return new BlobId(data);
        }

        public static BlobId From(EntryId entryId)
        {
            return new BlobId(entryId.GetBytes());
        }

        public static BlobId From(UserId userId)
        {
            return new BlobId(userId.GetGuid().ToByteArray());
        }

        public static BlobId Create()
        {
            return new BlobId(Guid.NewGuid().ToByteArray());
        }

        public int CompareTo(BlobId? other)
        {
            if (other == null)
            {
                return -1;
            }
            else
            {
                return data.Span.SequenceCompareTo(other.data.Span);
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is BlobId id &&
                   data.Span.SequenceEqual(id.data.Span);
        }

        public override int GetHashCode()
        {
            return 42;
        }
    }
}
