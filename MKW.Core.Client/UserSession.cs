using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class UserSession : IEntryController, IDisposable
    {
        protected readonly ClientSession client;
        protected readonly IDatabase database;

        internal IDatabaseUser DatabaseUser { get; }

        public UserId Id => DatabaseUser.Id;
        public AsymmetricTransformer Transformer { get; }
        public UserTrustController TrustController { get; }

        public UserSession(ClientSession client /* reference */,
                           IDatabase database /* reference */,
                           IDatabaseUser user /* reference */,
                           ReadOnlySpan<byte> privateKey)
        {
            this.client = client;
            this.database = database;
            DatabaseUser = user;

            Transformer = AsymmetricTransformer.Open(user.PublicKey.Span, privateKey);
            TrustController = new UserTrustController(client, this);
        }

        public IEntrySession OpenEntry(EntryId id)
        {
            IDatabaseEntry dbEntry = database.OpenEntry(id, false);
            return new UserEntry(client, this, dbEntry);
        }

        public IEntrySession CreateEntry(EntryId id)
        {
            IDatabaseEntry dbEntry = database.CreateEntry(id);
            dbEntry.Save();
            return new UserEntry(client, this, dbEntry);
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
                yield return new UserEntry(client, this, entry);
            }
        }

        public void Dispose()
        {
            Transformer.Dispose();
            TrustController.Dispose();
        }

        public void UpdateTrust(UserId userId, Trust trust)
        {
            TrustController.UpdateTrust(userId, trust);
        }

        public IEnumerable<UserInfo> EnumerateUsersTrust()
        {
            return TrustController.EnumerateImplicitlyTrustedUsers();
        }
    }
}
