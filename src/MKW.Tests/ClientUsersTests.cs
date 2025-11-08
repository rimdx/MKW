// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.Core.Client;
using MKW.Core.Exceptions;
using MKW.Storage;
using MKW.Storage.Exceptions;
using MKW.Testing.Client;
using NUnit.Framework.Legacy;

namespace MKW.Tests
{
    public class ClientUsersTests
    {
        [Test]
        public void SimpleAddUserTest()
        {
            using ClientSandBox sbox = new ClientSandBox();
            using IDatabase db = sbox.OpenDatabase();
            using ClientSession session = ClientSession.Open(db, sbox.Crypto);

            using IUserSession s1 = sbox.CreateUser(session, "whattheheckamidoing", out UserInfo user);

            DatabaseUser[] users = db.EnumerateUsers().ToArray();
            ClassicAssert.AreEqual(2, users.Length);

            ClassicAssert.AreEqual(user.Id, users[1].Id);
            ClassicAssert.AreEqual(user.PublicKey, users[1].ProtectedData.PublicKey);
        }

        [Test]
        public void OpenUserTest()
        {
            using ClientSandBox sbox = new ClientSandBox();
            using IDatabase db = sbox.OpenDatabase();
            using ClientSession client = sbox.OpenSession(db);

            using IUserSession user = sbox.CreateUser(client, "awesomesecretno1willeverguess", out _);

            using IUserSession userSession = client.OpenUser(user.Id, "awesomesecretno1willeverguess");

            Assert.Throws<UserDoesNotExistException>(
                () => client.OpenUser(UserId.Create(),
                                      "awesomesecretno1willeverguess")
            );

            Assert.Throws<InvalidPasswordException>(
                () => client.OpenUser(user.Id, "randomheckerpwdhaha")
            );
        }

        [Test]
        public void OpenUserTestNoId()
        {
            using ClientSandBox sbox = new ClientSandBox();
            using IDatabase db = sbox.OpenDatabase();
            using ClientSession client = sbox.OpenSession(db);

            using IUserSession user1 = sbox.CreateUser(client, "cred1", out _);
            using IUserSession user2 = sbox.CreateUser(client, "cred2", out _);
            using IUserSession user3 = sbox.CreateUser(client, "cred3", out _);

            using IUserSession userSession1 = client.OpenUser("cred1");
            using IUserSession userSession2 = client.OpenUser("cred2");
            using IUserSession userSession3 = client.OpenUser("cred3");

            Assert.Throws<Exception>(
                () => client.OpenUser("nonexistingpassword")
            );
        }

        [Test]
        [Ignore("TODO")]
        public void UntrustedUserTest()
        {
            using ClientSandBox sbox = new ClientSandBox();
            using IDatabase database = sbox.OpenDatabase();
            using ClientSession client = sbox.OpenSession(database);

            UserInfo userInfo;
            using (IUserSession user = sbox.CreateUser(client, "123", out userInfo))
            {
            }

            // database.AddTrustSignature(new DatabaseTrustSignature
            // {
            //     Id = userInfo.Id,
            //     SignatureBytes = new byte[42],
            // });

            Assert.Throws<Exception>(() => client.OpenUser(userInfo.Id, "123"));
        }

        [Test]
        [Ignore("TODO")]
        public void UntrustedAdminTest()
        {
            using ClientSandBox sbox = new ClientSandBox();
            using IDatabase database = sbox.OpenDatabase();
            using ClientSession client = sbox.OpenSession(database);

            UserInfo userInfo;
            using (IUserSession user = sbox.CreateUser(client, "123", out userInfo))
            {
            }

            using (var transaction = database.BeginTransaction())
            {
                DatabaseUser dbUser = database.OpenUser(userInfo.Id);

                transaction.UpdateUser(dbUser with
                {
                    ProtectedData = new DatabaseUserProtectedDataSigned
                    {
                        PublicKey = new byte[42],
                        Metadata = new byte[24],
                        Signature = dbUser.ProtectedData.Signature,
                    },
                });

                transaction.Commit();
            }

            Assert.Throws<Exception>(() => client.OpenUser(userInfo.Id, "123"));
            Assert.Throws<Exception>(() => client.OpenAdmin(sbox.AdminSecret));
        }

        [Test]
        public void AccessRequestTests()
        {
            using ClientSandBox sbox = new ClientSandBox();
            using IDatabase db = sbox.OpenDatabase();
            using ClientSession client = sbox.OpenSession(db);

            using IAdminSession admin = sbox.OpenAdmin(client);

            EntryId entryId = admin.CreateEntry(sbox.CreatePayload("secret stuff"));

            // create request
            UserAccessRequest request = client.CreateUserAccessRequest("secret");

            // nothing changed yet
            CollectionAssert.AreEqual(
                new[]
                {
                    client.GetAdminInfo(),
                },
                client.EnumerateUsers());

            // approve request
            UserInfo addedUser = admin.CreateUser(
                request,
                new UserMetadata
                {
                    DisplayName = "Mr. Bob",
                    UserId = "notbob@contoso.com"
                });

            // verify
            CollectionAssert.AreEqual(
                new[]
                {
                    client.GetAdminInfo(),
                    addedUser,
                },
                client.EnumerateUsers());

            CollectionAssert.AreEqual(addedUser.PublicKey.ToArray(),
                                      request.PublicKey.ToArray());

            using IUserSession userSession = client.OpenUser(addedUser.Id, "secret");

            userSession.CreateEntry(sbox.CreatePayload("secret stuff"));

            EntryId newEntryId = userSession.CreateEntry(sbox.CreatePayload("new entry"));

            ClassicAssert.AreEqual(sbox.CreatePayload("new entry"),
                                   admin.OpenEntry(newEntryId));
        }
    }
}
