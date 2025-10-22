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
        private readonly SystemCredentialsManager credManager;

        public UserController(ClientCryptography crypto,
                              IDatabase database)
        {
            this.crypto = crypto;
            this.database = database;

            credManager = new SystemCredentialsManager(crypto);
        }

        public IUserSession OpenUser(UserId id, string password)
        {
            if (id.IsAdmin)
            {
                return OpenAdmin(password);
            }
            else
            {
                DatabaseUser user = database.OpenUser(id);
                return OpenUserInternal(user, password);
            }
        }

        public IUserSession OpenUser(string password)
        {
            foreach (DatabaseUser user in database.EnumerateUsers())
            {
                try
                {
                    return OpenUserInternal(user, password);
                }
                catch (Exceptions.InvalidPasswordException)
                {
                    // Ignore this user if the credentials are invalid
                }
            }

            throw new Exception("No valid user found with the provided password.");
        }

        private IUserSession OpenUserInternal(DatabaseUser user, string password)
        {
            IUserCredentials creds = crypto.OpenUserCredentials(password, user.Salt);

            SystemCredentials systemCreds = credManager.OpenCredentials(user, creds);

            IAsymmetricPrivateTransformer transformer = crypto.OpenAsymmetricTransformer(
                systemCreds.PrivateKey);

            return new UserSession(crypto, database, user, transformer);
        }

        public IAdminSession OpenAdmin(string password)
        {
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
