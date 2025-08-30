using MKW.Core.Client;
using MKW.Core.Client.Notify;
using MKW.Core.Storage;
using NUnit.Framework.Legacy;

namespace MKW.Tests
{
    public class ClientEntriesTests
    {
        [Test]
        public void AddEntryTests()
        {
            using SandBox sbox = new SandBox();
            using IDatabase db = sbox.OpenDatabase();
            using ClientSession session = ClientSession.Open(db);

            UserInfo admin = session.GetAdminInfo();

            sbox.CreateUser(session, "secretprotector", out UserInfo user).Dispose();
            IDatabaseUser[] users = db.EnumerateUsers().ToArray();
            using UserSession userSession = session.OpenUser(users[0].Id, "secretprotector");

            EntryInfo entry = session.UpdateEntry(EntryId.FromGuid(new Guid("{747CF732-93E4-4D9D-A929-15E05CFF0DE5}")),
                                                  new EntryPayload("secret"));

            ClassicAssert.AreEqual(1, db.EnumerateUsers().Count());
            ClassicAssert.AreEqual(1, db.EnumerateEntries().Count());
            ClassicAssert.AreEqual(2, db.EnumerateEntries().First().Keys.Count);

            ClassicAssert.AreEqual(entry.Id, db.EnumerateEntries().First().Id);
            ClassicAssert.AreEqual(ActionInfo.Added, entry.Action);

            admin.Trust = Trust.SelfTrust;
            user.Trust = Trust.ExplicitTrust;
            CollectionAssert.AreEqual(
                new UserInfo[]
                {
                    admin,
                    user
                },
                entry.EncodedForUsers);

            ClassicAssert.AreEqual(new EntryPayload("secret"),
                                   userSession.OpenEntry(entry.Id).OpenPayload());
        }

        [Test]
        public void HiddenEntriesTests()
        {
            using SandBox sbox = new SandBox();
            using ClientSession session = sbox.OpenSession();

            sbox.CreateUser(session, "iamanoldman", out UserInfo oldUser).Dispose();

            session.UpdateEntry(EntryId.FromGuid(new Guid("{9A7B1777-A77F-4C87-AC51-B330698EF737}")), new EntryPayload("entry1"));
            session.UpdateEntry(EntryId.FromGuid(new Guid("{F36E862F-C445-4EDD-9D5D-7414414330D9}")), new EntryPayload("entry2"));

            sbox.CreateUser(session, "ihatehimbutcantseehisstuff", out UserInfo newUser).Dispose();

            session.UpdateEntry(EntryId.FromGuid(new Guid("{77498C4F-60CC-4D6B-BDC8-204EB187AE26}")), new EntryPayload("entry3"));

            using UserSession oldSession = session.OpenUser("iamanoldman");

            CollectionAssert.AreEqual(
                new[]
                {
                    new
                    {
                        Id = EntryId.FromGuid(new Guid("{9A7B1777-A77F-4C87-AC51-B330698EF737}")),
                        Payload = new EntryPayload("entry1")
                    },
                    new
                    {
                        Id = EntryId.FromGuid(new Guid("{F36E862F-C445-4EDD-9D5D-7414414330D9}")),
                        Payload = new EntryPayload("entry2")
                    },
                    new
                    {
                        Id = EntryId.FromGuid(new Guid("{77498C4F-60CC-4D6B-BDC8-204EB187AE26}")),
                        Payload = new EntryPayload("entry3")
                    },
                },
                oldSession.EnumerateEntries().Select(entry => new
                {
                    Id = entry.Id,
                    Payload = entry.OpenPayload()
                })
            );

            using UserSession newSession = session.OpenUser("ihatehimbutcantseehisstuff");

            CollectionAssert.AreEqual(
                new[]
                {
                    new
                    {
                        Id = EntryId.FromGuid(new Guid("{9A7B1777-A77F-4C87-AC51-B330698EF737}")),
                        Payload = (EntryPayload?)null
                    },
                    new
                    {
                        Id = EntryId.FromGuid(new Guid("{F36E862F-C445-4EDD-9D5D-7414414330D9}")),
                        Payload = (EntryPayload?)null
                    },
                    new
                    {
                        Id = EntryId.FromGuid(new Guid("{77498C4F-60CC-4D6B-BDC8-204EB187AE26}")),
                        Payload = (EntryPayload?)new EntryPayload("entry3")
                    },
                },
                newSession.EnumerateEntries().Select(entry => new
                {
                    Id = entry.Id,
                    Payload = entry.OpenPayload()
                })
            );
        }

