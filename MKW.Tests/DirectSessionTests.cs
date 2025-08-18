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

            var user = session.AddUser("whattheheckamidoing");

            ClassicAssert.AreEqual(1, db.Database.Users.Count);

            ClassicAssert.AreEqual(user.Id, db.Database.Users[0].Id);
            ClassicAssert.AreEqual(user.PublicKey, db.Database.Users[0].PublicKey);
        }

        [Test]
        public void OpenUserTest()
        {
            MemoryDatabaseSession db = new MemoryDatabaseSession();

            ClientSession session = new ClientSession(db);

            var user = session.AddUser("awesomesecretno1willeverguess");

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
        public void AddEntryTests()
        {
            MemoryDatabaseSession db = new MemoryDatabaseSession();

            ClientSession session = new ClientSession(db);
            var user = session.AddUser("protectmyballs");
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

            ClassicAssert.AreEqual("balls", userSession.GetEntry(entry.Id).ToString());
        }
    }
}
