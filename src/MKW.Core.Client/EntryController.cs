using MKW.Core.Notify;
using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Client
{
    public class EntryController : IEntryController, IDisposable
    {
        private readonly IDatabase database;
        private readonly ICryptographyProvider crypto;
        private readonly IDatabaseUser user;

        public EntryController(IDatabase database /* reference */,
                               ICryptographyProvider crypto,
                               IDatabaseUser user)
        {
            this.database = database;
            this.crypto = crypto;
            this.user = user;
        }

        private ITrustProvider OpenTrustProvider()
        {
            // TODO: verify admin
            IDatabaseUser admin = database.OpenUser(UserId.Admin(), true);
            return new UserTrustProvider(database, crypto, admin);
        }

        public IEntrySession OpenEntry(EntryId id)
        {
            IDatabaseEntry dbEntry = database.OpenEntry(id, false);
            return Entry.Open(database, crypto, dbEntry);
        }

        public IEntrySession CreateEntry(EntryId id)
        {
            IDatabaseEntry dbEntry = database.CreateEntry(id);
            dbEntry.Save();

            using ITrustProvider trustProvider = OpenTrustProvider();
            return Entry.Create(database, crypto, trustProvider, dbEntry);
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
                Action = ActionInfo.Deleted,
                EncodedForUsers = []
            };
        }

        public IEntrySession EnsureEntry(EntryId id, out bool created)
        {
            created = !database.HasEntry(id);

            if (created)
            {
                return CreateEntry(id);
            }
            else
            {
                return OpenEntry(id);
            }
        }

        public EntryInfo UpdateEntry(EntryId id, EntryPayload? payload)
        {
            if (payload == null)
            {
                return DeleteEntry(id);
            }
            else
            {
                using IEntrySession entry = EnsureEntry(id, out bool created);

                EntryInfo notify = entry.UpdatePayload(payload);

                return new EntryInfo
                {
                    Id = notify.Id,
                    EncodedForUsers = notify.EncodedForUsers,
                    Action = created ? ActionInfo.Added : ActionInfo.Updated,
                };
            }
        }

        public IEnumerable<IEntrySession> EnumerateEntries()
        {
            foreach (IDatabaseEntry entry in database.EnumerateEntries())
            {
                yield return Entry.Open(database, crypto, entry);
            }
        }

        public void Dispose()
        {
        }
    }
}
