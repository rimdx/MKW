namespace MKW.Core.Storage
{
    public record class SignedPayload(
        ReadOnlyMemory<byte> Payload,
        ReadOnlyMemory<byte> Signature
    );
}
