using MKW.Core;
using MKW.Core.Client;
using MKW.Core.Client.Notify;
using MKW.Core.Storage;
using NUnit.Framework.Legacy;
using System.Security.Cryptography;

namespace MKW.Tests
{
    public class DirectSessionTests
    {
        [Test]
        public void SimpleAddUserTest()
        {
            using MemoryDatabaseSession db = new MemoryDatabaseSession();
            using ClientSession session = ClientSession.Open(db);

            var user = session.PromoteUser("whattheheckamidoing");

            var users = db.EnumerateUsers().ToArray();
            ClassicAssert.AreEqual(1, users.Length);

            ClassicAssert.AreEqual(user.Id, users[0].Id);
            ClassicAssert.AreEqual(user.PublicKey, users[0].PublicKey);
        }

        [Test]
        public void OpenUserTest()
        {
            using var sbox = new SandBox();
            using var client = sbox.OpenSession();

            var user = client.PromoteUser("awesomesecretno1willeverguess");

            using UserSession userSession = client.OpenUser(user.Id, "awesomesecretno1willeverguess");

            Assert.Throws<Exception>(
                () => client.OpenUser(new Guid("{DEADCCCE-69CF-3242-810A-C54B3D490797}"),
                                       "awesomesecretno1willeverguess")
            );

            Assert.Throws<CryptographicException>(
                () => client.OpenUser(user.Id, "randomheckerpwdhaha")
            );
        }

        [Test]
        public void OpenUserTestNoId()
        {
            using var sbox = new SandBox();
            using var client = sbox.OpenSession();

            var user1 = client.PromoteUser("cred1");
            var user2 = client.PromoteUser("cred2");
            var user3 = client.PromoteUser("cred3");

            using UserSession userSession1 = client.OpenUser("cred1");
            using UserSession userSession2 = client.OpenUser("cred2");
            using UserSession userSession3 = client.OpenUser("cred3");

            Assert.Throws<Exception>(
                () => client.OpenUser("nonexistingpassword")
            );
        }

        [Test]
        public void AddEntryTests()
        {
            using MemoryDatabaseSession db = new MemoryDatabaseSession();
            using ClientSession session = ClientSession.Open(db);

            var user = session.PromoteUser("secretprotector");
            var users = db.EnumerateUsers().ToArray();
            using UserSession userSession = session.OpenUser(users[0].Id, "secretprotector");

            var entry = session.UpdateEntry(new Guid("{747CF732-93E4-4D9D-A929-15E05CFF0DE5}"),
                                            new EntryPayload("secret"));

            ClassicAssert.AreEqual(1, db.Database.Users.Count);
            ClassicAssert.AreEqual(1, db.Database.Entries.Count);
            ClassicAssert.AreEqual(1, db.Database.Entries.First().Value.Keys.Count);

            ClassicAssert.AreEqual(entry.Id, db.Database.Entries.First().Key);
            ClassicAssert.AreEqual(ActionInfo.Added, entry.Action);

            CollectionAssert.AreEqual(
                new UserInfo[]
                {
                    user
                },
                entry.EncodedForUsers);

            ClassicAssert.AreEqual(
                new KeyedEntry
                {
                    Id = entry.Id,
                    Payload = new EntryPayload("secret")
                },
                userSession.GetEntry(entry.Id));
        }

        [Test]
        public void HiddenEntriesTests()
        {
            using MemoryDatabaseSession db = new MemoryDatabaseSession();
            using ClientSession session = ClientSession.Open(db);

            UserInfo oldUser = session.PromoteUser("iamanoldman");

            session.UpdateEntry(new Guid("{9A7B1777-A77F-4C87-AC51-B330698EF737}"), new EntryPayload("entry1"));
            session.UpdateEntry(new Guid("{F36E862F-C445-4EDD-9D5D-7414414330D9}"), new EntryPayload("entry2"));

            UserInfo newUser = session.PromoteUser("ihatehimbutcantseehisstuff");

            session.UpdateEntry(new Guid("{77498C4F-60CC-4D6B-BDC8-204EB187AE26}"), new EntryPayload("entry3"));

            using UserSession oldSession = session.OpenUser("iamanoldman");

            CollectionAssert.AreEqual(
                new KeyedEntry[]
                {
                    new KeyedEntry
                    {
                        Id = new Guid("{9A7B1777-A77F-4C87-AC51-B330698EF737}"),
                        Payload = new EntryPayload("entry1")
                    },
                    new KeyedEntry
                    {
                        Id = new Guid("{F36E862F-C445-4EDD-9D5D-7414414330D9}"),
                        Payload = new EntryPayload("entry2")
                    },
                    new KeyedEntry
                    {
                        Id = new Guid("{77498C4F-60CC-4D6B-BDC8-204EB187AE26}"),
                        Payload = new EntryPayload("entry3")
                    },
                },
                oldSession.EnumerateEntries()
            );

            using UserSession newSession = session.OpenUser("ihatehimbutcantseehisstuff");

            CollectionAssert.AreEqual(
                new KeyedEntry[]
                {
                    new KeyedEntry
                    {
                        Id = new Guid("{9A7B1777-A77F-4C87-AC51-B330698EF737}"),
                        Payload = null
                    },
                    new KeyedEntry
                    {
                        Id = new Guid("{F36E862F-C445-4EDD-9D5D-7414414330D9}"),
                        Payload = null
                    },
                    new KeyedEntry
                    {
                        Id = new Guid("{77498C4F-60CC-4D6B-BDC8-204EB187AE26}"),
                        Payload = new EntryPayload("entry3")
                    },
                },
                newSession.EnumerateEntries()
            );
        }
    }
}
