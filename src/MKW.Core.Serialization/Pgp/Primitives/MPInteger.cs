using MKW.Common;
using Org.BouncyCastle.Math;
using System.Buffers.Binary;

namespace MKW.Core.Serialization.Pgp.Primitives
{
    public sealed class MPInteger : PgpObject
    {
        public MPInteger(PgpInputStream stream)
        {
            ushort lengthInBits = BinaryPrimitives.ReadUInt16BigEndian(stream.ReadExact(2).Span);
            int lengthInBytes = (lengthInBits + 7) / 8;

            ReadOnlyMemory<byte> bytes = stream.ReadExact(lengthInBytes);
            Value = new BigInteger(1, bytes.ToArray());
        }

        public MPInteger(BigInteger value)
        {
            if (value.SignValue < 0)
            {
                throw new ArgumentException("Values must be positive", nameof(value));
            }

            Value = value;
        }

        public BigInteger Value { get; }

        public override void Encode(PgpOutputStream stream)
        {
            Span<byte> buf = stackalloc byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(buf, (ushort)Value.BitLength);
            stream.Write(buf);

            stream.Write(Value.ToByteArrayUnsigned());
        }
    }
}
