using MKW.Core.Cryptography;

namespace MKW.Core.Client
{
    public class EntryPayload
    {
        public ReadOnlyMemory<byte> Data { get; }

        public EntryPayload(ReadOnlyMemory<byte> data)
        {
            Data = data;
        }

        public EntryPayload(string data)
        {
            Data = EncodingConverter.GetBytes(data);
        }

        public override string ToString()
        {
            return EncodingConverter.GetString(Data.Span);
        }

        // Not for production use. Probably...
        public override bool Equals(object? obj)
        {
            return obj is EntryPayload other && Data.Span.SequenceEqual(other.Data.Span);
        }

        public override int GetHashCode()
        {
            return Data.GetHashCode();
        }
    }
}
