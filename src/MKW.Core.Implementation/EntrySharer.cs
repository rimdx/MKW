using MKW.Core.Storage;

namespace MKW.Core.Implementation
{
    public class EntrySharer : IDisposable
    {
        private readonly ITrustProvider trustProvider;
        private readonly EntryDecoder decoder;
        private readonly EntryEncoder encoder;

        public EntrySharer(ITrustProvider trustProvider,
                           EntryDecoder decoder,
                           EntryEncoder encoder)
        {
            this.trustProvider = trustProvider;
            this.decoder = decoder;
            this.encoder = encoder;
        }

        public DatabaseEntry ShareEntry(DatabaseEntry entry)
        {
            EntryPayload? payload = decoder.DecodeEntry(entry);

            if (payload == null)
            {
                throw new Exception("The entry is not encrypted for this user.");
            }

            return encoder.EncodeEntry(entry, payload);
        }

        public void Dispose()
        {
        }
    }
}
