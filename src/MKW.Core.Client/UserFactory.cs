// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Cryptography;
using MKW.Storage;

namespace MKW.Core.Client
{
    internal sealed class UserFactory
    {
        private readonly ClientCryptography crypto;
        private readonly IDatabase database;

        public UserFactory(ClientCryptography crypto,
                           IDatabase database)
        {
            this.crypto = crypto;
            this.database = database;
        }

        public UserAccessRequest CreateUserAccessRequest(string password)
        {
            DatabaseUser admin = database.OpenUser(UserId.Admin());

            SystemCredentialsManager credManager = new SystemCredentialsManager(crypto);

            IUserCredentials userCreds = crypto.CreateUserCredentials(password);
            SystemCredentials systemCreds = credManager.GenerateCredentials(userCreds);

            using IAsymmetricPrivateTransformer transformer = crypto.OpenAsymmetricTransformer(
                systemCreds.PrivateKey);

            // TODO: prompt user?
            ReadOnlyMemory<byte> signature = transformer.Sign(admin.ProtectedData.PublicKey.Span);

            return new UserAccessRequest
            {
                Salt = systemCreds.Salt,
                PublicKey = systemCreds.PublicKey,
                EncryptedPrivateKey = systemCreds.EncryptedPrivateKey,
                AdminSignature = signature,
            };
        }
    }
}
