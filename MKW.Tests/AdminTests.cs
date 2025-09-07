using MKW.Core.Client;
using MKW.Core.Client.Notify;
using MKW.Core.Storage;
using MKW.Core.Storage.JSON;
using MKW.Testing.Client;
using NUnit.Framework.Legacy;

namespace MKW.Tests
{
    [TestFixture]
    public class AdminTests
    {
        [Test]
        public void AddOpenSimpleTest()
        {
            using ClientSandBox sbox = new ClientSandBox(false);
            using JSONDatabaseSession db = JSONDatabaseSession.Create(sbox.DatabasePath);
            using ClientSession client = ClientSession.Create(db, "adminsecret");

            using AdminSession adminSession = client.OpenAdmin("adminsecret");
        }

        [Test]
        public void UpdateTrustTest()
        {
            using ClientSandBox sbox = new ClientSandBox(false);
            using JSONDatabaseSession db = JSONDatabaseSession.Create(sbox.DatabasePath);
            using ClientSession client = ClientSession.Create(db, "adminsecret");

            UserInfo admin = client.GetAdminInfo();
            AdminSession adminSession = client.OpenAdmin("adminsecret");

            UserInfo user1 = client.PromoteUser("user1");
            UserInfo user2 = client.PromoteUser("user2");

            admin.Trust = Trust.SelfTrust;
            user1.Trust = Trust.ExplicitTrust;
            user2.Trust = Trust.ExplicitTrust;

            CollectionAssert.AreEqual(
                new UserInfo[] { admin },
                client.EnumerateUsersTrust());

            adminSession.UpdateTrust(user1.Id, Trust.ExplicitTrust);
            CollectionAssert.AreEqual(
                new UserInfo[] { admin, user1 },
                client.EnumerateUsersTrust());

            adminSession.UpdateTrust(user1.Id, Trust.None);
            adminSession.UpdateTrust(user2.Id, Trust.ExplicitTrust);

            user1.Trust = Trust.None;

            CollectionAssert.AreEqual(
                new UserInfo[] { admin, user2 },
                client.EnumerateUsersTrust());
        }

        [Test]
        public void EntriesHiddenForUntrustedUsersTest()
        {
            using ClientSandBox sbox = new ClientSandBox();
            using ClientSession client = sbox.OpenSession();

            UserInfo trusted = client.PromoteUser("trusted");
            UserInfo untrusted = client.PromoteUser("untrusted");

            using AdminSession admin = sbox.OpenAdmin(client);

            admin.UpdateTrust(trusted.Id, Trust.ExplicitTrust);
            EntryInfo entry = client.UpdateEntry(EntryId.Create(), new EntryPayload("test data"));

            {
                using UserSession user = client.OpenUser("trusted");
                using IEntrySession entrySession = user.OpenEntry(entry.Id);

                ClassicAssert.AreEqual(new EntryPayload("test data"), entrySession.OpenPayload());
            }

            {
                using UserSession user = client.OpenUser("untrusted");
                using IEntrySession entrySession = user.OpenEntry(entry.Id);

                ClassicAssert.AreEqual(null, entrySession.OpenPayload());
            }
        }

        [Test]
        public void OpenAdminAsUser()
        {
            using ClientSandBox sbox = new ClientSandBox();
            using ClientSession client = sbox.OpenSession();

            using (UserSession admin = client.OpenUser(UserId.Admin(), sbox.AdminSecret))
            {
            }

            using (UserSession admin = client.OpenUser(sbox.AdminSecret))
            {
            }
        }

        [Test]
        public void NewEntriesAreSharedWithAdminTest()
        {
            using ClientSandBox sbox = new ClientSandBox();
            using ClientSession client = sbox.OpenSession();

            EntryId entryId;

            {
                using UserSession user = sbox.CreateUser(client, "user1", out _);
                using IEntrySession entry = user.CreateEntry();
                entry.UpdatePayload(new EntryPayload("data"));
                entryId = entry.Id;
            }

            {
                using UserSession admin = sbox.OpenAdmin(client);
                using IEntrySession entry = admin.OpenEntry(entryId);

                ClassicAssert.AreEqual(new EntryPayload("data"),
                                       entry.OpenPayload());
            }
        }

        [Test]
        public void NewEntriesAreAlwaysSharedWithMeEvenThoughNooneTrustsMeFineIHaveNoIdeaHowToMakeTheTitleBiggerTest()
        {
            using ClientSandBox sbox = new ClientSandBox();
            using ClientSession client = sbox.OpenSession();

            using UserSession user = sbox.CreateUser(client, "user1", out UserInfo userInfo, false);

            EntryId entryId = EntryId.Create();

            {
                using IEntrySession entry = user.CreateEntry(entryId);
                entry.UpdatePayload(new EntryPayload("data"));

                UserInfo[] users = [.. entry.EnumerateEncodedForUsers()];
                ClassicAssert.AreEqual(1, users.Length);
                ClassicAssert.AreEqual(userInfo.Id, users[0].Id);
                ClassicAssert.AreEqual(Trust.Unknown, users[0].Trust);
            }

            {
                using IEntrySession entry = user.OpenEntry(entryId);

                UserInfo[] users = [.. entry.EnumerateEncodedForUsers()];
                ClassicAssert.AreEqual(1, users.Length);
                ClassicAssert.AreEqual(userInfo.Id, users[0].Id);
                ClassicAssert.AreEqual(Trust.Unknown, users[0].Trust);

                ClassicAssert.AreEqual(new EntryPayload("data"),
                                       entry.OpenPayload());
            }
        }

        [Test]
        public void ClientMustFailOperationIfNoAdminExist()
        {
            using ClientSandBox sbox = new ClientSandBox(false);
            using JSONDatabaseSession db = JSONDatabaseSession.Create(sbox.DatabasePath);

            Assert.Throws<Exception>(() => ClientSession.Open(db));

            // todo: maybe do this somehow?

            // Assert.Throws<Exception>(() => client.PromoteUser("user"));
            // using Entry entry = client.CreateEntry();
            // Assert.Throws<Exception>(() => entry.UpdatePayload(new EntryPayload("123")));
            // Assert.Throws<Exception>(() => client.OpenAdmin("123"));
        }

        [Test]
        public void ShareEntriesWithNewUsersTest()
        {
            using ClientSandBox sbox = new ClientSandBox();
            using ClientSession client = sbox.OpenSession();

            using UserSession admin = sbox.OpenAdmin(client);

            EntryId entryId;

            {
                using IEntrySession entry = admin.CreateEntry();
                entry.UpdatePayload(new EntryPayload("data"));
                entryId = entry.Id;
            }

            using UserSession user = sbox.CreateUser(client, "user1", out _);

            {
                using IEntrySession entry = user.OpenEntry(entryId);
                ClassicAssert.AreEqual(null, entry.OpenPayload());
            }

            {
                using IEntrySession entry = admin.OpenEntry(entryId);
                entry.Share(user.Id);
            }

            {
                using IEntrySession entry = user.OpenEntry(entryId);
                ClassicAssert.AreEqual(new EntryPayload("data"),
                                       entry.OpenPayload());
            }
        }
    }
}
