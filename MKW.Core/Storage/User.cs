namespace MKW.Core.Storage
{
    public record class User
    {
        public required Guid Id { get; set; }

        public required byte[] Salt { get; set; }

        public required byte[] PublicKey { get; set; }

        public required byte[] EncryptedPrivateKey { get; set; }
    }
}
