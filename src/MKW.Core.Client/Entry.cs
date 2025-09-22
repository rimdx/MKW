using MKW.Core.Implementation;
using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Client
{
    internal class Entry
        : IEntrySession
        , IDisposable
    {
        private readonly IDatabase database;
        private readonly EntryId entryId;
        private readonly AccessController accessController;

        private readonly EntryEncoder encoder;
        private readonly EntryDecoder decoder;
        private readonly EntrySharer sharer;

        public EntryId Id => entryId;

        protected Entry(IDatabase database,
                        ICryptographyProvider crypto,
                        IUserSession user,
                        IAsymmetricPrivateTransformer privateKey,
                        EntryId entryId,
                        AccessController accessController)
        {
            this.database = database;
            this.entryId = entryId;
            this.accessController = accessController;

            encoder = new EntryEncoder(crypto, database, accessController);
            decoder = new EntryDecoder(crypto, user, privateKey);
            sharer = new EntrySharer(accessController, decoder, encoder);
        }

        public static Entry Create(IDatabase database,
                                   ICryptographyProvider crypto,
                                   IUserSession user,
                                   IAsymmetricPrivateTransformer privateKey,
                                   EntryId entryId)
        {
            AccessController accessController = AccessController.Create(user);

            return new Entry(database,
                             crypto,
                             user,
                             privateKey,
                             entryId,
                             accessController /* move */);
        }

        public static Entry Open(IDatabase database,
                                 ICryptographyProvider crypto,
                                 IUserSession user,
                                 IAsymmetricPrivateTransformer privateKey,
                                 EntryId entryId)
        {
            IDatabaseEntry entry = database.OpenEntry(entryId, true);

            AccessController accessController = AccessController.Open(entry);

            return new Entry(database,
                             crypto,
                             user,
                             privateKey,
                             entryId,
                             accessController /* move */);
        }

        public EntryInfo UpdatePayload(EntryPayload payload)
        {
            IDatabaseEntry entry = database.OpenEntry(entryId, false);

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
            IDatabaseEntry entry = database.OpenEntry(entryId, true);
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
            IDatabaseEntry entry = database.OpenEntry(entryId, false);

            sharer.ShareEntry(entry, userId);

            entry.Save();
        }

        public void Dispose()
        {
            accessController.Dispose();
        }
    }
}
