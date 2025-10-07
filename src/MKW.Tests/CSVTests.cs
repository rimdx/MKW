using MKW.Core.Serialization.CSV;
using NUnit.Framework.Legacy;

namespace MKW.Tests
{
    public class CSVTests
    {
        [Test]
        public void SimpleTokenTest()
        {
            using StringReader reader = new StringReader("a,b,c");
            using CSVTokenReader tokenizer = new CSVTokenReader(reader);

            CSVToken[] tokens = [.. tokenizer.EnumerateTokens()];

            CollectionAssert.AreEqual(
                new CSVToken[]
                {
                    new CSVTokenString("a"),
                    new CSVTokenSeparator(),
                    new CSVTokenString("b"),
                    new CSVTokenSeparator(),
                    new CSVTokenString("c"),
                },
                tokens
            );
        }

        [Test]
        public void SimpleTokenTestSpaces()
        {
            using StringReader reader = new StringReader("  a  ,  b ,c");
            using CSVTokenReader tokenizer = new CSVTokenReader(reader);

            CSVToken[] tokens = [.. tokenizer.EnumerateTokens()];

            CollectionAssert.AreEqual(
                new CSVToken[]
                {
                    new CSVTokenString("  a  "),
                    new CSVTokenSeparator(),
                    new CSVTokenString("  b "),
                    new CSVTokenSeparator(),
                    new CSVTokenString("c"),
                },
                tokens
            );
        }

        [Test]
        public void SimpleTokenTestNewLines()
        {
            using StringReader reader = new StringReader("a,b,c\n  x,y,z\n\n\n\n");
            using CSVTokenReader tokenizer = new CSVTokenReader(reader);

            CSVToken[] tokens = [.. tokenizer.EnumerateTokens()];

            CollectionAssert.AreEqual(
                new CSVToken[]
                {
                    new CSVTokenString("a"),
                    new CSVTokenSeparator(),
                    new CSVTokenString("b"),
                    new CSVTokenSeparator(),
                    new CSVTokenString("c"),
                    new CSVTokenNewLine(),
                    new CSVTokenString("x"),
                    new CSVTokenSeparator(),
                    new CSVTokenString("y"),
                    new CSVTokenSeparator(),
                    new CSVTokenString("z"),
                    new CSVTokenNewLine(),
                },
                tokens
            );
        }

        [Test]
        public void SimpleTokenTestQuote()
        {
            using StringReader reader = new StringReader("\"abc xyz\",123");
            using CSVTokenReader tokenizer = new CSVTokenReader(reader);

            CSVToken[] tokens = [.. tokenizer.EnumerateTokens()];

            CollectionAssert.AreEqual(
                new CSVToken[]
                {
                    new CSVTokenString("abc xyz"),
                    new CSVTokenSeparator(),
                    new CSVTokenString("123"),
                },
                tokens
            );
        }

        [Test]
        public void SimpleTokenTestDoubleQuoteEscaping()
        {
            using StringReader reader = new StringReader("\"abc\"\"xyz\",123");
            using CSVTokenReader tokenizer = new CSVTokenReader(reader);

            CSVToken[] tokens = [.. tokenizer.EnumerateTokens()];

            CollectionAssert.AreEqual(
                new CSVToken[]
                {
                    new CSVTokenString("abc\"xyz"),
                    new CSVTokenSeparator(),
                    new CSVTokenString("123"),
                },
                tokens
            );
        }

        [Test]
        public void SerializeTest()
        {
            using StringReader reader = new StringReader("abc,xyz\n123,456");
            using CSVTokenReader tokenizer = new CSVTokenReader(reader);
            using CSVSerializer serializer = new CSVSerializer(tokenizer);

            CSVRow row = serializer.ReadRow()!;

            CollectionAssert.AreEqual(
                new[]
                {
                    new CSVField("abc"),
                    new CSVField("xyz"),
                },
                row);

            row = serializer.ReadRow()!;

            CollectionAssert.AreEqual(
                new[]
                {
                    new CSVField("123"),
                    new CSVField("456"),
                },
                row);

            ClassicAssert.IsNull(serializer.ReadRow());
        }
    }
}
