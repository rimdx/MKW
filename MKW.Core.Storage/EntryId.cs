namespace MKW.Core.Storage
{
    public record class EntryId
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

        public Guid GetGuid()
        {
            return id;
        }

        public static EntryId FromGuid(Guid id)
        {
            return new EntryId(id);
        }

        public static EntryId Create()
        {
            return new EntryId(Guid.NewGuid());
        }
    }
}
