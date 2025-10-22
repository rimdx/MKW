// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core.Serialization;
using MKW.Cryptography;
using MKW.Storage;

namespace MKW.Core.Client
{
    internal sealed class AdminSession
        : IAdminSession
        , IUserSession
        , IDisposable
    {
        private readonly ClientCryptography crypto;
        private readonly IDatabase database;
        private readonly DatabaseUser admin;
        private readonly IAsymmetricPrivateTransformer transformer;

        private readonly UserMetadataDecoder metadata;
        private readonly EntryController entryController;
        private readonly UserTrustProvider trustProvider;

        private readonly UserMetadataEncoder metadataEncoder;

        public UserId Id => admin.Id;

        public AdminSession(ClientCryptography crypto,
                            IDatabase database,
                            DatabaseUser admin,
                            IAsymmetricPrivateTransformer transformer)
        {
            this.crypto = crypto;
            this.database = database;
            this.admin = admin;
            this.transformer = transformer;

            metadata = new UserMetadataDecoder(database, transformer);
            entryController = new EntryController(database, crypto, this, transformer);
            trustProvider = new UserTrustProvider(database, crypto, transformer, transformer);

            metadataEncoder = new UserMetadataEncoder(transformer);
        }

        public UserInfo CreateUser(UserAccessRequest request, UserMetadata metadata)
        {
            UserId userId = UserId.Create();

            SignedPayload metadataBytes = metadataEncoder.EncodeMetadata(metadata);

            DatabaseUserProtectedDataSigned protectedDataSigned = new DatabaseUserProtectedDataSigned
            {
                PublicKey = request.PublicKey,
                Metadata = metadataBytes.Payload,
                Signature = transformer.Sign(request.PublicKey.Span),
            };

            DatabaseUser user = new DatabaseUser
            {
                Id = userId,
                Salt = request.Salt,
                ProtectedData = protectedDataSigned,
                PrivateKey = request.EncryptedPrivateKey,
            };

            database.CreateUser(userId, user);

            database.AddTrustSignature(new DatabaseTrustSignature
            {
                Id = userId,
                SignatureBytes = request.AdminSignature
            });

            EntryDecoder decoder = new EntryDecoder(crypto, this, transformer);
            EntryEncoder encoder = new EntryEncoder(crypto, database, this);
            DatabaseEntry[] entries = [.. database.EnumerateEntries()];

            foreach (DatabaseEntry entry in entries)
            {
                EntryPayload? payload = decoder.DecodeEntry(entry);

                if (payload != null)
                {
                    DatabaseEntry encoded = encoder.EncodeEntry(entry.Id, payload);
                    database.UpdateEntry(entry.Id, encoded);
                }
                else
                {
                    // TODO: fail? warn?
                }
            }

            return new UserInfo
            {
                Id = user.Id,
                PublicKey = user.ProtectedData.PublicKey,
                Trust = Trust.Unknown,
                Metadata = metadata
            };
        }

        public UserMetadata OpenMetadata()
        {
            return metadata.OpenMetadata(admin);
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
        }
    }
}
