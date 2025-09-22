namespace MKW.Core
{
    public record class UserAccessRequest
    {
        /* plain */
        public required ReadOnlyMemory<byte> Salt { get; init; }
        public required ReadOnlyMemory<byte> PublicKey { get; init; }

        /* encrypted */
        public required ReadOnlyMemory<byte> EncryptedPrivateKey { get; init; }

        public required ReadOnlyMemory<byte> AdminSignature { get; init; }
    }
}
