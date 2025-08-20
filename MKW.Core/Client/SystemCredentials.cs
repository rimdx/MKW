namespace MKW.Core.Client
{
    public record class SystemCredentials
    {
        public required Memory<byte> Salt { get; set; }

        public required Memory<byte> PublicKey { get; set; }

        // Encrypted using user's password
        public required Memory<byte> PrivateKey { get; set; }
    }
}
