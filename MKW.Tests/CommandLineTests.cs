using MKW.Core.Client;
using MKW.Core.Client.Notify;
using MKW.Core.Storage;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace MKW.Tests
{
    public class CommandLineTests
    {
        [Test]
        public void SimpleTest()
        {
            using var sbox = new SandBox();

            // create a test database file
            ClassicAssert.AreEqual(
                $"  -- EXIT CODE: 0\r\n",
                sbox.Run($"mkw touch {sbox.DatabasePath}")
            );

            // run the command again to ensure it opens the existing file
            ClassicAssert.AreEqual(
                $"  -- EXIT CODE: 0\r\n",
                sbox.Run($"mkw touch {sbox.DatabasePath}")
            );
        }

        [Test]
        public void AddUserTest()
        {
            using var sbox = new SandBox();

            string output = sbox.Run($"mkw add-user {sbox.DatabasePath} --password lifeishard");

            using var db = sbox.OpenDatabase();

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
            using var sbox = new SandBox();

            sbox.Run("mkw --help");
        }

        [Test]
        public void ListEntriesTest()
        {
            using var sbox = new SandBox();

            using var db = sbox.OpenDatabase();

            var session = new ClientSession(db);

            var user1 = session.PromoteUser("amogus");
            var user2 = session.PromoteUser("r34");

            session.UpdateEntry(new Guid("{9FC58C78-005D-47B6-82DA-4054D079B536}"), new EntryPayload("sus1"));
            session.UpdateEntry(new Guid("{A909CB08-25EF-4C3C-8913-959140E86BDA}"), new EntryPayload("sus2"));

            db.Dispose();

            var output = sbox.Run($"mkw entries {sbox.DatabasePath} --password amogus");

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
            using var sbox = new SandBox();

            using (var db = sbox.OpenDatabase())
            {
                ClientSession session = new ClientSession(db);

                UserInfo oldUser = session.PromoteUser("iamanoldman");

                session.UpdateEntry(new Guid("{9A7B1777-A77F-4C87-AC51-B330698EF737}"), new EntryPayload("entry1"));
                session.UpdateEntry(new Guid("{F36E862F-C445-4EDD-9D5D-7414414330D9}"), new EntryPayload("entry2"));

                UserInfo newUser = session.PromoteUser("ihatehimbutcantseehisstuff");

                session.UpdateEntry(new Guid("{77498C4F-60CC-4D6B-BDC8-204EB187AE26}"), new EntryPayload("entry3"));
            }

            {
                var output = sbox.Run($"mkw entries {sbox.DatabasePath} --password iamanoldman");

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
                var output = sbox.Run($"mkw entries {sbox.DatabasePath} --password ihatehimbutcantseehisstuff");

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
            using var sbox = new SandBox();

            sbox.Run($"mkw add-user {sbox.DatabasePath} --password test1");
            sbox.Run($"mkw add-user {sbox.DatabasePath} --password test2");

            var output1 = sbox.Run($"mkw add-entry {sbox.DatabasePath} \"secret entry no1\"");

            using var db = sbox.OpenDatabase();

            ClassicAssert.AreEqual(
                $"""
                  -- EXIT CODE: 0
                  -- STDOUT:
                Added: {db.EnumerateEntries().First().Id} for 2 users

                """,
                output1);
        }

        [Test]
        public void InteractivePromptTest()
        {
            using var sbox = new SandBox();

            sbox.Run($"mkw add-user {sbox.DatabasePath}");
            sbox.Run($"mkw add-user {sbox.DatabasePath} --non-interactive");
            sbox.Run($"mkw add-user {sbox.DatabasePath} --force-interactive", "test3");

            using var db = sbox.OpenDatabase();

            ClassicAssert.AreEqual(1, db.EnumerateUsers().Count());

            var session = new ClientSession(db);

            var user = session.OpenUser("test3");
        }
    }
}