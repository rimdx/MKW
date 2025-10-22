// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core.Serialization;
using MKW.Cryptography;
using MKW.Storage;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Ocsp;

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

            DatabaseUserProtectedData protectedData = new DatabaseUserProtectedData
            {
                PublicKey = systemCreds.PublicKey,
                Metadata = UserMetadataSerializer.Serialize(adminMetadata),
            };

            ReadOnlyMemory<byte> protectedDataBytes = database.SerializeProtectedData(protectedData);

            DatabaseUserProtectedDataSigned protectedDataSigned = new DatabaseUserProtectedDataSigned
            {
                PublicKey = protectedData.PublicKey,
                Metadata = protectedData.Metadata,
                Signature = transformer.Sign(protectedDataBytes.Span),
            };

            DatabaseUser admin = new DatabaseUser
            {
                Id = UserId.Admin(),
                ProtectedData = protectedDataSigned,
                PrivateKey = systemCreds.EncryptedPrivateKey,
                Salt = systemCreds.Salt,
            };

            database.CreateUser(UserId.Admin(), admin);

            // We are not gonna sign ourselves (as AddTrustSignature) for now
            // TODO: ?

            return new ClientSession(database, cryptoProvider);
        }
    }
}
