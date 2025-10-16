using MKW.Common;
using MKW.Core.Serialization.Pgp.Primitives;
using Org.BouncyCastle.Bcpg;

namespace MKW.Core.Serialization.Pgp.Packets
{
    // The body of this packet consists of:
    // 
    // - A one-octet version number.  The only currently defined version
    //   is 4.
    // 
    // - A one-octet number describing the symmetric algorithm used.
    // 
    // - A string-to-key (S2K) specifier, length as defined above.
    // 
    // - Optionally, the encrypted session key itself, which is decrypted
    //   with the string-to-key object.
    public sealed class SymEncryptedSessionKeyV4 : PgpPacket
    {
        private static readonly PgpVersion version = new PgpVersion(4);

        public override PacketTag Tag => PacketTag.SymmetricKeyEncryptedSessionKey;

        public SymmetricKeyAlgorithmTag SymmetricAlgorithmTag { get; }
        public StringToKey StringToKey { get; }

        public SymEncryptedSessionKeyV4(PgpInputStream stream)
        {
            version.ConsumeVersion(stream);
            SymmetricAlgorithmTag = (SymmetricKeyAlgorithmTag)stream.RequireByte();
            StringToKey = new StringToKey(stream);
        }

        public override void Encode(PgpOutputStream stream)
        {
            version.Encode(stream);
            stream.WriteByte((byte)SymmetricAlgorithmTag);
            StringToKey.Encode(stream);
        }
    }
}
