using Org.BouncyCastle.Bcpg;

namespace MKW.Storage.MKGP
{
    public sealed class EntryObject : BcpgObject
    {
        private readonly DatabaseEntry entry;

        public EntryObject(BcpgInputStream stream)
        {
        }

        public EntryObject(DatabaseEntry entry)
        {
            this.entry = entry;
        }

        public override void Encode(BcpgOutputStream stream)
        {
            //AsymmetricKeyParameter pubkey = PublicKeyFactory.CreateKey(entry.Data);

            SymEncryptedProtectedData packet = new SymEncryptedProtectedData(entry.Data, entry.Salt);
            byte[] packetBytes = packet.GetEncoded();

            using BcpgOutputStream packetStream = new BcpgOutputStream(stream,
                                                                       PacketTag.SymmetricEncryptedIntegrityProtected,
                                                                       packetBytes.LongLength);

            stream.Write(packetBytes);
        }
    }
}
