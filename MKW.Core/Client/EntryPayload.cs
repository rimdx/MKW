using MKW.Core.Cryptography;
using System.Text;

namespace MKW.Core.Client
{
    public class EntryPayload
    {
        public byte[] Data { get; }

        public EntryPayload(byte[] data)
        {
            Data = data;
        }

        public EntryPayload(string data)
        {
            Data = EncodingConverter.GetBytes(data);
        }

        public override string ToString()
        {
            return EncodingConverter.GetString(Data);
        }

        // Not for production use. Probably...
        public override bool Equals(object? obj)
        {
            return obj is EntryPayload payload && Enumerable.SequenceEqual(Data, payload.Data);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Data);
        }
    }
}