        [Test]
        public void ClientEntryAPITest()
        {
            // init
            using SandBox sbox = new SandBox();
            using ClientSession client = sbox.OpenSession();

            client.PromoteUser("usersecret");
            using UserSession user = client.OpenUser("usersecret");
            using AdminSession admin = sbox.OpenAdmin(client);
            admin.UpdateTrust(user.Id, Trust.ExplicitTrust);

            // create
            using IEntry entry = client.CreateEntry();

            ClassicAssert.AreNotEqual(EntryId.FromGuid(Guid.Empty), entry.Id);
            ClassicAssert.AreEqual(null,
                                   user.OpenEntry(entry.Id).OpenPayload());

            //UserInfo[] users = [
            //    new UserInfo
            //    {
            //        Id = user.Id,
            //        PublicKey = client.
            //        Trust = Trust.Unknown,
            //    }
            //];

            // initial update

            // todo: assert notify info
            //ClassicAssert.AreEqual(new EntryInfo
            //{
            //    Id = entry.Id,
            //    Action = ActionInfo.Updated, // todo: added?
            //    EncodedForUsers = new[]
            //    {
            //    }
            //},

            entry.UpdatePayload(new EntryPayload("data1"));

            ClassicAssert.AreEqual(new EntryPayload("data1"),
                                   user.OpenEntry(entry.Id).OpenPayload());

            // another update

            entry.UpdatePayload(new EntryPayload("data2"));

            ClassicAssert.AreEqual(new EntryPayload("data2"),
                                   user.OpenEntry(entry.Id).OpenPayload());

            // create with same id
            Assert.Throws<Exception>(() => client.CreateEntry(entry.Id));

            // delete
            client.DeleteEntry(entry.Id);
            Assert.Throws<Exception>(() => user.OpenEntry(entry.Id));
            client.DeleteEntry(entry.Id);
            Assert.Throws<Exception>(() => user.OpenEntry(entry.Id));
        }

        [Test]
        public void ClientUpdateEntryAPITest()
        {
            // init
            using SandBox sbox = new SandBox();
            using ClientSession client = sbox.OpenSession();

            using UserSession user = sbox.CreateUser(client, "usersecret", out _);

            // create
            EntryId id = EntryId.Create();
            EntryInfo entry = client.UpdateEntry(id, new EntryPayload("data1"));

            ClassicAssert.AreEqual(id, entry.Id);
            ClassicAssert.AreEqual(new EntryPayload("data1"),
                                   user.OpenEntry(entry.Id).OpenPayload());

            // update
            client.UpdateEntry(id, new EntryPayload("data2"));
            ClassicAssert.AreEqual(new EntryPayload("data2"),
                                   user.OpenEntry(entry.Id).OpenPayload());

            // delete
            client.UpdateEntry(id, null);
            Assert.Throws<Exception>(() => user.OpenEntry(entry.Id));
            client.UpdateEntry(id, null);
            Assert.Throws<Exception>(() => user.OpenEntry(entry.Id));
        }

        [Test]
        public void UserEntryAPITest()
        {
            // init
            using SandBox sbox = new SandBox();
            EntryId entryId;

            using (ClientSession client = sbox.OpenSession())
            {
                using UserSession user = sbox.CreateUser(client, "usersecret", out _);

                // create
                using IEntry entry = user.CreateEntry();
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
                using UserSession user = client.OpenUser("usersecret");
                using IEntry entry = user.OpenEntry(entryId);
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
