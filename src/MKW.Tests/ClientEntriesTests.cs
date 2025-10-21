// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using MKW.Core;
using MKW.Core.Client;
using MKW.Storage;
using MKW.Storage.Exceptions;
using MKW.Testing.Client;
using NUnit.Framework.Legacy;

namespace MKW.Tests
{
    public class ClientEntriesTests
    {
        [Test]
        public void AddEntryTests()
        {
            using ClientSandBox sbox = new ClientSandBox();
            using IDatabase db = sbox.OpenDatabase();
            using ClientSession client = ClientSession.Open(db, sbox.Crypto);

            using IUserSession user = sbox.CreateUser(client, "secretprotector", out _);
            using IAdminSession admin = sbox.OpenAdmin(client);

            EntryId entryId = EntryId.Create();
            admin.CreateEntry(entryId, sbox.CreatePayload("secret"));

            ClassicAssert.AreEqual(2, db.EnumerateUsers().Count());
            ClassicAssert.AreEqual(1, db.EnumerateEntries().Count());
            ClassicAssert.AreEqual(2, db.EnumerateEntries().First().Keys.Count);

            ClassicAssert.AreEqual(entryId, db.EnumerateEntries().First().Id);

            ClassicAssert.AreEqual(sbox.CreatePayload("secret"),
                                   user.OpenEntry(entryId));
            ClassicAssert.AreEqual(sbox.CreatePayload("secret"),
                                   admin.OpenEntry(entryId));
        }

        [Test]
        public void HiddenEntriesTests()
        {
            using ClientSandBox sbox = new ClientSandBox();
            using IDatabase db = sbox.OpenDatabase();
            using ClientSession client = ClientSession.Open(db, sbox.Crypto);

            using IAdminSession admin = sbox.OpenAdmin(client);
            using IUserSession oldUser = sbox.CreateUser(client, "iamanoldman", out _);
            using IUserSession newUser = sbox.CreateUser(client, "ihatehimbutcantseehisstuff", out _);

            EntryId entry1id = oldUser.CreateEntry(sbox.CreatePayload("entry1"));
            EntryId entry2id = oldUser.CreateEntry(sbox.CreatePayload("entry2"));

            {
                DatabaseEntry entry = db.OpenEntry(entry1id);
                entry.Keys.Remove(newUser.Id);
                db.UpdateEntry(entry1id, entry);
            }

            CollectionAssert.AreEqual(
                new Dictionary<EntryId, EntryPayload?>
                {
                    { entry1id, sbox.CreatePayload("entry1") },
                    { entry2id, sbox.CreatePayload("entry2") },
                },
                oldUser.EnumerateEntries()
            );

            CollectionAssert.AreEqual(
                new Dictionary<EntryId, EntryPayload?>
                {
                    { entry1id, null },
                    { entry2id, sbox.CreatePayload("entry2") },
                },
                newUser.EnumerateEntries()
            );

            oldUser.UpdateEntry(entry1id, sbox.CreatePayload("newcontent"));

            CollectionAssert.AreEqual(
                new Dictionary<EntryId, EntryPayload?>
                {
                    { entry1id, sbox.CreatePayload("newcontent") },
                    { entry2id, sbox.CreatePayload("entry2") },
                },
                oldUser.EnumerateEntries()
            );

            CollectionAssert.AreEqual(
                new Dictionary<EntryId, EntryPayload?>
                {
                    { entry1id, sbox.CreatePayload("newcontent") },
                    { entry2id, sbox.CreatePayload("entry2") },
                },
                newUser.EnumerateEntries()
            );
        }

        [Test]
        public void UserEntryAPITest()
        {
            // init
            using ClientSandBox sbox = new ClientSandBox();
            EntryId entryId;

            using (IDatabase db = sbox.OpenDatabase())
            using (ClientSession client = sbox.OpenSession(db))
            {
                using IUserSession user = sbox.CreateUser(client, "usersecret", out _);

                // create
                entryId = user.CreateEntry(sbox.CreatePayload("data1"));

                ClassicAssert.AreEqual(sbox.CreatePayload("data1"),
                                       user.OpenEntry(entryId));

                // another update
                user.UpdateEntry(entryId, sbox.CreatePayload("data2"));

                ClassicAssert.AreEqual(sbox.CreatePayload("data2"),
                                       user.OpenEntry(entryId));

                // create with same id
                Assert.Throws<EntryAlreadyExistsException>(() => user.CreateEntry(entryId, sbox.CreatePayload("invalid")));
            }

            // blank session
            using (IDatabase db = sbox.OpenDatabase())
            using (ClientSession client = sbox.OpenSession(db))
            {
                using IUserSession user = client.OpenUser("usersecret");

                ClassicAssert.AreEqual(sbox.CreatePayload("data2"),
                                       user.OpenEntry(entryId));

                // delete
                user.DeleteEntry(entryId);
                Assert.Throws<EntryDoesNotExistException>(() => user.OpenEntry(entryId));
                user.DeleteEntry(entryId);
                Assert.Throws<EntryDoesNotExistException>(() => user.OpenEntry(entryId));
            }
        }
    }
}
