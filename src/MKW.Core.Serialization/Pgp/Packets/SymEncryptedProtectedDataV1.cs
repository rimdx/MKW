using MKW.Common;
using MKW.Core.Serialization.Pgp.Primitives;
using Org.BouncyCastle.Bcpg;

namespace MKW.Core.Serialization.Pgp.Packets
{
    // - A one-octet version number.  The only currently defined value is 1.
    //
    // - Encrypted data, the output of the selected symmetric-key cipher
    //   operating in Cipher Feedback mode with shift amount equal to the
    //   block size of the cipher (CFB-n where n is the block size).
    public sealed class SymEncryptedProtectedDataV1 : PgpPacket
    {
        private static readonly PgpVersion version = new PgpVersion(1);

        public override PacketTag Tag => PacketTag.SymmetricEncryptedIntegrityProtected;

        public ReadOnlyMemory<byte> Data { get; }

        public SymEncryptedProtectedDataV1(PgpInputStream stream)
        {
            version.ConsumeVersion(stream);
            Data = stream.ReadAll();
        }

        public SymEncryptedProtectedDataV1(ReadOnlyMemory<byte> data)
        {
            Data = data;
        }

        public override void Encode(PgpOutputStream stream)
        {
            version.Encode(stream);
            stream.Write(Data.Span);
        }
    }
}
