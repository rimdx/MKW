// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.Core.Client;
using MKW.Cryptography;
using MKW.Cryptography.Loader;
using MKW.Storage;
using MKW.Storage.JSON;
using MKW.Storage.MKPG;
using MKW.Testing.Common;

namespace MKW.Testing.Client
{
    public class ClientSandBox : SandBoxBase
    {
        private const bool USE_MKPGDatabase = false;

        public string AdminSecret => "adminsecret123";

        public ICryptographyProvider Crypto = CryptographyLoader.GetProvider();

        public ClientSandBox(bool init = true)
        {
            if (init)
            {
                UserMetadata metadata = new UserMetadata
                {
                    DisplayName = "admin",
                    UserId = "admin@contoso.com"
                };

                using IDatabase db = CreateDatabase();
                using ClientSession client = ClientSession.Create(db, Crypto, AdminSecret, metadata);
            }
        }

        public EntryPayload CreatePayload(string content)
        {
            EntryPayload result = new EntryPayload();
            result.SetProperty(new EntryPayloadKey("mkw:test:property"), content);
            return result;
        }

        public IDatabase CreateDatabase()
        {
            if (USE_MKPGDatabase)
            {
                return MKPGDatabase.Create(DatabasePath);
            }
            else
            {
                return JSONDatabaseSession.Create(DatabasePath);
            }
        }

        public IDatabase OpenDatabase()
        {
            if (USE_MKPGDatabase)
            {
                return MKPGDatabase.Open(DatabasePath);
            }
            else
            {
                return JSONDatabaseSession.Open(DatabasePath);
            }
        }

        public ClientSession OpenSession(IDatabase db)
        {
            return ClientSession.Open(db, Crypto);
        }

        public IAdminSession OpenAdmin(ClientSession client)
        {
            return client.OpenAdmin(AdminSecret);
        }

        public IUserSession CreateUser(ClientSession client,
                                       string password,
                                       out UserInfo user)
        {
            using IAdminSession admin = OpenAdmin(client);

            UserAccessRequest request = client.CreateUserAccessRequest(password);

            user = admin.CreateUser(
                request,
                new UserMetadata
                {
                    UserId = $"{password}@privatetestgang.com",
                    DisplayName = password
                });

            IUserSession userSession = client.OpenUser(user.Id, password);

            return userSession;
        }
    }
}
