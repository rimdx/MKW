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
    }
}