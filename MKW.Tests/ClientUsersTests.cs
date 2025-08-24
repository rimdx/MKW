using MKW.Core.Client;
using MKW.Core.Client.Notify;
using MKW.Core.Storage;
using NUnit.Framework.Legacy;
using System.Security.Cryptography;

namespace MKW.Tests
{
    public class ClientUsersTests
    {
        [Test]
        public void SimpleAddUserTest()
        {
            using SandBox sbox = new SandBox();
            using IDatabase db = sbox.OpenDatabase();
            using ClientSession session = ClientSession.Open(db);

            UserInfo user = session.PromoteUser("whattheheckamidoing");

            IDatabaseUser[] users = db.EnumerateUsers().ToArray();
            ClassicAssert.AreEqual(1, users.Length);

            ClassicAssert.AreEqual(user.Id, users[0].Id);
            ClassicAssert.AreEqual(user.PublicKey, users[0].PublicKey);
        }

        [Test]
        public void OpenUserTest()
        {
            using SandBox sbox = new SandBox();
            using ClientSession client = sbox.OpenSession();

            UserInfo user = client.PromoteUser("awesomesecretno1willeverguess");

            using UserSession userSession = client.OpenUser(user.Id, "awesomesecretno1willeverguess");

            Assert.Throws<Exception>(
                () => client.OpenUser(UserId.Create(),
                                      "awesomesecretno1willeverguess")
            );

            Assert.Throws<CryptographicException>(
                () => client.OpenUser(user.Id, "randomheckerpwdhaha")
            );
        }

        [Test]
        public void OpenUserTestNoId()
        {
            using SandBox sbox = new SandBox();
            using ClientSession client = sbox.OpenSession();

            UserInfo user1 = client.PromoteUser("cred1");
            UserInfo user2 = client.PromoteUser("cred2");
            UserInfo user3 = client.PromoteUser("cred3");

            using UserSession userSession1 = client.OpenUser("cred1");
            using UserSession userSession2 = client.OpenUser("cred2");
            using UserSession userSession3 = client.OpenUser("cred3");

            Assert.Throws<Exception>(
                () => client.OpenUser("nonexistingpassword")
            );
        }
    }
}
