namespace MKW.Core
{
    public record class SignedPayload(
        ReadOnlyMemory<byte> Payload,
        ReadOnlyMemory<byte> Signature
    );
}
