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

        public void Dispose()
        {
        }
    }
}
