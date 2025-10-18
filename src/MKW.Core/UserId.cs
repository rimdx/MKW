namespace MKW.Core
{
    public sealed class UserId : IdBase
    {
        public const int Size = 16;

        public bool IsAdmin => Equals(Admin());

        private UserId(ReadOnlyMemory<byte> data)
            : base(data.ToArray(), Size)
        {
        }

        public override string ToString()
        {
            return new Guid(data.ToArray()).ToString();
        }

        public Guid GetGuid()
        {
            return new Guid(data.ToArray());
        }

        public static UserId Admin()
        {
            return new UserId(new byte[Size]);
        }

        public static UserId FromGuid(Guid id)
        {
            return new UserId(id.ToByteArray());
        }

        public static UserId Create()
        {
            return new UserId(Guid.NewGuid().ToByteArray());
        }
    }
}
