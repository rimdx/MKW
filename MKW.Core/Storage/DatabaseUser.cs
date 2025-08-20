namespace MKW.Core.Storage
{
    public record class DatabaseUser
    {
        public required Guid Id { get; set; }

        public required Memory<byte> Salt { get; set; }

        public required Memory<byte> PublicKey { get; set; }

        public required Memory<byte> PrivateKey { get; set; }
    }
}
