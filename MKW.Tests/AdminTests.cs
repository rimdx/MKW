using MKW.Core.Client;
using MKW.Core.Client.Notify;
using NUnit.Framework.Legacy;

namespace MKW.Tests
{
    [TestFixture]
    public class AdminTests
    {
        [Test]
        public void AddOpenSimpleTest()
        {
            using SandBox sbox = new SandBox();
            using ClientSession client = sbox.OpenSession();

            UserInfo admin = client.PromoteAdmin("adminsecret");

            AdminSession adminSession = client.OpenAdmin("adminsecret");
        }

        [Test]
        public void UpdateTrustTest()
        {
            using SandBox sbox = new SandBox();
            using ClientSession client = sbox.OpenSession();

            UserInfo admin = client.PromoteAdmin("adminsecret");
            AdminSession adminSession = client.OpenAdmin("adminsecret");

            UserInfo user1 = client.PromoteUser("user1");
            UserInfo user2 = client.PromoteUser("user2");

            user1.Trust = Trust.None;
            user2.Trust = Trust.None;
            CollectionAssert.AreEqual(
                new UserInfo[] { user1, user2 },
                adminSession.EnumerateUsersTrust());

            adminSession.UpdateTrust(user1.Id, Trust.FullTrust);
            user1.Trust = Trust.FullTrust;
            user2.Trust = Trust.None;
            CollectionAssert.AreEqual(
                new UserInfo[] { user1, user2 },
                adminSession.EnumerateUsersTrust());

            adminSession.UpdateTrust(user1.Id, Trust.None);
            adminSession.UpdateTrust(user2.Id, Trust.FullTrust);
            user1.Trust = Trust.None;
            user2.Trust = Trust.FullTrust;
            CollectionAssert.AreEqual(
                new UserInfo[] { user1, user2 },
                adminSession.EnumerateUsersTrust());
        }
    }
}
