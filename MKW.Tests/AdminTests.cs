using MKW.Core.Client;
using MKW.Core.Client.Notify;
using MKW.Core.Storage;
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
                client.EnumerateUsersTrust());

            adminSession.UpdateTrust(user1.Id, Trust.FullTrust);
            user1.Trust = Trust.FullTrust;
            user2.Trust = Trust.None;
            CollectionAssert.AreEqual(
                new UserInfo[] { user1, user2 },
                client.EnumerateUsersTrust());

            adminSession.UpdateTrust(user1.Id, Trust.None);
            adminSession.UpdateTrust(user2.Id, Trust.FullTrust);
            user1.Trust = Trust.None;
            user2.Trust = Trust.FullTrust;
            CollectionAssert.AreEqual(
                new UserInfo[] { user1, user2 },
                client.EnumerateUsersTrust());
        }

        [Test]
        public void EntriesHiddenForUntrustedUsersTest()
        {
            using SandBox sbox = new SandBox();
            using ClientSession client = sbox.OpenSession();

            UserInfo trusted = client.PromoteUser("trusted");
            UserInfo untrusted = client.PromoteUser("untrusted");

            client.PromoteAdmin("admin");
            using AdminSession admin = client.OpenAdmin("admin");

            admin.UpdateTrust(trusted.Id, Trust.FullTrust);
            EntryInfo entry = client.UpdateEntry(EntryId.Create(), new EntryPayload("test data"));

            {
                using UserSession user = client.OpenUser("trusted");
                using UserEntry entrySession = user.OpenEntry(entry.Id);

                ClassicAssert.AreEqual(new EntryPayload("test data"), entrySession.OpenPayload());
            }

            {
                using UserSession user = client.OpenUser("untrusted");
                using UserEntry entrySession = user.OpenEntry(entry.Id);

                ClassicAssert.AreEqual(null, entrySession.OpenPayload());
            }
        }

        [Test]
        public void OpenAdminAsUser()
        {
            using SandBox sbox = new SandBox();
            using ClientSession client = sbox.OpenSession();

            client.PromoteAdmin("adminsecret");

            using (UserSession admin = client.OpenUser(UserId.Admin(), "adminsecret"))
            {
            }

            using (UserSession admin = client.OpenUser("adminsecret"))
            {
            }
        }
    }
}
