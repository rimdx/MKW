using MKW.Core.Client;
using MKW.Core.Client.Notify;
using MKW.Core.Storage;
using MKW.Core.Storage.JSON;
using NUnit.Framework.Legacy;

namespace MKW.Tests
{
    public class ClientEntriesTests
    {
        [Test]
        public void AddEntryTests()
        {
            using MemoryDatabaseSession db = new MemoryDatabaseSession();
            using ClientSession session = ClientSession.Open(db);

            UserInfo user = session.PromoteUser("secretprotector");
            IDatabaseUser[] users = db.EnumerateUsers().ToArray();
            using UserSession userSession = session.OpenUser(users[0].Id, "secretprotector");

            EntryInfo entry = session.UpdateEntry(new Guid("{747CF732-93E4-4D9D-A929-15E05CFF0DE5}"),
                                            new EntryPayload("secret"));

            ClassicAssert.AreEqual(1, db.Database.Users.Count);
            ClassicAssert.AreEqual(1, db.Database.Entries.Count);
            ClassicAssert.AreEqual(1, db.Database.Entries.First().Value.Keys.Count);

            ClassicAssert.AreEqual(entry.Id, db.Database.Entries.First().Key);
            ClassicAssert.AreEqual(ActionInfo.Added, entry.Action);

            user.Trust = Trust.FullTrust;
            CollectionAssert.AreEqual(
                new UserInfo[]
                {
                    user
                },
                entry.EncodedForUsers);

            using UserEntry entrySession = userSession.OpenEntry(entry.Id);

            ClassicAssert.AreEqual(new EntryPayload("secret"), entrySession.OpenPayload());
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
                new[]
                {
                    new
                    {
                        Id = new Guid("{9A7B1777-A77F-4C87-AC51-B330698EF737}"),
                        Payload = new EntryPayload("entry1")
                    },
                    new
                    {
                        Id = new Guid("{F36E862F-C445-4EDD-9D5D-7414414330D9}"),
                        Payload = new EntryPayload("entry2")
                    },
                    new
                    {
                        Id = new Guid("{77498C4F-60CC-4D6B-BDC8-204EB187AE26}"),
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
                        Id = new Guid("{9A7B1777-A77F-4C87-AC51-B330698EF737}"),
                        Payload = (EntryPayload?)null
                    },
                    new
                    {
                        Id = new Guid("{F36E862F-C445-4EDD-9D5D-7414414330D9}"),
                        Payload = (EntryPayload?)null
                    },
                    new
                    {
                        Id = new Guid("{77498C4F-60CC-4D6B-BDC8-204EB187AE26}"),
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
    }
}
