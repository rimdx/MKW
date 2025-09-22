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
            using ClientSession client = ClientSession.Open(db);

            using IUserSession user = sbox.CreateUser(client, "secretprotector", out _);
            using IAdminSession admin = sbox.OpenAdmin(client);

            EntryId entryId = EntryId.Create();

            EntryInfo entry = admin.UpdateEntry(entryId, new EntryPayload("secret"));

            ClassicAssert.AreEqual(2, db.EnumerateUsers().Count());
            ClassicAssert.AreEqual(1, db.EnumerateEntries().Count());
            ClassicAssert.AreEqual(2, db.EnumerateEntries().First().Keys.Count);

            ClassicAssert.AreEqual(entryId, db.EnumerateEntries().First().Id);

            CollectionAssert.AreEqual(
                new[]
                {
                    UserId.Admin(),
                    user.Id,
                },
                entry.EncodedForUsers);

            ClassicAssert.AreEqual(new EntryPayload("secret"),
                                   user.OpenEntry(entry.Id).OpenPayload());
            ClassicAssert.AreEqual(new EntryPayload("secret"),
                                   admin.OpenEntry(entry.Id).OpenPayload());
        }

        [Test]
        public void HiddenEntriesTests()
        {
            using ClientSandBox sbox = new ClientSandBox();
            using IDatabase db = sbox.OpenDatabase();
            using ClientSession client = ClientSession.Open(db);

            using IAdminSession admin = sbox.OpenAdmin(client);
            using IUserSession oldUser = sbox.CreateUser(client, "iamanoldman", out _);
            using IUserSession newUser = sbox.CreateUser(client, "ihatehimbutcantseehisstuff", out _);

            EntryId id1 = EntryId.Create();
            EntryId id2 = EntryId.Create();

            oldUser.UpdateEntry(id1, new EntryPayload("entry1"));
            oldUser.UpdateEntry(id2, new EntryPayload("entry2"));

            {
                IDatabaseEntry entry = db.OpenEntry(id1, false);
                entry.Keys.Remove(newUser.Id);
                entry.Save();
            }

            CollectionAssert.AreEqual(
                new EntryPayload?[]
                {
                    new EntryPayload("entry1"),
                    new EntryPayload("entry2"),
                },
                oldUser.EnumerateEntries().Select(entry => entry.OpenPayload())
            );
            CollectionAssert.AreEqual(
                new EntryPayload?[]
                {
                    null,
                    new EntryPayload("entry2"),
                },
                newUser.EnumerateEntries().Select(entry => entry.OpenPayload())
            );

            oldUser.UpdateEntry(id1, new EntryPayload("newcontent"));

            CollectionAssert.AreEqual(
                new EntryPayload?[]
                {
                    new EntryPayload("newcontent"),
                    new EntryPayload("entry2"),
                },
                oldUser.EnumerateEntries().Select(entry => entry.OpenPayload())
            );
            CollectionAssert.AreEqual(
                new EntryPayload?[]
                {
                    null,
                    new EntryPayload("entry2"),
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

            using (ClientSession client = sbox.OpenSession())
            {
                using IUserSession user = sbox.CreateUser(client, "usersecret", out _);

                // create
                using IEntrySession entry = user.CreateEntry();
                entryId = entry.Id;

                ClassicAssert.AreNotEqual(EntryId.FromGuid(Guid.Empty), entry.Id);
                ClassicAssert.AreEqual(null,
                                       user.OpenEntry(entry.Id).OpenPayload());

                // initial update
                entry.UpdatePayload(new EntryPayload("data1"));

                ClassicAssert.AreEqual(new EntryPayload("data1"),
                                       user.OpenEntry(entry.Id).OpenPayload());

                // another update
                entry.UpdatePayload(new EntryPayload("data2"));

                ClassicAssert.AreEqual(new EntryPayload("data2"),
                                       user.OpenEntry(entry.Id).OpenPayload());

                // create with same id
                Assert.Throws<Exception>(() => user.CreateEntry(entry.Id));
            }

            // blank session
            using (ClientSession client = sbox.OpenSession())
            {
                using IUserSession user = client.OpenUser("usersecret");
                using IEntrySession entry = user.OpenEntry(entryId);
                ClassicAssert.AreEqual(new EntryPayload("data2"),
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
