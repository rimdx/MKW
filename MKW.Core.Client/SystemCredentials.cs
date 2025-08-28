namespace MKW.Core.Client
{
    public record class SystemCredentials
    {
        public required ReadOnlyMemory<byte> Salt { get; set; }

        public required ReadOnlyMemory<byte> PublicKey { get; set; }

        // Encrypted using user's password
        public required ReadOnlyMemory<byte> PrivateKey { get; set; }
    }
}
