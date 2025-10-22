// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Cryptography;
using MKW.Storage;

namespace MKW.Core.Client
{
    internal sealed class UserController : IDisposable
    {
        private readonly ClientCryptography crypto;
        private readonly IDatabase database;

        public UserController(ClientCryptography crypto,
                              IDatabase database)
        {
            this.crypto = crypto;
            this.database = database;
        }

        public IUserSession OpenUser(UserId id, string password)
        {
            if (id.IsAdmin)
            {
                return OpenAdmin(password);
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

        public void Dispose()
        {
            /* no-op */
        }
    }
}
