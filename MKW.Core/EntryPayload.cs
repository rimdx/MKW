using System.Text;

namespace MKW.Core
{
    public record class EntryPayload
    {
        public byte[] Data { get; }

        public EntryPayload(byte[] data)
        {
            Data = data;
        }

        public EntryPayload(string data)
        {
            Data = Encoding.Unicode.GetBytes(data);
        }

        public override string ToString()
        {
            return Encoding.Unicode.GetString(Data);
        }
    }
}
