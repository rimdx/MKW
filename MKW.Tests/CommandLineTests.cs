using MKW.Core.Client;
using MKW.Core.Client.Notify;
using MKW.Core.Storage;
using NUnit.Framework.Legacy;

namespace MKW.Tests
{
    public class CommandLineTests
    {
        [Test]
        public void SimpleTest()
        {
            using SandBox sbox = new SandBox();

            // create a test database file
            ClassicAssert.AreEqual(
                $"  -- EXIT CODE: 0\r\n",
                sbox.Run($"mkw create {sbox.DatabasePath} --password {sbox.AdminSecret}")
            );

            // run the command again to ensure it opens the existing file
            ClassicAssert.AreEqual(
                $"  -- EXIT CODE: 0\r\n",
                sbox.Run($"mkw create {sbox.DatabasePath} --password {sbox.AdminSecret}")
            );
        }

        [Test]
        public void AddUserTest()
        {
            using SandBox sbox = new SandBox();

            sbox.Run($"mkw create {sbox.DatabasePath} --password {sbox.AdminSecret}");
            string output = sbox.Run($"mkw user add {sbox.DatabasePath} --password lifeishard");

            using Core.Storage.IDatabase db = sbox.OpenDatabase();

            ClassicAssert.AreEqual(
                $"  -- EXIT CODE: 0\r\n" +
                $"  -- STDOUT:\r\n" +
                $"User added with ID: {db.EnumerateUsers().First().Id}\r\n",
                output
            );
        }

        [Test]
        public void HelpTest()
        {
            using SandBox sbox = new SandBox();

            sbox.Run("mkw --help");
        }

        [Test]
        public void ListEntriesTest()
        {
            using SandBox sbox = new SandBox();
            using ClientSession client = sbox.OpenSession();

            UserInfo user1 = client.PromoteUser("amogus");
            UserInfo user2 = client.PromoteUser("r34");

            client.UpdateEntry(new Guid("{9FC58C78-005D-47B6-82DA-4054D079B536}"), new EntryPayload("sus1"));
            client.UpdateEntry(new Guid("{A909CB08-25EF-4C3C-8913-959140E86BDA}"), new EntryPayload("sus2"));

            client.Dispose();

            string output = sbox.Run($"mkw entry list {sbox.DatabasePath} --password amogus");

            ClassicAssert.AreEqual(
                """
                  -- EXIT CODE: 0
                  -- STDOUT:
                -- 9fc58c78-005d-47b6-82da-4054d079b536:
                sus1
                -- a909cb08-25ef-4c3c-8913-959140e86bda:
                sus2

                """,
                output);
        }

        [Test]
        public void HiddenEntriesTests()
        {
            using SandBox sbox = new SandBox();

            using (ClientSession client = sbox.OpenSession())
            {
                UserInfo oldUser = client.PromoteUser("iamanoldman");

                client.UpdateEntry(new Guid("{9A7B1777-A77F-4C87-AC51-B330698EF737}"), new EntryPayload("entry1"));
                client.UpdateEntry(new Guid("{F36E862F-C445-4EDD-9D5D-7414414330D9}"), new EntryPayload("entry2"));

                UserInfo newUser = client.PromoteUser("ihatehimbutcantseehisstuff");

                client.UpdateEntry(new Guid("{77498C4F-60CC-4D6B-BDC8-204EB187AE26}"), new EntryPayload("entry3"));
            }

            {
                string output = sbox.Run($"mkw entry list {sbox.DatabasePath} --password iamanoldman");

                ClassicAssert.AreEqual(
                    """
                      -- EXIT CODE: 0
                      -- STDOUT:
                    -- 9a7b1777-a77f-4c87-ac51-b330698ef737:
                    entry1
                    -- f36e862f-c445-4edd-9d5d-7414414330d9:
                    entry2
                    -- 77498c4f-60cc-4d6b-bdc8-204eb187ae26:
                    entry3

                    """,
                    output);
            }

            {
                string output = sbox.Run($"mkw entry list {sbox.DatabasePath} --password ihatehimbutcantseehisstuff");

                ClassicAssert.AreEqual(
                    """
                      -- EXIT CODE: 0
                      -- STDOUT:
                    -- 9a7b1777-a77f-4c87-ac51-b330698ef737:
                    [hidden]
                    -- f36e862f-c445-4edd-9d5d-7414414330d9:
                    [hidden]
                    -- 77498c4f-60cc-4d6b-bdc8-204eb187ae26:
                    entry3

                    """,
                    output);
            }
        }

