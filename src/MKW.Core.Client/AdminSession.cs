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
        private DatabaseUser admin;
        private readonly IAsymmetricPrivateTransformer transformer;

        private readonly UserMetadataDecoder metadata;
        private readonly EntryController entryController;
        private readonly UserTrustProvider trustProvider;

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
        }


        public UserInfo CreateUser(UserAccessRequest request, UserMetadata metadata)
        {
            // === 🤓 TERMINOLOGY 🤓 ===
            // admin -> source (the one who performs the action)
            // user -> target (the one whom me perform the action over)
            //
            // signatures are identified by the location where they are stored in the database.
            //
            // each signature is stored in the same place alongside with the data
            //
            // for example, our admin, since they can have as much signature as the amount
            // of users in the database, store signatures of all these users, as a confirmation
            // of their own pubkey+metadata packet.

            UserId userId = UserId.Create();

            DatabaseUserProtectedData protectedData = new DatabaseUserProtectedData
            {
                PublicKey = request.PublicKey,
                Metadata = UserMetadataSerializer.Serialize(metadata),
            };

            ReadOnlyMemory<byte> protectedDataBytes = database.SerializeProtectedData(protectedData);

            // represents a signature created by the admin (source) to add to the user (target)
            DatabaseTrustSignature targetSignature = new DatabaseTrustSignature
            {
                Id = admin.Id,
                SignatureBytes = transformer.Sign(protectedDataBytes.Span),
            };

            // represents a signature created by the user as they initialized their
            // user access request and have confirmed that they trust the admin (us,
            // the source of the operation).
            DatabaseTrustSignature sourceSignature = new DatabaseTrustSignature
            {
                Id = userId,
                SignatureBytes = request.AdminSignature,
            };

            DatabaseUserProtectedDataSigned targetProtectedDataSigned = new DatabaseUserProtectedDataSigned
            {
                PublicKey = protectedData.PublicKey,
                Metadata = protectedData.Metadata,
                Signature = [targetSignature],
            };

            DatabaseUser user = new DatabaseUser
            {
                Id = userId,
                Salt = request.Salt,
                ProtectedData = targetProtectedDataSigned,
                PrivateKey = request.EncryptedPrivateKey,
            };

            database.CreateUser(userId, user);

            // modify ourselves in the database to include user's signature (the one from
            // user access request).
            admin = UserSignatureManager.AddSignature(admin, sourceSignature);
            database.UpdateUser(UserId.Admin(), admin);

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
                IsAdmin = user.Id.IsAdmin,
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
