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
                "  -- EXIT CODE: 0\r\n" +
                "  -- STDOUT:\r\n" +
                "[Verbose] Initializing database file at ./test.json.\r\n" +
                "[Verbose] Successfully opened database file.\r\n",
                sbox.Run("mkw ./test.json")
            );

            // run the command again to ensure it opens the existing file
            ClassicAssert.AreEqual(
                "  -- EXIT CODE: 0\r\n" +
                "  -- STDOUT:\r\n" +
                "[Verbose] Opening database file: ./test.json.\r\n" +
                "[Verbose] Successfully opened database file.\r\n",
                sbox.Run("mkw ./test.json")
            );
        }
    }
}