using MKW.Core;
using MKW.Core.Client;
using MKW.Core.Client.Exceptions;
using MKW.Core.Notify;
using MKW.Core.Storage;
using MKW.Testing.Client;
using NUnit.Framework.Legacy;

namespace MKW.Tests
{
    public class ClientUsersTests
    {
        [Test]
        public void SimpleAddUserTest()
        {
            using ClientSandBox sbox = new ClientSandBox();
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
            using ClientSandBox sbox = new ClientSandBox();
            using ClientSession client = sbox.OpenSession();

            UserInfo user = client.PromoteUser("awesomesecretno1willeverguess");

            using IUserSession userSession = client.OpenUser(user.Id, "awesomesecretno1willeverguess");

            Assert.Throws<Exception>(
                () => client.OpenUser(UserId.Create(),
                                      "awesomesecretno1willeverguess")
            );

            Assert.Throws<InvalidPasswordException>(
                () => client.OpenUser(user.Id, "randomheckerpwdhaha")
            );
        }

        [Test]
        public void OpenUserTestNoId()
        {
            using ClientSandBox sbox = new ClientSandBox();
            using ClientSession client = sbox.OpenSession();

            UserInfo user1 = client.PromoteUser("cred1");
            UserInfo user2 = client.PromoteUser("cred2");
            UserInfo user3 = client.PromoteUser("cred3");

            using IUserSession userSession1 = client.OpenUser("cred1");
            using IUserSession userSession2 = client.OpenUser("cred2");
            using IUserSession userSession3 = client.OpenUser("cred3");

            Assert.Throws<Exception>(
                () => client.OpenUser("nonexistingpassword")
            );
        }
    }
}
