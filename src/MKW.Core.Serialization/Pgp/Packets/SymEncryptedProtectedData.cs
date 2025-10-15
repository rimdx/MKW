using MKW.Common;
using Org.BouncyCastle.Bcpg;

namespace MKW.Core.Serialization.Pgp.Packets
{
    public sealed class SymEncryptedProtectedData : PgpPacket
    {
        private const byte version = 0x02;

        public ReadOnlyMemory<byte> Salt { get; }
        public ReadOnlyMemory<byte> Data { get; }

        public override PacketTag Tag => PacketTag.SymmetricEncryptedIntegrityProtected;

        public SymEncryptedProtectedData(BcpgInputStream stream)
        {
            byte version = stream.RequireByte();
            byte cipherAlgorithmId = stream.RequireByte();
            byte aeadAlgorithmIdentifier = stream.RequireByte();
            byte chunkSize = stream.RequireByte();

            byte[] salt = new byte[32];
            stream.Read(salt);
            Salt = salt;

            Data = stream.ReadAll();
        }

        public SymEncryptedProtectedData(ReadOnlyMemory<byte> data, ReadOnlyMemory<byte> salt)
        {
            Data = data;
            Salt = salt;
        }

        public override void Encode(PgpOutputStream stream)
        {
            // - A 1-octet version number with value 2.
            // - A 1-octet cipher algorithm ID.
            // - A 1-octet AEAD algorithm identifier.
            // - A 1-octet chunk size.
            // - 32 octets of salt. The salt is used to derive the message key and MUST be securely generated (see Section 13.10).
            // - Encrypted data; that is, the output of the selected symmetric key cipher operating in the given AEAD mode.
            // - A final summary authentication tag for the AEAD mode.

            stream.WriteByte(0x01);

            //stream.WriteByte((byte)SymmetricKeyAlgorithmTag.Aes128);
            //stream.WriteByte((byte)AeadAlgorithmTag.Gcm);
            //stream.WriteByte(42); // chunk size

            int blockSize = 16;

            if (Salt.Length != blockSize)
            {
                throw new Exception($"Bad block size: {Salt.Length}");
            }

            stream.Write(Salt.Span);
            stream.Write(Data.Span);
        }
    }
}
