using MKW.Core.Implementation;
using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Client
{
    internal class UserSession
        : IUserSession
        , IEntryController
        , ITrustProvider
        , IDisposable
    {
        protected readonly ICryptographyProvider crypto;
        protected readonly IDatabase database;
        protected readonly IEntryController entryController;
        protected readonly UserMetadataDecoder metadata;
        protected readonly IAsymmetricPublicTransformer adminPublicKey;
        protected readonly UserTrustProvider trustProvider;

        internal IDatabaseUser DatabaseUser { get; }

        public UserId Id => DatabaseUser.Id;
        public IAsymmetricPrivateTransformer Transformer { get; }

        public UserSession(ICryptographyProvider crypto,
                           IDatabase database /* reference */,
                           IDatabaseUser user /* reference */,
                           ReadOnlySpan<byte> privateKey)
        {
            this.crypto = crypto;
            this.database = database;
            DatabaseUser = user;

            IDatabaseUser admin = database.OpenUser(UserId.Admin(), true);
            adminPublicKey = crypto.OpenAsymmetricTransformer(admin.PublicKey.Payload.Span);

            entryController = new EntryController(crypto, database, this);
            Transformer = crypto.OpenAsymmetricTransformer(user.PublicKey.Payload.Span, privateKey);
            metadata = new UserMetadataDecoder(adminPublicKey);
            trustProvider = new UserTrustProvider(database, crypto, Transformer, adminPublicKey);
        }

        public UserMetadata OpenMetadata()
        {
            return metadata.OpenMetadata(DatabaseUser);
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

        public IEnumerable<UserInfo> EnumerateTrustedUsers()
        {
            foreach (UserInfo user in trustProvider.EnumerateTrustedUsers())
            {
                yield return user;
            }
        }

        public bool VerifyTrust(UserId userId)
        {
            return trustProvider.VerifyTrust(userId);
        }

        public void Dispose()
        {
            Transformer.Dispose();
            trustProvider.Dispose();
            entryController.Dispose();
            adminPublicKey.Dispose();
        }
    }
}
