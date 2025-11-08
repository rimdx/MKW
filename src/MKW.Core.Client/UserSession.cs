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
        private readonly IAsymmetricPublicTransformer adminPublicKey;

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
            AsymmetricPublicKey decodedKey = crypto.DecodePkcsPublicKey(admin.ProtectedData.PublicKey.Span);

            adminPublicKey = crypto.OpenAsymmetricTransformer(
                decodedKey);

            metadata = new UserMetadataDecoder(database, adminPublicKey);
        }

        public UserMetadata OpenMetadata()
        {
            return metadata.OpenMetadata(user);
        }

        public EntryPayload? OpenEntry(EntryId entryId)
        {
            UserTrustProvider trustProvider = CreateTrustProvider(database.CreateSnapshot());
            EntryController entryController = new EntryController(database, crypto, user.Id, trustProvider, transformer);

            return entryController.Open(entryId);
        }

        public void CreateEntry(EntryId entryId, EntryPayload payload)
        {
            UserTrustProvider trustProvider = CreateTrustProvider(database.CreateSnapshot());
            EntryController entryController = new EntryController(database, crypto, user.Id, trustProvider, transformer);

            entryController.Create(entryId, payload);
        }

        public EntryId CreateEntry(EntryPayload payload)
        {
            UserTrustProvider trustProvider = CreateTrustProvider(database.CreateSnapshot());
            EntryController entryController = new EntryController(database, crypto, user.Id, trustProvider, transformer);

            EntryId entryId = EntryId.Create();
            entryController.Create(entryId, payload);
            return entryId;
        }

        public void UpdateEntry(EntryId entryId, EntryPayload newPayload)
        {
            UserTrustProvider trustProvider = CreateTrustProvider(database.CreateSnapshot());
            EntryController entryController = new EntryController(database, crypto, user.Id, trustProvider, transformer);

            entryController.Update(entryId, newPayload);
        }

        public void DeleteEntry(EntryId entryId)
        {
            using (IDatabaseNG.ITransaction transaction = database.BeginTransaction())
            {
                transaction.DeleteEntry(entryId);
                transaction.Commit();
            }
        }

        public IEnumerable<KeyValuePair<EntryId, EntryPayload?>> EnumerateEntries()
        {
            IDatabaseNG.ISnapshot snapshot = database.CreateSnapshot();
            foreach (DatabaseEntry entry in snapshot.EnumerateEntries())
            {
                EntryPayload? payload = OpenEntry(entry.Id);
                yield return new KeyValuePair<EntryId, EntryPayload?>(entry.Id, payload);
            }
        }

        // ITrustProvider

        public IEnumerable<UserId> EnumerateTrustedUsers()
        {
            UserTrustProvider trustProvider = CreateTrustProvider(database.CreateSnapshot());

            foreach (UserId userId in trustProvider.EnumerateTrustedUsers())
            {
                yield return userId;
            }
        }

        public bool VerifyTrust(UserId userId)
        {
            UserTrustProvider trustProvider = CreateTrustProvider(database.CreateSnapshot());

            return trustProvider.VerifyTrust(userId);
        }

        private UserTrustProvider CreateTrustProvider(IDatabaseNG.ISnapshot snapshot)
        {
            return new UserTrustProvider(snapshot, crypto, transformer, adminPublicKey);
        }

        public void Dispose()
        {
            transformer.Dispose();
            adminPublicKey.Dispose();
        }
    }
}
