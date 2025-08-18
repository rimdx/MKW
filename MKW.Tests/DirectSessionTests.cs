using MKW.Core;
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

            session.AddUser("whattheheckamidoing");

            ClassicAssert.AreEqual(1, db.Database.Users.Count);
        }

        [Test]
        public void OpenUserTest()
        {
            MemoryDatabaseSession db = new MemoryDatabaseSession();

            ClientSession session = new ClientSession(db);

            session.AddUser("awesomesecretno1willeverguess");

            User user = db.Database.Users[0];

            UserSession userSession = session.OpenUser(user.Id, "awesomesecretno1willeverguess");

            Assert.Throws<InvalidOperationException>(
                () => session.OpenUser(new Guid("{DEADCCCE-69CF-3242-810A-C54B3D490797}"),
                                       "awesomesecretno1willeverguess")
            );

            Assert.Throws<CryptographicException>(
                () => session.OpenUser(user.Id, "randomheckerpwdhaha")
            );
        }
    }
}
