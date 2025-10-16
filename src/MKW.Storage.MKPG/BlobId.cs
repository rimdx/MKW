using MKW.Core;

namespace MKW.Storage.MKPG
{
    internal sealed record class BlobId : IComparable<BlobId>
    {
        private readonly Guid id;

        private BlobId(Guid id)
        {
            this.id = id;
        }

        public override string ToString()
        {
            return id.ToString();
        }

        public Guid GetGuid()
        {
            return id;
        }

        public static BlobId From(Guid id)
        {
            return new BlobId(id);
        }

        public static BlobId From(EntryId entryId)
        {
            return new BlobId(entryId.GetGuid());
        }

        public static BlobId From(UserId userId)
        {
            return new BlobId(userId.GetGuid());
        }

        public static BlobId Create()
        {
            return new BlobId(Guid.NewGuid());
        }

        public int CompareTo(BlobId? other)
        {
            return id.CompareTo(other?.id);
        }
    }
}
