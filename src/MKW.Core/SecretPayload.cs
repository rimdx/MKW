namespace MKW.Core
{
    public record class SecretPayload(
        ReadOnlyMemory<byte> EncryptedPayload
    );
}
