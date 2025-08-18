using MKW.Core.Client;
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

            var user1 = session.AddUser("amogus");
            var user2 = session.AddUser("r34");

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
    }
}