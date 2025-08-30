using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class UserSession : IUserSession, IEntryController, IDisposable
    {
        protected readonly ClientSession client;
        protected readonly IDatabase database;
        protected readonly IEntryController entryController;

        internal IDatabaseUser DatabaseUser { get; }

        public UserId Id => DatabaseUser.Id;
        public IAsymmetricPrivateTransformer Transformer { get; }
        public UserTrustController TrustController { get; }

        public UserSession(ClientSession client /* reference */,
                           IDatabase database /* reference */,
                           IDatabaseUser user /* reference */,
                           ReadOnlySpan<byte> privateKey)
        {
            this.client = client;
            this.database = database;
            DatabaseUser = user;
            entryController = new UserEntryController(client, database, this);

            Transformer = AsymmetricTransformer.Open(user.PublicKey.Span, privateKey);
            TrustController = new UserTrustController(client, this);
        }

        // IEntryController

        public IEntrySession OpenEntry(EntryId id)
        {
            return entryController.OpenEntry(id);
        }

        public IEntrySession CreateEntry(EntryId id)
        {
            return entryController.CreateEntry(id);
        }

        public IEntrySession CreateEntry()
        {
            return entryController.CreateEntry();
        }

        public EntryInfo DeleteEntry(EntryId id)
        {
            return entryController.DeleteEntry(id);
        }

        public EntryInfo UpdateEntry(EntryId id, EntryPayload? payload)
        {
            return entryController.UpdateEntry(id, payload);
        }

        public IEnumerable<IEntrySession> EnumerateEntries()
        {
            foreach (IEntrySession entry in entryController.EnumerateEntries())
            {
                yield return entry;
            }
        }

        // todo: ITrustProvider

        public void UpdateTrust(UserId userId, Trust trust)
        {
            TrustController.UpdateTrust(userId, trust);
        }

        public IEnumerable<UserInfo> EnumerateUsersTrust()
        {
            return TrustController.EnumerateImplicitlyTrustedUsers();
        }

        public void Dispose()
        {
            Transformer.Dispose();
            TrustController.Dispose();
            entryController.Dispose();
        }
    }
}
