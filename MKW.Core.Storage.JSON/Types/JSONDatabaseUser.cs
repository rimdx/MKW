namespace MKW.Core.Storage.JSON.Types
{
    public record class JSONDatabaseUser
    {
        public required ReadOnlyMemory<byte> Salt { get; set; }

        public required ReadOnlyMemory<byte> PublicKey { get; set; }

        public required ReadOnlyMemory<byte> PrivateKey { get; set; }
    }
}
