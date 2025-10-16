using MKW.Common;
using Org.BouncyCastle.Bcpg;

namespace MKW.Core.Serialization.Pgp.Packets
{
    public sealed class StringToKey : PgpObject
    {
        public StringToKeyTag StringToKeyTag { get; }
        public HashAlgorithmTag HashAlgorithmTag { get; }
        public ReadOnlyMemory<byte> Salt { get; }
        public byte Count { get; }

        public StringToKey(PgpInputStream stream)
        {
            StringToKeyTag = (StringToKeyTag)stream.RequireByte();

            if (StringToKeyTag == StringToKeyTag.Simple)
            {
                // Octet 0:        0x00
                // Octet 1:        hash algorithm

                HashAlgorithmTag = (HashAlgorithmTag)stream.RequireByte();
            }
            else if (StringToKeyTag == StringToKeyTag.Salted)
            {
                // Octet 0:        0x01
                // Octet 1:        hash algorithm
                // Octets 2-9:     8-octet salt value

                HashAlgorithmTag = (HashAlgorithmTag)stream.RequireByte();
                Salt = stream.ReadExact(8);
            }
            else if (StringToKeyTag == StringToKeyTag.IteratedSalted)
            {
                // Octet  0:        0x03
                // Octet  1:        hash algorithm
                // Octets 2-9:      8-octet salt value
                // Octet  10:       count, a one-octet, coded value

                HashAlgorithmTag = (HashAlgorithmTag)stream.RequireByte();
                Salt = stream.ReadExact(8);
                Count = stream.RequireByte();
            }
            else
            {
                throw new Exception($"Unknown S2K algorithm: {StringToKeyTag}");
            }
        }

        public override void Encode(PgpOutputStream stream)
        {
            if (StringToKeyTag == StringToKeyTag.Simple)
            {
                stream.WriteByte((byte)HashAlgorithmTag);
            }
            else if (StringToKeyTag == StringToKeyTag.Salted)
            {
                stream.WriteByte((byte)HashAlgorithmTag);
                stream.Write(Salt.Span.Slice(0, 8));
            }
            else if (StringToKeyTag == StringToKeyTag.IteratedSalted)
            {
                stream.WriteByte((byte)HashAlgorithmTag);
                stream.Write(Salt.Span.Slice(0, 8));
                stream.WriteByte(Count);
            }
            else
            {
                throw new Exception($"Unknown S2K algorithm: {StringToKeyTag}");
            }
        }
    }
}
