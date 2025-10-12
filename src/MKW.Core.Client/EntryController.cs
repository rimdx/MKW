using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Client
{
    internal sealed class EntryController : IEntryController, IDisposable
    {
        private readonly ICryptographyProvider crypto;
        private readonly IDatabase database;
        private readonly IUserSession user;
        private readonly IAsymmetricPrivateTransformer privateKey;

        public EntryController(ICryptographyProvider crypto,
                               IDatabase database,
                               IUserSession user,
                               IAsymmetricPrivateTransformer privateKey)
        {
            this.crypto = crypto;
            this.database = database;
            this.user = user;
            this.privateKey = privateKey;
        }

        public IEntrySession OpenEntry(EntryId id)
        {
            return Entry.Open(database, crypto, user, privateKey, id);
        }

        public IEntrySession CreateEntry(EntryId id)
        {
            return Entry.Create(database, crypto, user, privateKey, id);
        }

        public IEntrySession CreateEntry()
        {
            return CreateEntry(EntryId.Create());
        }

        public EntryInfo DeleteEntry(EntryId id)
        {
            database.DeleteEntry(id);

            return new EntryInfo
            {
                Id = id,
                EncodedForUsers = []
            };
        }

        public IEnumerable<IEntrySession> EnumerateEntries()
        {
            foreach (DatabaseEntry entry in database.EnumerateEntries())
            {
                yield return Entry.Open(database, crypto, user, privateKey, entry.Id);
            }
        }

        public void Dispose()
        {
        }
    }
}
