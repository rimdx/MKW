// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Cryptography;
using MKW.Storage;

namespace MKW.Core.Client
{
    internal sealed class AdminController : IDisposable
    {
        private readonly ClientCryptography crypto;
        private readonly IDatabase database;

        public AdminController(ClientCryptography crypto,
                               IDatabase database)
        {
            this.crypto = crypto;
            this.database = database;
        }

        public IAdminSession OpenAdmin(string password)
        {
            SystemCredentialsManager credManager = new SystemCredentialsManager(crypto);

            DatabaseUser admin = database.OpenUser(UserId.Admin());

            IUserCredentials creds = crypto.OpenUserCredentials(password,
                                                                admin.Salt);

            SystemCredentials systemCreds = credManager.OpenCredentials(admin, creds);

            IAsymmetricPrivateTransformer transformer = crypto.OpenAsymmetricTransformer(systemCreds.PrivateKey);

            return new AdminSession(crypto, database, admin, transformer);
        }

        public UserInfo GetAdminInfo()
        {
            DatabaseUser admin = database.OpenUser(UserId.Admin());

            AsymmetricPublicKey decodedKey = crypto.DecodePkcsPublicKey(admin.PublicKey.Payload.Span);

            using IAsymmetricPublicTransformer adminKey = crypto.OpenAsymmetricTransformer(decodedKey);

            UserMetadataDecoder metadataDecoder = new UserMetadataDecoder(adminKey);

            return CreateUserInfo(admin, metadataDecoder.OpenMetadata(admin));
        }

        public void Dispose()
        {
        }

        private static UserInfo CreateUserInfo(DatabaseUser user, UserMetadata metadata, Trust trust = Trust.Unknown)
        {
            return new UserInfo
            {
                Id = user.Id,
                PublicKey = user.PublicKey.Payload,
                Trust = trust,
                Metadata = metadata
            };
        }
    }
}
