namespace MKW.Core.Storage
{
    public record class SecretPayload(
        ReadOnlyMemory<byte> EncryptedPayload
    );
}
