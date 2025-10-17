namespace MKW.Core.Serialization.Pgp
{
    public sealed record class PgpArmouredMessage
    {
        public required string MessageTypeHeader { get; init; }

        public required IReadOnlyCollection<string> Headers { get; init; }

        public required ReadOnlyMemory<byte> Data { get; init; }
    }
}
