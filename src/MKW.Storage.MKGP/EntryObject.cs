using MKW.Core.Serialization.Pgp;
using MKW.Core.Serialization.Pgp.Packets;

namespace MKW.Storage.MKGP
{
    public sealed class EntryObject : PgpObject
    {
        private readonly DatabaseEntry entry;

        public EntryObject(PgpInputStream stream)
        {
        }

        public EntryObject(DatabaseEntry entry)
        {
            this.entry = entry;
        }

        public override void Encode(PgpOutputStream stream)
        {
            //AsymmetricKeyParameter pubkey = PublicKeyFactory.CreateKey(entry.Data);

            stream.WritePacket(new SymEncryptedProtectedDataV1(entry.Data));
        }
    }
}
