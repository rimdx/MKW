using MKW.Core.Notify;
using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Client
{
    public class UserEntryController : IEntryController, IDisposable
    {
        private readonly ClientSession client;
        private readonly ICryptographyProvider crypto;
        private readonly IDatabase database;
        private readonly UserSession user;

        public UserEntryController(ClientSession client /* reference */,
                                   ICryptographyProvider crypto,
                                   IDatabase database /* reference */,
                                   UserSession user /* reference */)
        {
            this.client = client;
            this.crypto = crypto;
            this.database = database;
            this.user = user;
        }

        public IEntrySession OpenEntry(EntryId id)
        {
            IDatabaseEntry dbEntry = database.OpenEntry(id, false);
            return UserEntry.Open(database, crypto, user, dbEntry);
        }

        public IEntrySession CreateEntry(EntryId id)
        {
            IDatabaseEntry dbEntry = database.CreateEntry(id);
            dbEntry.Save();
            return UserEntry.Create(database, crypto, user, dbEntry);
        }

        public IEntrySession CreateEntry() => CreateEntry(EntryId.Create());

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
                yield return UserEntry.Open(database, crypto, user, entry);
            }
        }

        public void Dispose()
        {
        }
    }
}
