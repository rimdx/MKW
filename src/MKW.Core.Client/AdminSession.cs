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
        private readonly ICryptographyProvider crypto;
        private readonly IDatabase database;
        private readonly DatabaseUser admin;
        private readonly IAsymmetricPrivateTransformer transformer;

        private readonly IEntryController entryController;
        private readonly UserMetadataDecoder metadata;
        private readonly UserTrustProvider trustProvider;

        private readonly UserMetadataEncoder metadataEncoder;

        public UserId Id => admin.Id;

        public AdminSession(ICryptographyProvider crypto,
                            IDatabase database,
                            DatabaseUser admin,
                            ReadOnlySpan<byte> privateKey)
        {
            this.crypto = crypto;
            this.database = database;
            this.admin = admin;

            transformer = crypto.OpenAsymmetricTransformer(admin.PublicKey.Payload.Span, privateKey);

            entryController = new EntryController(crypto, database, this, transformer);
            metadata = new UserMetadataDecoder(transformer);
            trustProvider = new UserTrustProvider(database, crypto, transformer, transformer);

            metadataEncoder = new UserMetadataEncoder(transformer);
        }

        public UserInfo CreateUser(UserAccessRequest request, UserMetadata metadata)
        {
            UserId userId = UserId.Create();

            DatabaseUser user = new DatabaseUser
            {
                Id = userId,
                Salt = request.Salt,
                PublicKey = new SignedPayload(request.PublicKey, transformer.Sign(request.PublicKey.Span)),
                PrivateKey = request.EncryptedPrivateKey,
                Metadata = metadataEncoder.EncodeMetadata(metadata),
                AdminSignature = request.AdminSignature,
            };

            database.CreateUser(userId, user);

            EntryDecoder decoder = new EntryDecoder(crypto, this, transformer);
            EntryEncoder encoder = new EntryEncoder(crypto, database, trustProvider);
            DatabaseEntry[] entries = [.. database.EnumerateEntries()];

            foreach (DatabaseEntry entry in entries)
            {
                EntryPayload? payload = decoder.DecodeEntry(entry);

                if (payload != null)
                {
                    database.UpdateEntry(entry.Id, encoder.EncodeEntry(entry, payload));
                }
                else
                {
                    // TODO: fail? warn?
                }
            }

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
            transformer.Dispose();
            trustProvider.Dispose();
            entryController.Dispose();
        }
    }
}
