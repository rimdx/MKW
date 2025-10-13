using NUnit.Framework.Legacy;

namespace MKW.GUI.Tests
{
    public class SingleLineConverterTests
    {
        [Test]
        [TestCase("abc", "abc")]
        [TestCase("abc 123", "abc 123")]
        [TestCase("abc   123", "abc 123")]
        [TestCase("abc  \n\n 123", "abc 123")]
        [TestCase("abc  \n\r\n 123", "abc 123")]
        public void SimpleTest(string value, string expected)
        {
            ClassicAssert.AreEqual(expected, SingleLineConverter.CompactText(value));
        }
    }
}
