using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class UserSession : IDisposable
    {
        protected readonly ClientSession client;

        internal IDatabaseUser DatabaseUser { get; }

        public UserId Id => DatabaseUser.Id;
        public AsymmetricTransformer Transformer { get; }

        public UserSession(ClientSession client /* reference */,
                           IDatabaseUser user /* reference */,
                           ReadOnlySpan<byte> privateKey)
        {
            this.client = client;
            DatabaseUser = user;

            Transformer = AsymmetricTransformer.Open(user.PublicKey.Span, privateKey);
        }

        public UserEntry OpenEntry(EntryId id)
        {
            IDatabaseEntry dbEntry = client.Database.OpenEntry(id, false);
            return new UserEntry(client, this, dbEntry);
        }

        public UserEntry CreateEntry(EntryId id)
        {
            IDatabaseEntry dbEntry = client.Database.CreateEntry(id);
            dbEntry.Save();
            return new UserEntry(client, this, dbEntry);
        }

        public UserEntry CreateEntry() => CreateEntry(EntryId.Create());

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

        public UserEntry EnsureEntry(EntryId id, out bool created)
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

        public IEnumerable<UserEntry> EnumerateEntries()
        {
            foreach (IDatabaseEntry entry in client.Database.EnumerateEntries())
            {
                yield return new UserEntry(client, this, entry);
            }
        }

        public void Dispose()
        {
            Transformer.Dispose();
        }

        public void UpdateTrust(UserId userId, Trust trust)
        {
            using UserTrustController trustController = new UserTrustController(client, this);

            trustController.UpdateTrust(userId, trust);
        }
    }
}
