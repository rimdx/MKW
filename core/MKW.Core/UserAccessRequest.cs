namespace MKW.Core
{
    public record class UserAccessRequest
    {
        /* plain */
        public ReadOnlyMemory<byte> Salt { get; init; }
        public ReadOnlyMemory<byte> PublicKey { get; init; }

        /* encrypted */
        public ReadOnlyMemory<byte> EncryptedPrivateKey { get; init; }

        public ReadOnlyMemory<byte> AdminSignature { get; init; }
    }
}
