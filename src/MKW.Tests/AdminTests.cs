using MKW.Core;
using MKW.Core.Client;
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

            UserMetadata metadata = new UserMetadata
            {
                DisplayName = "admin",
                UserId = "admin@contoso.com"
            };

            using ClientSession client = ClientSession.Create(db, sbox.AdminSecret, metadata);

            using IAdminSession adminSession = client.OpenAdmin(sbox.AdminSecret);
        }

        //[Test]
        //public void EntriesHiddenForUntrustedUsersTest()
        //{
        //    using ClientSandBox sbox = new ClientSandBox();
        //    using ClientSession client = sbox.OpenSession();

        //    using IUserSession trusted = sbox.CreateUser(client, "trusted", out _, false);
        //    using IUserSession untrusted = sbox.CreateUser(client, "untrusted", out _, false);

        //    using IAdminSession admin = sbox.OpenAdmin(client);

        //    admin.AddTrust(trusted.Id);
        //    EntryInfo entry = admin.UpdateEntry(EntryId.Create(), new EntryPayload("test data"));

        //    {
        //        using IUserSession user = client.OpenUser("trusted");
        //        using IEntrySession entrySession = user.OpenEntry(entry.Id);

        //        ClassicAssert.AreEqual(new EntryPayload("test data"), entrySession.OpenPayload());
        //    }

        //    {
        //        using IUserSession user = client.OpenUser("untrusted");
        //        using IEntrySession entrySession = user.OpenEntry(entry.Id);

        //        ClassicAssert.AreEqual(null, entrySession.OpenPayload());
        //    }
        //}

        [Test]
        public void OpenAdminAsUser()
        {
            using ClientSandBox sbox = new ClientSandBox();
            using IDatabase db = sbox.OpenDatabase(); 
            using ClientSession client = sbox.OpenSession(db);

            using (IUserSession admin = client.OpenUser(UserId.Admin(), sbox.AdminSecret))
            {
            }

            using (IUserSession admin = client.OpenUser(sbox.AdminSecret))
            {
            }
        }

        [Test]
        public void NewEntriesAreSharedWithAdminTest()
        {
            using ClientSandBox sbox = new ClientSandBox();
            using IDatabase db = sbox.OpenDatabase(); 
            using ClientSession client = sbox.OpenSession(db);

            EntryId entryId;

            {
                using IUserSession user = sbox.CreateUser(client, "user1", out _);
                using IEntrySession entry = user.CreateEntry();
                entry.UpdatePayload(new EntryPayload("data"));
                entryId = entry.Id;
            }

            {
                using IAdminSession admin = sbox.OpenAdmin(client);
                using IEntrySession entry = admin.OpenEntry(entryId);

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
        public void ShareAllEntriesWithNewUsers()
        {
            using ClientSandBox sbox = new ClientSandBox();
            using IDatabase db = sbox.OpenDatabase(); 
            using ClientSession client = sbox.OpenSession(db);

            using IAdminSession admin = sbox.OpenAdmin(client);

            using IEntrySession entry1 = admin.CreateEntry();
            entry1.UpdatePayload(new EntryPayload("data1"));

            using IEntrySession entry2 = admin.CreateEntry();
            entry2.UpdatePayload(new EntryPayload("data2"));

            using IEntrySession entry3 = admin.CreateEntry();
            entry3.UpdatePayload(new EntryPayload("data3"));

            using IUserSession user = sbox.CreateUser(client, "user1", out _);

            CollectionAssert.AreEquivalent(
                new[]
                {
                    new EntryPayload("data1"),
                    new EntryPayload("data2"),
                    new EntryPayload("data3"),
                },
                user.EnumerateEntries().Select(value => value.OpenPayload()));
        }
    }
}
