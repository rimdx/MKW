namespace MKW.Core.Client
{
    public record class SystemCredentials
    {
        public required byte[] Salt { get; set; }

        public required byte[] PublicKey { get; set; }

        // Encrypted using user's password
        public required byte[] PrivateKey { get; set; }
    }
}
