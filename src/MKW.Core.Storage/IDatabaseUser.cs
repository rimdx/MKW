namespace MKW.Core.Storage
{
    public interface IDatabaseUser : ISavable
    {
        UserId Id { get; }

        ReadOnlyMemory<byte> Salt { get; set; }

        SignedPayload PublicKey { get; set; }
        SecretPayload PrivateKey { get; set; }

        SignedPayload Metadata { get; set; }

        ReadOnlyMemory<byte> AdminSignature { get; set; }
    }
}
