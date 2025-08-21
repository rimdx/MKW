namespace MKW.Core.Storage.JSON.Types
{
    public record class JSONDatabaseUser
    {
        public required ReadOnlyMemory<byte> Salt { get; init; }

        public required ReadOnlyMemory<byte> PublicKey { get; init; }

        public required ReadOnlyMemory<byte> PrivateKey { get; init; }
    }
}
