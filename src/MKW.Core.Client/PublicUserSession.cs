// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Cryptography;
using MKW.Storage;

namespace MKW.Core.Client
{
    internal sealed class PublicUserSession
    {
        private readonly ClientCryptography crypto;
        private readonly IDatabase database;

        public PublicUserSession(ClientCryptography crypto,
                                 IDatabase database)
        {
            this.crypto = crypto;
            this.database = database;
        }

        public IEnumerable<UserInfo> EnumerateUsers()
        {
            DatabaseUser admin = database.OpenUser(UserId.Admin());

            AsymmetricPublicKey decodedKey = crypto.DecodePkcsPublicKey(admin.ProtectedData.PublicKey.Span);

            using IAsymmetricPublicTransformer adminKey = crypto.OpenAsymmetricTransformer(decodedKey);

            UserMetadataDecoder metadataDecoder = new UserMetadataDecoder(database, adminKey);

            foreach (DatabaseUser user in database.EnumerateUsers())
            {
                yield return CreateUserInfo(user, metadataDecoder.OpenMetadata(user));
            }
        }

        public UserInfo GetUserInfo(UserId id)
        {
            DatabaseUser admin = database.OpenUser(UserId.Admin());
            DatabaseUser user = database.OpenUser(id);

            AsymmetricPublicKey decodedKey = crypto.DecodePkcsPublicKey(admin.ProtectedData.PublicKey.Span);

            using IAsymmetricPublicTransformer adminKey = crypto.OpenAsymmetricTransformer(decodedKey);

            UserMetadataDecoder metadataDecoder = new UserMetadataDecoder(database, adminKey);

            return CreateUserInfo(user, metadataDecoder.OpenMetadata(user));
        }

        public UserInfo GetAdminInfo()
        {
            DatabaseUser admin = database.OpenUser(UserId.Admin());

            AsymmetricPublicKey decodedKey = crypto.DecodePkcsPublicKey(admin.ProtectedData.PublicKey.Span);

            using IAsymmetricPublicTransformer adminKey = crypto.OpenAsymmetricTransformer(decodedKey);

            UserMetadataDecoder metadataDecoder = new UserMetadataDecoder(database, adminKey);

            return CreateUserInfo(admin, metadataDecoder.OpenMetadata(admin));
        }

        private static UserInfo CreateUserInfo(DatabaseUser user, UserMetadata metadata, Trust trust = Trust.Unknown)
        {
            return new UserInfo
            {
                Id = user.Id,
                PublicKey = user.ProtectedData.PublicKey,
                Trust = trust,
                Metadata = metadata
            };
        }
    }
}
