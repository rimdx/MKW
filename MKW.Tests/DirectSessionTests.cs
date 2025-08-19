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
            MemoryDatabaseSession db = new MemoryDatabaseSession();

            ClientSession session = new ClientSession(db);

            var user = session.PromoteUser("whattheheckamidoing");

            ClassicAssert.AreEqual(1, db.Database.Users.Count);

            ClassicAssert.AreEqual(user.Id, db.Database.Users[0].Id);
            ClassicAssert.AreEqual(user.PublicKey, db.Database.Users[0].PublicKey);
        }

        [Test]
        public void OpenUserTest()
        {
            MemoryDatabaseSession db = new MemoryDatabaseSession();

            ClientSession session = new ClientSession(db);

            var user = session.PromoteUser("awesomesecretno1willeverguess");

            UserSession userSession = session.OpenUser(user.Id, "awesomesecretno1willeverguess");

            Assert.Throws<InvalidOperationException>(
                () => session.OpenUser(new Guid("{DEADCCCE-69CF-3242-810A-C54B3D490797}"),
                                       "awesomesecretno1willeverguess")
            );

            Assert.Throws<CryptographicException>(
                () => session.OpenUser(user.Id, "randomheckerpwdhaha")
            );
        }

        [Test]
        public void OpenUserTestNoId()
        {
            MemoryDatabaseSession db = new MemoryDatabaseSession();

            ClientSession session = new ClientSession(db);

            var user1 = session.PromoteUser("cred1");
            var user2 = session.PromoteUser("cred2");
            var user3 = session.PromoteUser("cred3");

            UserSession userSession1 = session.OpenUser("cred1");
            UserSession userSession2 = session.OpenUser("cred2");
            UserSession userSession3 = session.OpenUser("cred3");

            Assert.Throws<Exception>(
                () => session.OpenUser("nonexistingpassword")
            );
        }

        [Test]
        public void AddEntryTests()
        {
            MemoryDatabaseSession db = new MemoryDatabaseSession();

            ClientSession session = new ClientSession(db);
            var user = session.PromoteUser("protectmyballs");
            UserSession userSession = session.OpenUser(db.Database.Users[0].Id, "protectmyballs");

            var entry = session.UpdateEntry(new Guid("{747CF732-93E4-4D9D-A929-15E05CFF0DE5}"),
                                            new EntryPayload("balls"));

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
                    Payload = new EntryPayload("balls")
                },
                userSession.GetEntry(entry.Id));
        }

        [Test]
        public void HiddenEntriesTests()
        {
            MemoryDatabaseSession db = new MemoryDatabaseSession();
            ClientSession session = new ClientSession(db);

            UserInfo oldUser = session.PromoteUser("iamanoldman");

            session.UpdateEntry(new Guid("{9A7B1777-A77F-4C87-AC51-B330698EF737}"), new EntryPayload("entry1"));
            session.UpdateEntry(new Guid("{F36E862F-C445-4EDD-9D5D-7414414330D9}"), new EntryPayload("entry2"));

            UserInfo newUser = session.PromoteUser("ihatehimbutcantseehisstuff");

            session.UpdateEntry(new Guid("{77498C4F-60CC-4D6B-BDC8-204EB187AE26}"), new EntryPayload("entry3"));

            UserSession oldSession = session.OpenUser("iamanoldman");

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

            UserSession newSession = session.OpenUser("ihatehimbutcantseehisstuff");

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
