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
            using ClientSession session = ClientSession.Open(db);

            UserInfo admin = session.GetAdminInfo();

            sbox.CreateUser(session, "secretprotector", out UserInfo user).Dispose();
            IDatabaseUser[] users = db.EnumerateUsers().ToArray();
            using IUserSession userSession = session.OpenUser(users[1].Id, "secretprotector");

            EntryInfo entry = sbox.OpenAdmin(session).UpdateEntry(
                EntryId.FromGuid(new Guid("{747CF732-93E4-4D9D-A929-15E05CFF0DE5}")),
                new EntryPayload("secret"));

            ClassicAssert.AreEqual(2, db.EnumerateUsers().Count());
            ClassicAssert.AreEqual(1, db.EnumerateEntries().Count());
            ClassicAssert.AreEqual(2, db.EnumerateEntries().First().Keys.Count);

            ClassicAssert.AreEqual(entry.Id, db.EnumerateEntries().First().Id);

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

        //[Test]
        //[Ignore("im too stupid for this rn")]
        //public void HiddenEntriesTests()
        //{
        //    using ClientSandBox sbox = new ClientSandBox();
        //    using ClientSession session = sbox.OpenSession();

        //    using IUserSession oldSession = sbox.CreateUser(session, "iamanoldman", out _, false);

        //    oldSession.UpdateEntry(EntryId.FromGuid(new Guid("{9A7B1777-A77F-4C87-AC51-B330698EF737}")), new EntryPayload("entry1"));
        //    oldSession.UpdateEntry(EntryId.FromGuid(new Guid("{F36E862F-C445-4EDD-9D5D-7414414330D9}")), new EntryPayload("entry2"));

        //    CollectionAssert.AreEqual(
        //        new[]
        //        {
        //            new
        //            {
        //                Id = EntryId.FromGuid(new Guid("{9A7B1777-A77F-4C87-AC51-B330698EF737}")),
        //                Payload = new EntryPayload("entry1")
        //            },
        //            new
        //            {
        //                Id = EntryId.FromGuid(new Guid("{F36E862F-C445-4EDD-9D5D-7414414330D9}")),
        //                Payload = new EntryPayload("entry2")
        //            },
        //        },
        //        oldSession.EnumerateEntries().Select(entry => new
        //        {
        //            Id = entry.Id,
        //            Payload = entry.OpenPayload()
        //        })
        //    );

        //    using IUserSession newSession = sbox.CreateUser(session, "ihatehimbutcantseehisstuff", out _, false);

        //    newSession.UpdateEntry(EntryId.FromGuid(new Guid("{77498C4F-60CC-4D6B-BDC8-204EB187AE26}")), new EntryPayload("entry3"));

        //    CollectionAssert.AreEqual(
        //        new[]
        //        {
        //            new
        //            {
        //                Id = EntryId.FromGuid(new Guid("{9A7B1777-A77F-4C87-AC51-B330698EF737}")),
        //                Payload = (EntryPayload?)new EntryPayload("entry1")
        //            },
        //            new
        //            {
        //                Id = EntryId.FromGuid(new Guid("{F36E862F-C445-4EDD-9D5D-7414414330D9}")),
        //                Payload = (EntryPayload?)new EntryPayload("entry2")
        //            },
        //            new
        //            {
        //                Id = EntryId.FromGuid(new Guid("{77498C4F-60CC-4D6B-BDC8-204EB187AE26}")),
        //                Payload = (EntryPayload?)null
        //            },
        //        },
        //        oldSession.EnumerateEntries().Select(entry => new
        //        {
        //            Id = entry.Id,
        //            Payload = entry.OpenPayload()
        //        })
        //    );

        //    CollectionAssert.AreEqual(
        //        new[]
        //        {
        //            new
        //            {
        //                Id = EntryId.FromGuid(new Guid("{9A7B1777-A77F-4C87-AC51-B330698EF737}")),
        //                Payload = (EntryPayload?)null
        //            },
        //            new
        //            {
        //                Id = EntryId.FromGuid(new Guid("{F36E862F-C445-4EDD-9D5D-7414414330D9}")),
        //                Payload = (EntryPayload?)null
        //            },
        //            new
        //            {
        //                Id = EntryId.FromGuid(new Guid("{77498C4F-60CC-4D6B-BDC8-204EB187AE26}")),
        //                Payload = (EntryPayload?)new EntryPayload("entry3")
        //            },
        //        },
        //        newSession.EnumerateEntries().Select(entry => new
        //        {
        //            Id = entry.Id,
        //            Payload = entry.OpenPayload()
        //        })
        //    );
        //}

        [Test]
        [Ignore("todo")]
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
