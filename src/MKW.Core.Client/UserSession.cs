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
        private readonly IDatabase database;
        private readonly IEntryController entryController;
        private readonly UserMetadataDecoder metadata;
        private readonly IAsymmetricPublicTransformer adminPublicKey;
        private readonly UserTrustProvider trustProvider;

        private readonly IDatabaseUser databaseUser;

        public UserId Id => databaseUser.Id;
        public IAsymmetricPrivateTransformer Transformer { get; }

        public UserSession(ICryptographyProvider crypto,
                           IDatabase database /* reference */,
                           IDatabaseUser user /* reference */,
                           ReadOnlySpan<byte> privateKey)
        {
            this.database = database;
            databaseUser = user;

            IDatabaseUser admin = database.OpenUser(UserId.Admin(), true);
            adminPublicKey = crypto.OpenAsymmetricTransformer(admin.PublicKey.Payload.Span);

            Transformer = crypto.OpenAsymmetricTransformer(user.PublicKey.Payload.Span, privateKey);

            entryController = new EntryController(crypto, database, this, Transformer);
            metadata = new UserMetadataDecoder(adminPublicKey);
            trustProvider = new UserTrustProvider(database, crypto, Transformer, adminPublicKey);
        }

        public UserMetadata OpenMetadata()
        {
            return metadata.OpenMetadata(databaseUser);
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
