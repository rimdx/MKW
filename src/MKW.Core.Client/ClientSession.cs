// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Cryptography;
using MKW.Storage;

namespace MKW.Core.Client
{
    public class ClientSession : IDisposable
    {
        public IDatabase Database { get; }

        private readonly ClientCryptography crypto;
        private readonly UserController userController;
        private readonly AdminController adminController;

        internal ClientSession(IDatabase db, ICryptographyProvider crypto)
        {
            Database = db;

            this.crypto = new ClientCryptography(crypto, db.GetConfiguration());

            userController = new UserController(this, this.crypto, Database);
            adminController = new AdminController(this.crypto, Database);
        }

        public static ClientSession Open(IDatabase db, ICryptographyProvider crypto)
        {
            ClientSession client = new ClientSession(db, crypto);

            // ensure the admin actually exists
            // a database without admin is invalid
            client.Database.OpenUser(UserId.Admin());

            return client;
        }

        public static ClientSession Create(IDatabase db,
                                           ICryptographyProvider crypto,
                                           string adminPassword,
                                           UserMetadata adminMetadata)
        {
            return DatabaseFactory.InitializeDatabase(db, crypto, adminPassword, adminMetadata);
        }

        // IUserController

        public IUserSession OpenUser(UserId id, string password)
        {
            return userController.OpenUser(id, password);
        }

        public IEnumerable<UserInfo> EnumerateUsers()
        {
            foreach (UserInfo user in userController.EnumerateUsers())
            {
                yield return user;
            }
        }

        public IUserSession OpenUser(string password)
        {
            return userController.OpenUser(password);
        }

        public UserInfo GetUserInfo(UserId id)
        {
            return userController.GetUserInfo(id);
        }

        public UserAccessRequest CreateUserAccessRequest(string password)
        {
            return userController.CreateUserAccessRequest(password);
        }

        // IAdminController

        public IAdminSession OpenAdmin(string password)
        {
            return adminController.OpenAdmin(password);
        }

        public UserInfo GetAdminInfo()
        {
            return adminController.GetAdminInfo();
        }

        public void Dispose()
        {
            userController.Dispose();
        }
    }
}