        [Test]
        public void AddEntryTest()
        {
            using SandBox sbox = new SandBox();

            sbox.Run($"mkw create {sbox.DatabasePath} --password {sbox.AdminSecret}");
            sbox.Run($"mkw user add {sbox.DatabasePath} --password test1");
            sbox.Run($"mkw user add {sbox.DatabasePath} --password test2");

            string output1 = sbox.Run($"mkw entry add {sbox.DatabasePath} data1");

            Guid entryId1;
            UserId userId1;
            using (IDatabase db = sbox.OpenDatabase())
            {
                entryId1 = db.EnumerateEntries().First().Id;
                userId1 = db.EnumerateUsers().First().Id;
            }

            ClassicAssert.AreEqual(
                $"""
                  -- EXIT CODE: 0
                  -- STDOUT:
                Added: {entryId1} for 0 users

                """,
                output1);

            sbox.Run($"mkw admin trust {sbox.DatabasePath} --password {sbox.AdminSecret} --userid {userId1}");

            ClassicAssert.AreEqual(
                $"""
                  -- EXIT CODE: 0
                  -- STDOUT:
                -- {entryId1}:
                [hidden]
                
                """,
                sbox.Run($"mkw entry list {sbox.DatabasePath} --password test1"));

            ClassicAssert.AreEqual(
                $"""
                  -- EXIT CODE: 0
                  -- STDOUT:
                -- {entryId1}:
                [hidden]

                """,
                sbox.Run($"mkw entry list {sbox.DatabasePath} --password test2"));

            string output2 = sbox.Run($"mkw entry add {sbox.DatabasePath} data2");

            Guid entryId2;
            using (IDatabase db = sbox.OpenDatabase())
            {
                entryId2 = db.EnumerateEntries().ToArray()[1].Id;
            }

            ClassicAssert.AreEqual(
                $"""
                  -- EXIT CODE: 0
                  -- STDOUT:
                -- {entryId1}:
                [hidden]
                -- {entryId2}:
                data2

                """,
                sbox.Run($"mkw entry list {sbox.DatabasePath} --password test1"));

            ClassicAssert.AreEqual(
                $"""
                  -- EXIT CODE: 0
                  -- STDOUT:
                -- {entryId1}:
                [hidden]
                -- {entryId2}:
                [hidden]

                """,
                sbox.Run($"mkw entry list {sbox.DatabasePath} --password test2"));
        }

        [Test]
        public void InteractivePromptTest()
        {
            using SandBox sbox = new SandBox();

            sbox.Run($"mkw create {sbox.DatabasePath} --password {sbox.AdminSecret}");
            sbox.Run($"mkw user add {sbox.DatabasePath}");
            sbox.Run($"mkw user add {sbox.DatabasePath} --non-interactive");
            sbox.Run($"mkw user add {sbox.DatabasePath} --force-interactive", "test3");

            using IDatabase db = sbox.OpenDatabase();

            ClassicAssert.AreEqual(1, db.EnumerateUsers().Count());

            using ClientSession session = ClientSession.Open(db);

            UserSession user = session.OpenUser("test3");
        }

        [Test]
        public void CommandLineCreatesAdmin()
        {
            using SandBox sbox = new SandBox();

            sbox.Run($"mkw create {sbox.DatabasePath} --password {sbox.AdminSecret}");

            using ClientSession client = sbox.OpenSession();
            using AdminSession admin = sbox.OpenAdmin(client);
        }
    }
}