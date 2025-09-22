using MKW.Core.Implementation;
using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Client
{
    internal class AdminSession
        : IAdminSession
        , IUserSession
        , IUserHost
        , IEntryController
        , ITrustProvider
        , IDisposable
    {
        protected readonly ICryptographyProvider crypto;
        protected readonly IDatabase database;
        protected readonly IDatabaseUser admin;
        public IAsymmetricPrivateTransformer Transformer { get; }

        protected readonly IEntryController entryController;
        protected readonly UserMetadataDecoder metadata;
        protected readonly UserTrustProvider trustProvider;

        private readonly UserMetadataEncoder metadataEncoder;
        private readonly UserAccessController accessController;

        public UserId Id => admin.Id;

        public AdminSession(ICryptographyProvider crypto,
                            IDatabase database,
                            IDatabaseUser admin,
                            ReadOnlySpan<byte> privateKey)
        {
            this.crypto = crypto;
            this.database = database;
            this.admin = admin;

            Transformer = crypto.OpenAsymmetricTransformer(admin.PublicKey.Payload.Span, privateKey);

            entryController = new EntryController(crypto, database, this, Transformer);
            metadata = new UserMetadataDecoder(Transformer);
            trustProvider = new UserTrustProvider(database, crypto, Transformer, Transformer);

            metadataEncoder = new UserMetadataEncoder(Transformer);
            accessController = new UserAccessController(this);
        }

        public UserInfo CreateUser(UserAccessRequest request, UserMetadata metadata)
        {
            UserId userId = UserId.Create();

            IDatabaseUser user = database.CreateUser(userId);

            user.Salt = request.Salt;
            user.PublicKey = new SignedPayload(request.PublicKey, Transformer.Sign(request.PublicKey.Span));
            user.PrivateKey = request.EncryptedPrivateKey;
            user.Metadata = metadataEncoder.EncodeMetadata(metadata);

            user.AdminSignature = request.AdminSignature;

            user.Save();

            accessController.AddAccess(userId);

            return new UserInfo
            {
                Id = user.Id,
                PublicKey = user.PublicKey.Payload,
                Trust = Trust.Unknown,
                Metadata = metadata
            };
        }

        public UserMetadata OpenMetadata()
        {
            return metadata.OpenMetadata(admin);
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
        }
    }
}
