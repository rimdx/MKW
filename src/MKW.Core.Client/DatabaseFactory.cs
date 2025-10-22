// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Cryptography;
using MKW.Storage;

namespace MKW.Core.Client
{
    public static class DatabaseFactory
    {
        public static ClientSession InitializeDatabase(IDatabase database,
                                                       ICryptographyProvider cryptoProvider,
                                                       string adminPassword,
                                                       UserMetadata adminMetadata)
        {
            DatabaseConfiguration cryptoConfig = database.GetConfiguration();
            ClientCryptography crypto = new ClientCryptography(cryptoProvider, cryptoConfig);

            SystemCredentialsManager credManager = new SystemCredentialsManager(crypto);

            IUserCredentials userCreds = crypto.CreateUserCredentials(adminPassword);

            SystemCredentials systemCreds = credManager.GenerateCredentials(userCreds);

            using IAsymmetricPrivateTransformer transformer = crypto.OpenAsymmetricTransformer(
                systemCreds.PrivateKey);

            UserMetadataEncoder metadataEncoder = new UserMetadataEncoder(transformer);

            DatabaseUser admin = new DatabaseUser
            {
                Id = UserId.Admin(),
                PublicKey = new SignedPayload(systemCreds.PublicKey, null),
                PrivateKey = systemCreds.EncryptedPrivateKey,
                Salt = systemCreds.Salt,
                Metadata = metadataEncoder.EncodeMetadata(adminMetadata),
            };

            database.CreateUser(UserId.Admin(), admin);

            // We are not gonna sign ourselves (as AddTrustSignature) for now
            // TODO: ?

            return new ClientSession(database, cryptoProvider);
        }
    }
}
