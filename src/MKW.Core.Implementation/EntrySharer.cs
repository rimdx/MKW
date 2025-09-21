using MKW.Core.Storage;

namespace MKW.Core.Implementation
{
    public class EntrySharer : IDisposable
    {
        private readonly AccessController accessController;
        private readonly EntryDecoder decoder;
        private readonly EntryEncoder encoder;

        public EntrySharer(AccessController accessController,
                           EntryDecoder decoder,
                           EntryEncoder encoder)
        {
            this.accessController = accessController;
            this.decoder = decoder;
            this.encoder = encoder;
        }

        public void ShareEntry(IDatabaseEntry entry, UserId userId)
        {
            EntryPayload? payload = decoder.DecodeEntry(entry);

            if (payload == null)
            {
                throw new Exception("The entry is not encrypted for this user.");
            }

            accessController.AddAccess(userId);
            encoder.EncodeEntry(entry, payload);
        }

        public void Dispose()
        {
        }
    }
}
