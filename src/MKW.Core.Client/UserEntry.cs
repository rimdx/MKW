using MKW.Core.Implementation;
using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Client
{
    public class UserEntry
        : Entry
        , IEntrySession
        , IDisposable
    {
        protected readonly UserSession user;
        protected readonly EntryDecoder decoder;
        protected readonly EntrySharer sharer;

        protected UserEntry(IDatabase database,
                            ICryptographyProvider crypto,
                            UserSession user,
                            IDatabaseEntry entry,
                            AccessController accessController)
            : base(database, crypto, entry, accessController)
        {
            this.user = user;

            decoder = new EntryDecoder(crypto, user, user.Transformer);
            sharer = new EntrySharer(accessController, decoder, encoder);
        }

        public static UserEntry Create(IDatabase database,
                                       ICryptographyProvider crypto,
                                       UserSession user,
                                       IDatabaseEntry entry)
        {
            AccessController accessController = AccessController.Create(database, user, entry);

            return new UserEntry(database,
                                 crypto,
                                 user,
                                 entry,
                                 accessController /* move */);
        }

        public static UserEntry Open(IDatabase database,
                                     ICryptographyProvider crypto,
                                     UserSession user,
                                     IDatabaseEntry entry)
        {
            AccessController accessController = AccessController.Open(database, entry);

            return new UserEntry(database,
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
