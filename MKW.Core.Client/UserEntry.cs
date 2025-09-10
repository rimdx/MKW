using MKW.Cryptography;
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

        protected UserEntry(ClientSession client,
                            ICryptographyProvider crypto,
                            UserSession user,
                            IDatabaseEntry entry,
                            IEntryAccessController accessController)
            : base(client, crypto, entry, accessController)
        {
            this.user = user;

            decoder = new EntryDecoder(crypto, user);
            sharer = new EntrySharer(accessController, decoder, encoder);
        }

        public static UserEntry Create(ClientSession client,
                                       ICryptographyProvider crypto,
                                       UserSession user,
                                       IDatabaseEntry entry)
        {
            AccessController accessController = AccessController.Create(client, user, entry);

            return new UserEntry(client,
                                 crypto,
                                 user,
                                 entry,
                                 accessController /* move */);
        }

        public static UserEntry Open(ClientSession client,
                                     ICryptographyProvider crypto,
                                     UserSession user,
                                     IDatabaseEntry entry)
        {
            AccessController accessController = AccessController.Open(client, entry);

            return new UserEntry(client,
                                 crypto,
                                 user,
                                 entry,
                                 accessController /* move */);
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
