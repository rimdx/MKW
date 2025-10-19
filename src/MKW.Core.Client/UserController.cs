// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core.Implementation;
using MKW.Cryptography;
using MKW.Storage;

namespace MKW.Core.Client
{
    internal sealed class UserController : IDisposable
    {
        private readonly ClientSession client;
        private readonly ClientCryptography crypto;
        private readonly IDatabase database;

        public UserController(ClientSession client,
                              ClientCryptography crypto,
                              IDatabase database)
        {
            this.client = client;
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
            Memory<byte> signature = transformer.Sign(admin.PublicKey.Payload.Span);

            return new UserAccessRequest
            {
                Salt = systemCreds.Salt,
                PublicKey = systemCreds.PublicKey,
                EncryptedPrivateKey = systemCreds.EncryptedPrivateKey,
                AdminSignature = signature,
            };
        }

        public IUserSession OpenUser(UserId id, string password)
        {
            if (id.IsAdmin)
            {
                // todo:
                return client.OpenAdmin(password);
            }
            else
            {
                SystemCredentialsManager credManager = new SystemCredentialsManager(crypto);

                DatabaseUser user = database.OpenUser(id);

                // Credentials can be opened within the entered password and the public salt
                IUserCredentials creds = crypto.OpenUserCredentials(password, user.Salt);

                SystemCredentials systemCreds = credManager.OpenCredentials(user, creds);

                IAsymmetricPrivateTransformer transformer = crypto.OpenAsymmetricTransformer(
                    systemCreds.PrivateKey);

                return new UserSession(crypto, database, user, transformer);
            }
        }

        public IUserSession OpenUser(string password)
        {
            foreach (DatabaseUser user in database.EnumerateUsers())
            {
                try
                {
                    SystemCredentialsManager credManager = new SystemCredentialsManager(crypto);

                    IUserCredentials creds = crypto.OpenUserCredentials(password, user.Salt);

                    SystemCredentials systemCreds = credManager.OpenCredentials(user, creds);

                    IAsymmetricPrivateTransformer transformer = crypto.OpenAsymmetricTransformer(
                        systemCreds.PrivateKey);

                    return new UserSession(crypto, database, user, transformer);
                }
                catch (Exceptions.InvalidPasswordException)
                {
                    // Ignore this user if the credentials are invalid
                }
            }

            throw new Exception("No valid user found with the provided password.");
        }

        public IEnumerable<UserInfo> EnumerateUsers()
        {
            DatabaseUser admin = database.OpenUser(UserId.Admin());

            AsymmetricPublicKey decodedKey = crypto.DecodePkcsPublicKey(admin.PublicKey.Payload.Span);

            using IAsymmetricPublicTransformer adminKey = crypto.OpenAsymmetricTransformer(decodedKey);

            UserMetadataDecoder metadataDecoder = new UserMetadataDecoder(adminKey);

            foreach (DatabaseUser user in database.EnumerateUsers())
            {
                yield return CreateUserInfo(user, metadataDecoder.OpenMetadata(user));
            }
        }

        public UserInfo GetUserInfo(UserId id)
        {
            DatabaseUser admin = database.OpenUser(UserId.Admin());
            DatabaseUser user = database.OpenUser(id);

            AsymmetricPublicKey decodedKey = crypto.DecodePkcsPublicKey(admin.PublicKey.Payload.Span);

            using IAsymmetricPublicTransformer adminKey = crypto.OpenAsymmetricTransformer(decodedKey);

            UserMetadataDecoder metadataDecoder = new UserMetadataDecoder(adminKey);

            return CreateUserInfo(user, metadataDecoder.OpenMetadata(user));
        }

        public void Dispose()
        {
            /* no-op */
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
