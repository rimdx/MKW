using MKW.Core.Cryptography;
using System.Text;

namespace MKW.Core.Client
{
    public class EntryPayload
    {
        public Memory<byte> Data { get; }

        public EntryPayload(Memory<byte> data)
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
            return HashCode.Combine(Data);
        }
    }
}
