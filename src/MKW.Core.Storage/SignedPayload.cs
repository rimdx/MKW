namespace MKW.Core.Storage
{
    public record class SignedPayload
    {
        public required ReadOnlyMemory<byte> Payload { get; init; }
        public required ReadOnlyMemory<byte> Signature { get; init; }
    }
}
