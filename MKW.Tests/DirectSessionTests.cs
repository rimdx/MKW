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
            Database db = new Database()
            {
                Users = new List<User>()
            };

            DatabaseSession session = new DatabaseSession(db);

            session.AddUser("whattheheckamidoing");

            ClassicAssert.AreEqual(1, db.Users.Count);
        }

        [Test]
        public void OpenUserTest()
        {
            Database db = new Database()
            {
                Users = new List<User>()
            };

            DatabaseSession session = new DatabaseSession(db);

            session.AddUser("awesomesecretno1willeverguess");

            User user = db.Users[0];

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
