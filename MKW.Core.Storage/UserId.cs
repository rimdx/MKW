namespace MKW.Core.Storage
{
    public record class UserId
    {
        private readonly Guid id;

        public bool IsAdmin => id == Guid.Empty;

        private UserId(Guid id)
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

        public static UserId Admin()
        {
            return new UserId(Guid.Empty);
        }

        public static UserId FromGuid(Guid id)
        {
            return new UserId(id);
        }

        public static UserId Create()
        {
            return new UserId(Guid.NewGuid());
        }
    }
}
