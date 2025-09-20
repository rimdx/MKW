using MKW.Core.Notify;
using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Client
{
    public class UserSession
        : IUserSession
        , IEntryController
        , ITrustProvider
        , ITrustController
        , IDisposable
    {
        protected readonly ClientSession client;
        protected readonly ICryptographyProvider crypto;
        protected readonly IDatabase database;
        protected readonly IEntryController entryController;
        protected readonly UserMetadataDecoder metadata;

        internal IDatabaseUser DatabaseUser { get; }

        public UserId Id => DatabaseUser.Id;
        public IAsymmetricPrivateTransformer Transformer { get; }
        public UserTrustController TrustController { get; }

        private readonly UserAccessController accessController;

        public UserSession(ClientSession client /* reference */,
                           ICryptographyProvider crypto,
                           IDatabase database /* reference */,
                           IDatabaseUser user /* reference */,
                           ReadOnlySpan<byte> privateKey)
        {
            this.client = client;
            this.crypto = crypto;
            this.database = database;
            DatabaseUser = user;

            entryController = new UserEntryController(client, crypto, database, this);
            Transformer = crypto.OpenAsymmetricTransformer(user.PublicKey.Span, privateKey);
            TrustController = new UserTrustController(database, crypto, user, Transformer);
            accessController = new UserAccessController(this);
            metadata = new UserMetadataDecoder(user);
        }

        public UserMetadata OpenMetadata()
        {
            return metadata.OpenMetadata();
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

        // ITrustProvider

        public IEnumerable<UserInfo> EnumerateImplicitlyTrustedUsers()
        {
            foreach (UserInfo user in TrustController.EnumerateImplicitlyTrustedUsers())
            {
                yield return user;
            }
        }

        public IEnumerable<UserInfo> EnumerateExplicitlyTrustedUsers()
        {
            foreach (UserInfo user in TrustController.EnumerateExplicitlyTrustedUsers())
            {
                yield return user;
            }
        }

        public Trust GetExplicitTrust(ReadOnlySpan<byte> publicKey)
        {
            return TrustController.GetExplicitTrust(publicKey);
        }

        public Trust GetImplicitTrust(UserId userId)
        {
            return TrustController.GetImplicitTrust(userId);
        }

        // ITrustController


        public void AddTrust(UserId userId)
        {
            TrustController.AddTrust(userId);
            accessController.AddAccess(userId);
        }

        public void RemoveTrust(UserId userId)
        {
            TrustController.RemoveTrust(userId);
        }

        public void Dispose()
        {
            Transformer.Dispose();
            TrustController.Dispose();
            entryController.Dispose();
        }
    }
}
