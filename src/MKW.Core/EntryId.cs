namespace MKW.Core
{
    public record class EntryId : IComparable<EntryId>
    {
        private readonly Guid id;

        private EntryId(Guid id)
        {
            this.id = id;
        }

        public override string ToString()
        {
            return id.ToString();
        }

        public ReadOnlyMemory<byte> GetBytes()
        {
            return id.ToByteArray();
        }

        public string GetString()
        {
            return id.ToString();
        }

        public static EntryId FromBytes(ReadOnlySpan<byte> id)
        {
            return new EntryId(new Guid(id.ToArray()));
        }

        public static EntryId FromString(string str)
        {
            return new EntryId(new Guid(str));
        }

        public static EntryId Create()
        {
            return new EntryId(Guid.NewGuid());
        }

        public int CompareTo(EntryId? other)
        {
            return id.CompareTo(other?.id);
        }
    }
}
