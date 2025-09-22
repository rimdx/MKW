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

            DatabaseEntry entry = new DatabaseEntry
            {
                Id = entryId,
                Data = ReadOnlyMemory<byte>.Empty,
                Salt = ReadOnlyMemory<byte>.Empty,
                Keys = new Dictionary<UserId, ReadOnlyMemory<byte>>(),
            };

            database.CreateEntry(entryId, entry);

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
            DatabaseEntry entry = database.OpenEntry(entryId);

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
            DatabaseEntry entry = database.OpenEntry(entryId);

            DatabaseEntry newEntry = encoder.EncodeEntry(entry, payload);

            database.UpdateEntry(Id, newEntry);

            return new EntryInfo
            {
                Id = newEntry.Id,
                EncodedForUsers = [.. accessController.EnumerateAccess()]
            };
        }

        public EntryPayload? OpenPayload()
        {
            DatabaseEntry entry = database.OpenEntry(entryId);
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
            DatabaseEntry entry = database.OpenEntry(entryId);

            DatabaseEntry newEntry = sharer.ShareEntry(entry, userId);

            database.UpdateEntry(entryId, newEntry);
        }

        public void Dispose()
        {
            accessController.Dispose();
        }
    }
}
