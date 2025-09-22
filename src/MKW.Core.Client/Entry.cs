using MKW.Core.Implementation;
using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Client
{
    internal class Entry
        : IEntrySession
        , IDisposable
    {
        protected readonly IDatabase database;
        protected readonly ICryptographyProvider crypto;
        protected readonly IDatabaseEntry entry;
        protected readonly AccessController accessController;
        protected readonly UserSession user;

        protected readonly EntryEncoder encoder;
        protected readonly EntryDecoder decoder;
        protected readonly EntrySharer sharer;

        public EntryId Id => entry.Id;

        protected Entry(IDatabase database,
                        ICryptographyProvider crypto,
                        UserSession user,
                        IDatabaseEntry entry,
                        AccessController accessController)
        {
            this.database = database;
            this.crypto = crypto;
            this.user = user;
            this.entry = entry;
            this.accessController = accessController;

            encoder = new EntryEncoder(crypto, database, accessController);
            decoder = new EntryDecoder(crypto, user, user.Transformer);
            sharer = new EntrySharer(accessController, decoder, encoder);
        }

        public static Entry Create(IDatabase database,
                                   ICryptographyProvider crypto,
                                   UserSession user,
                                   IDatabaseEntry entry)
        {
            AccessController accessController = AccessController.Create(user);

            return new Entry(database,
                             crypto,
                             user,
                             entry,
                             accessController /* move */);
        }

        public static Entry Open(IDatabase database,
                                 ICryptographyProvider crypto,
                                 UserSession user,
                                 IDatabaseEntry entry)
        {
            AccessController accessController = AccessController.Open(database, entry.Id);

            return new Entry(database,
                             crypto,
                             user,
                             entry,
                             accessController /* move */);
        }

        public EntryInfo UpdatePayload(EntryPayload payload)
        {
            encoder.EncodeEntry(entry, payload);

            entry.Save();

            return new EntryInfo
            {
                Id = entry.Id,
                EncodedForUsers = [.. accessController.EnumerateAccess()]
            };
        }

        public EntryPayload? OpenPayload()
        {
            return decoder.DecodeEntry(entry);
        }

        public IEnumerable<UserId> EnumerateAccess()
        {
            foreach (UserId user in accessController.EnumerateAccess())
            {
                yield return user;
            }
        }

        public void AddAccess(UserId userId)
        {
            sharer.ShareEntry(entry, userId);
            entry.Save();
        }

        public void Dispose()
        {
            accessController.Dispose();
        }
    }
}
