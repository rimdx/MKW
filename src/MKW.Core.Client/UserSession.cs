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
        private readonly IAsymmetricPrivateTransformer transformer;

        private readonly EntryController entryController;
        private readonly UserMetadataDecoder metadata;
        private readonly IAsymmetricPublicTransformer adminPublicKey;
        private readonly UserTrustProvider trustProvider;

        private readonly DatabaseUser user;

        public UserId Id => user.Id;

        public UserSession(ICryptographyProvider crypto,
                           IDatabase database /* reference */,
                           DatabaseUser user /* reference */,
                           ReadOnlySpan<byte> privateKey)
        {
            this.database = database;
            this.user = user;
            transformer = crypto.OpenAsymmetricTransformer(user.PublicKey.Payload.Span, privateKey);

            DatabaseUser admin = database.OpenUser(UserId.Admin());
            adminPublicKey = crypto.OpenAsymmetricTransformer(admin.PublicKey.Payload.Span);

            entryController = new EntryController(crypto, database, this, transformer);
            metadata = new UserMetadataDecoder(adminPublicKey);
            trustProvider = new UserTrustProvider(database, crypto, transformer, adminPublicKey);
        }

        public UserMetadata OpenMetadata()
        {
            return metadata.OpenMetadata(user);
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
            transformer.Dispose();
            trustProvider.Dispose();
            entryController.Dispose();
            adminPublicKey.Dispose();
        }
    }
}
