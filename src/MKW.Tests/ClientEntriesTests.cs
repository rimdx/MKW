using MKW.Core;
using MKW.Core.Client;
using MKW.Core.Storage;
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

            using IEntrySession entry = admin.CreateEntry();
            entry.UpdatePayload(sbox.CreatePayload("secret"));

            ClassicAssert.AreEqual(2, db.EnumerateUsers().Count());
            ClassicAssert.AreEqual(1, db.EnumerateEntries().Count());
            ClassicAssert.AreEqual(2, db.EnumerateEntries().First().Keys.Count);

            ClassicAssert.AreEqual(entry.Id, db.EnumerateEntries().First().Id);

            CollectionAssert.AreEqual(
                new[]
                {
                    UserId.Admin(),
                    user.Id,
                },
                entry.EnumerateAccess());

            ClassicAssert.AreEqual(sbox.CreatePayload("secret"),
                                   user.OpenEntry(entry.Id).OpenPayload());
            ClassicAssert.AreEqual(sbox.CreatePayload("secret"),
                                   admin.OpenEntry(entry.Id).OpenPayload());
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

            using IEntrySession entry1 = oldUser.CreateEntry();
            entry1.UpdatePayload(sbox.CreatePayload("entry1"));

            using IEntrySession entry2 = oldUser.CreateEntry();
            entry2.UpdatePayload(sbox.CreatePayload("entry2"));

            {
                DatabaseEntry entry = db.OpenEntry(entry1.Id);
                entry.Keys.Remove(newUser.Id);
                db.UpdateEntry(entry1.Id, entry);
            }

            CollectionAssert.AreEqual(
                new EntryPayload?[]
                {
                    sbox.CreatePayload("entry1"),
                    sbox.CreatePayload("entry2"),
                },
                oldUser.EnumerateEntries().Select(entry => entry.OpenPayload())
            );
            CollectionAssert.AreEqual(
                new EntryPayload?[]
                {
                    null,
                    sbox.CreatePayload("entry2"),
                },
                newUser.EnumerateEntries().Select(entry => entry.OpenPayload())
            );

            entry1.UpdatePayload(sbox.CreatePayload("newcontent"));

            CollectionAssert.AreEqual(
                new EntryPayload?[]
                {
                    sbox.CreatePayload("newcontent"),
                    sbox.CreatePayload("entry2"),
                },
                oldUser.EnumerateEntries().Select(entry => entry.OpenPayload())
            );
            CollectionAssert.AreEqual(
                new EntryPayload?[]
                {
                    sbox.CreatePayload("newcontent"),
                    sbox.CreatePayload("entry2"),
                },
                newUser.EnumerateEntries().Select(entry => entry.OpenPayload())
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
                using IEntrySession entry = user.CreateEntry();
                entryId = entry.Id;

                ClassicAssert.AreNotEqual(EntryId.FromGuid(Guid.Empty), entry.Id);
                ClassicAssert.AreEqual(null,
                                       user.OpenEntry(entry.Id).OpenPayload());

                // initial update
                entry.UpdatePayload(sbox.CreatePayload("data1"));

                ClassicAssert.AreEqual(sbox.CreatePayload("data1"),
                                       user.OpenEntry(entry.Id).OpenPayload());

                // another update
                entry.UpdatePayload(sbox.CreatePayload("data2"));

                ClassicAssert.AreEqual(sbox.CreatePayload("data2"),
                                       user.OpenEntry(entry.Id).OpenPayload());

                // create with same id
                Assert.Throws<Exception>(() => user.CreateEntry(entry.Id));
            }

            // blank session
            using (IDatabase db = sbox.OpenDatabase())
            using (ClientSession client = sbox.OpenSession(db))
            {
                using IUserSession user = client.OpenUser("usersecret");
                using IEntrySession entry = user.OpenEntry(entryId);
                ClassicAssert.AreEqual(sbox.CreatePayload("data2"),
                                       user.OpenEntry(entry.Id).OpenPayload());

                // delete
                user.DeleteEntry(entry.Id);
                Assert.Throws<Exception>(() => user.OpenEntry(entry.Id));
                user.DeleteEntry(entry.Id);
                Assert.Throws<Exception>(() => user.OpenEntry(entry.Id));
            }
        }
    }
}
