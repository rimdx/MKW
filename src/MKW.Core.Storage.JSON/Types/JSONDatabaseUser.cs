namespace MKW.Core.Storage.JSON.Types
{
    internal record class JSONDatabaseUser
    {
        public required ReadOnlyMemory<byte> Salt { get; init; }

        public required ReadOnlyMemory<byte> PublicKey { get; init; }

        public required ReadOnlyMemory<byte> PrivateKey { get; init; }

        public required ReadOnlyMemory<byte> Metadata { get; init; }
        public required ReadOnlyMemory<byte> MetadataAdminSignature { get; init; }

        public required ReadOnlyMemory<byte> AdminTrustSignature { get; init; }
        public required ReadOnlyMemory<byte> AdminSignature { get; init; }
    }
}
