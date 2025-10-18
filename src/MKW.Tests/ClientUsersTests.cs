// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.Core.Client;
using MKW.Core.Exceptions;
using MKW.Storage;
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
            ClassicAssert.AreEqual(user.PublicKey, users[1].PublicKey.Payload);
        }

        [Test]
        public void OpenUserTest()
        {
            using ClientSandBox sbox = new ClientSandBox();
            using IDatabase db = sbox.OpenDatabase();
            using ClientSession client = sbox.OpenSession(db);

            using IUserSession user = sbox.CreateUser(client, "awesomesecretno1willeverguess", out _);

            using IUserSession userSession = client.OpenUser(user.Id, "awesomesecretno1willeverguess");

            Assert.Throws<Exception>(
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
        public void AccessRequestTests()
        {
            using ClientSandBox sbox = new ClientSandBox();
            using IDatabase db = sbox.OpenDatabase();
            using ClientSession client = sbox.OpenSession(db);

            using IAdminSession admin = sbox.OpenAdmin(client);

            EntryId entryId;
            using (IEntrySession entry = admin.CreateEntry())
            {
                entry.UpdatePayload(sbox.CreatePayload("secret stuff"));
                entryId = entry.Id;
            }

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

            using (IEntrySession entry = userSession.OpenEntry(entryId))
            {
                ClassicAssert.AreEqual(sbox.CreatePayload("secret stuff"),
                                       entry.OpenPayload());
            }

            EntryId newEntryId;
            using (IEntrySession entry = userSession.CreateEntry())
            {
                entry.UpdatePayload(sbox.CreatePayload("new entry"));
                newEntryId = entry.Id;
            }

            using (IEntrySession entry = admin.OpenEntry(newEntryId))
            {
                ClassicAssert.AreEqual(sbox.CreatePayload("new entry"),
                                       entry.OpenPayload());
            }
        }
    }
}
