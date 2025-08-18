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
                $"  -- EXIT CODE: 0\r\n" +
                $"  -- STDOUT:\r\n" +
                $"[Verbose] Opening database file: '{sbox.DatabasePath}'.\r\n" +
                $"[Verbose] Successfully opened database file.\r\n" +
                $"[Verbose] Successfully closed database file.\r\n",
                sbox.Run($"mkw {sbox.DatabasePath}")
            );

            // run the command again to ensure it opens the existing file
            ClassicAssert.AreEqual(
                $"  -- EXIT CODE: 0\r\n" +
                $"  -- STDOUT:\r\n" +
                $"[Verbose] Opening database file: '{sbox.DatabasePath}'.\r\n" +
                $"[Verbose] Successfully opened database file.\r\n" +
                $"[Verbose] Successfully closed database file.\r\n",
                sbox.Run($"mkw {sbox.DatabasePath}")
            );
        }

        [Test]
        public void AddUserTest()
        {
            using var sbox = new SandBox();
            // add a user to the database
            ClassicAssert.AreEqual(
                $"  -- EXIT CODE: 0\r\n" +
                $"  -- STDOUT:\r\n" +
                $"[Verbose] Opening database file: '{sbox.DatabasePath}'.\r\n" +
                $"[Verbose] Successfully opened database file.\r\n" +
                $"[Verbose] Adding user with password: 'lifeishard'.\r\n" +
                $"[Verbose] User added successfully.\r\n" +
                $"[Verbose] Successfully closed database file.\r\n",
                sbox.Run($"mkw {sbox.DatabasePath} --add-user lifeishard")
            );
        }
    }
}