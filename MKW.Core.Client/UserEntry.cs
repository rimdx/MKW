using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class UserEntry
        : Entry
        , IEntrySession
        , IEntryAccessController
        , IDisposable
    {
        protected readonly UserSession user;
        protected readonly EntryDecoder decoder;
        protected readonly EntrySharer sharer;

        public UserEntry(ClientSession client,
                         ICryptographyProvider crypto,
                         UserSession user,
                         IDatabaseEntry entry)
            : base(client, crypto, user.TrustController, entry)
        {
            this.user = user;
            decoder = new EntryDecoder(crypto, user);
            sharer = new EntrySharer(accessController, decoder, encoder);
        }

        public override EntryPayload? OpenPayload()
        {
            return decoder.DecodeEntry(entry);
        }

        public override void AddAccess(UserId userId)
        {
            sharer.ShareEntry(entry, userId);
            entry.Save();
        }
    }
}
