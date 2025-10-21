// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Cryptography;
using MKW.Storage;

namespace MKW.Core.Client
{
    internal sealed class UserSession
        : IUserSession
        , IDisposable
    {
        private readonly ClientCryptography crypto;
        private readonly IDatabase database;
        private readonly IAsymmetricPrivateTransformer transformer;

        private readonly UserMetadataDecoder metadata;
        private readonly EntryController entryController;
        private readonly IAsymmetricPublicTransformer adminPublicKey;
        private readonly UserTrustProvider trustProvider;

        private readonly DatabaseUser user;

        public UserId Id => user.Id;

        public UserSession(ClientCryptography crypto,
                           IDatabase database /* reference */,
                           DatabaseUser user /* reference */,
                           IAsymmetricPrivateTransformer transformer)
        {
            this.crypto = crypto;
            this.database = database;
            this.user = user;
            this.transformer = transformer;

            DatabaseUser admin = database.OpenUser(UserId.Admin());

            AsymmetricPublicKey decodedKey = crypto.DecodePkcsPublicKey(admin.PublicKey.Payload.Span);

            adminPublicKey = crypto.OpenAsymmetricTransformer(
                decodedKey);

            metadata = new UserMetadataDecoder(adminPublicKey);
            entryController = new EntryController(database, crypto, this, transformer);
            trustProvider = new UserTrustProvider(database, crypto, transformer, adminPublicKey);
        }

        public UserMetadata OpenMetadata()
        {
            return metadata.OpenMetadata(user);
        }

        public EntryPayload? OpenEntry(EntryId entryId)
        {
            return entryController.Open(entryId);
        }

        public void CreateEntry(EntryId entryId, EntryPayload payload)
        {
            entryController.Create(entryId, payload);
        }

        public EntryId CreateEntry(EntryPayload payload)
        {
            EntryId entryId = EntryId.Create();
            entryController.Create(entryId, payload);
            return entryId;
        }

        public void UpdateEntry(EntryId entryId, EntryPayload newPayload)
        {
            entryController.Update(entryId, newPayload);
        }

        public void DeleteEntry(EntryId entryId)
        {
            database.DeleteEntry(entryId);
        }

        public IEnumerable<KeyValuePair<EntryId, EntryPayload?>> EnumerateEntries()
        {
            foreach (DatabaseEntry entry in database.EnumerateEntries())
            {
                EntryPayload? payload = OpenEntry(entry.Id);
                yield return new KeyValuePair<EntryId, EntryPayload?>(entry.Id, payload);
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
