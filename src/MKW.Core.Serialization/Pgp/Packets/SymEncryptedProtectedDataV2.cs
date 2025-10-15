using MKW.Common;
using MKW.Core.Serialization.Pgp.Primitives;
using Org.BouncyCastle.Bcpg;

namespace MKW.Core.Serialization.Pgp.Packets
{
    // - A 1-octet version number with value 2.
    // - A 1-octet cipher algorithm ID.
    // - A 1-octet AEAD algorithm identifier.
    // - A 1-octet chunk size.
    // - 32 octets of salt. The salt is used to derive the message key and MUST be securely generated (see Section 13.10).
    // - Encrypted data; that is, the output of the selected symmetric key cipher operating in the given AEAD mode.
    // - A final summary authentication tag for the AEAD mode.
    public sealed class SymEncryptedProtectedDataV2 : PgpPacket
    {
        private static readonly PgpVersion version = new PgpVersion(2);

        public override PacketTag Tag => PacketTag.SymmetricEncryptedIntegrityProtected;

        public SymmetricKeyAlgorithmTag CipherAlgorithmTag { get; }
        public AeadAlgorithmTag AlgorithmTag { get; }
        public byte ChunkSize { get; }
        public ReadOnlyMemory<byte> Data { get; }
        public ReadOnlyMemory<byte> Salt { get; }

        public SymEncryptedProtectedDataV2(PgpInputStream stream)
        {
            version.ConsumeVersion(stream);

            CipherAlgorithmTag = (SymmetricKeyAlgorithmTag)stream.RequireByte();
            AlgorithmTag = (AeadAlgorithmTag)stream.RequireByte();
            ChunkSize = stream.RequireByte();

            Salt = stream.ReadExact(32);
            Data = stream.ReadAll();
        }

        public SymEncryptedProtectedDataV2(SymmetricKeyAlgorithmTag cipherAlgorithmTag,
                                           AeadAlgorithmTag algorithmTag,
                                           byte chunkSize,
                                           ReadOnlyMemory<byte> data,
                                           ReadOnlyMemory<byte> salt)
        {
            CipherAlgorithmTag = cipherAlgorithmTag;
            AlgorithmTag = algorithmTag;
            ChunkSize = chunkSize;
            Data = data;
            Salt = salt;
        }

        public override void Encode(PgpOutputStream stream)
        {
            version.Encode(stream);

            stream.WriteByte((byte)CipherAlgorithmTag);
            stream.WriteByte((byte)AlgorithmTag);
            stream.WriteByte(ChunkSize);

            if (Salt.Length != 32)
            {
                throw new Exception($"Salt must be 32 bytes.");
            }

            stream.Write(Salt.Span);
            stream.Write(Data.Span);
        }
    }
}
