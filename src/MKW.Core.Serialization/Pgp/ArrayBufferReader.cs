namespace MKW.Core.Serialization.Pgp
{
    public sealed class ArrayBufferReader
    {
        private ReadOnlyMemory<byte> Data;

        public ArrayBufferReader(ReadOnlyMemory<byte> data)
        {
            Data = data;
        }

        private void Advance(int count)
        {
            Data = Data.Slice(count);
        }

        public ReadOnlyMemory<byte> ReadBytes(int count)
        {
            ReadOnlyMemory<byte> slice = Data.Slice(0, count);
            Advance(count);
            return slice;
        }

        public ReadOnlyMemory<byte> ReadAll()
        {
            ReadOnlyMemory<byte> slice = Data;
            Advance(slice.Length);
            return slice;
        }

        public byte ReadByte()
        {
            var b = Data.Span[0];
            Advance(1);
            return b;
        }
    }
}
