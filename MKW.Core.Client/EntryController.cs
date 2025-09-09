using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class EntryController : IEntryController, IDisposable
    {
        private readonly ClientSession client;
        private readonly ICryptographyProvider crypto;
        private readonly IDatabaseUser user;

        public EntryController(ClientSession client /* reference */,
                               ICryptographyProvider crypto,
                               IDatabaseUser user)
        {
            this.client = client;
            this.crypto = crypto;
            this.user = user;
        }

        private ITrustProvider OpenTrustProvider()
        {
            return new UserTrustProvider(client, crypto, user);
        }

        public IEntrySession OpenEntry(EntryId id)
        {
            IDatabaseEntry dbEntry = client.Database.OpenEntry(id, false);
            return Entry.Open(client, crypto, dbEntry);
        }

        public IEntrySession CreateEntry(EntryId id)
        {
            IDatabaseEntry dbEntry = client.Database.CreateEntry(id);
            dbEntry.Save();

            using ITrustProvider trustProvider = OpenTrustProvider();
            return Entry.Create(client, crypto, trustProvider, dbEntry);
        }

        public IEntrySession CreateEntry()
        {
            return CreateEntry(EntryId.Create());
        }

        public EntryInfo DeleteEntry(EntryId id)
        {
            client.Database.DeleteEntry(id);

            return new EntryInfo
            {
                Id = id,
                Action = ActionInfo.Deleted,
                EncodedForUsers = []
            };
        }

        public IEntrySession EnsureEntry(EntryId id, out bool created)
        {
            created = !client.Database.HasEntry(id);

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
            foreach (IDatabaseEntry entry in client.Database.EnumerateEntries())
            {
                yield return Entry.Open(client, crypto, entry);
            }
        }

        public void Dispose()
        {
        }
    }
}
