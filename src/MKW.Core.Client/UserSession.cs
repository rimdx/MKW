using MKW.Core.Implementation;
using MKW.Cryptography;
using MKW.Storage;

namespace MKW.Core.Client
{
    internal sealed class UserSession
        : IUserSession
        , IDisposable
    {
        private readonly ICryptographyProvider crypto;
        private readonly IDatabase database;
        private readonly IAsymmetricPrivateTransformer transformer;

        private readonly UserMetadataDecoder metadata;
        private readonly IAsymmetricPublicTransformer adminPublicKey;
        private readonly UserTrustProvider trustProvider;

        private readonly DatabaseUser user;

        public UserId Id => user.Id;

        public UserSession(ICryptographyProvider crypto,
                           IDatabase database /* reference */,
                           DatabaseUser user /* reference */,
                           IAsymmetricPrivateTransformer transformer)
        {
            this.crypto = crypto;
            this.database = database;
            this.user = user;
            this.transformer = transformer;

            DatabaseUser admin = database.OpenUser(UserId.Admin());

            adminPublicKey = crypto.OpenAsymmetricTransformer(
                admin.PublicKey.Payload.Span,
                CommonCryptographyAlgorithms.Rsa2048);

            metadata = new UserMetadataDecoder(adminPublicKey);
            trustProvider = new UserTrustProvider(database, crypto, transformer, adminPublicKey);
        }

        public UserMetadata OpenMetadata()
        {
            return metadata.OpenMetadata(user);
        }

        public IEntrySession OpenEntry(EntryId id)
        {
            return Entry.Open(database, crypto, this, transformer, id);
        }

        public IEntrySession CreateEntry(EntryId id)
        {
            return Entry.Create(database, crypto, this, transformer, id);
        }

        public IEntrySession CreateEntry()
        {
            return CreateEntry(EntryId.Create());
        }

        public void DeleteEntry(EntryId id)
        {
            database.DeleteEntry(id);
        }

        public IEnumerable<IEntrySession> EnumerateEntries()
        {
            foreach (DatabaseEntry entry in database.EnumerateEntries())
            {
                yield return OpenEntry(entry.Id);
            }
        }

        // ITrustProvider

        public IEnumerable<UserId> EnumerateTrustedUsers()
        {
            foreach (UserId userId in trustProvider.EnumerateTrustedUsers())
            {
                yield return userId;
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
            adminPublicKey.Dispose();
        }
    }
}
