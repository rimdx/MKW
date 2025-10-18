using MKW.Core.Serialization.Pgp;
using NUnit.Framework.Legacy;

namespace MKW.Storage.Tests
{
    public class PgpArmourSerializerTests
    {
        [Test]
        public void SimpleSerializeTest()
        {
            using StringWriter writer = new StringWriter();

            PgpArmouredMessage msg = new PgpArmouredMessage
            {
                MessageTypeHeader = "PGP MESSAGE",
                Data = new byte[] { 1, 2, 3, 4 },
                Headers = [
                    new PgpArmourHeader("Version", "0.42.0"),
                ],
            };

            PgpArmouredMessageSerializer.Serialize(writer, msg);

            Console.WriteLine(writer.ToString());

            using StringReader reader = new StringReader(writer.ToString());

            PgpArmouredMessage? deserialized = PgpArmouredMessageSerializer.Deserialize(reader);

            ClassicAssert.NotNull(deserialized);
            ClassicAssert.AreEqual(msg.MessageTypeHeader, deserialized.MessageTypeHeader);
            CollectionAssert.AreEqual(msg.Data.ToArray(), deserialized.Data.ToArray());
        }

        [Test]
        [TestCase(PgpTestKeys.TestPublicKey)]
        [TestCase(PgpTestKeys.TestPrivateKey)]
        [TestCase(PgpTestKeys.PublicKeyEncryptedMessage)]
        [TestCase(PgpTestKeys.SymmetricallyEncryptedMessage)]
        public void Samples(string content)
        {
            using StringReader reader = new StringReader(content);

            PgpArmouredMessage? deserialized = PgpArmouredMessageSerializer.Deserialize(reader);
            ClassicAssert.NotNull(deserialized);

            using StringWriter writer = new StringWriter();
            writer.NewLine = "\n";

            PgpArmouredMessageSerializer.Serialize(writer, deserialized);

            Console.WriteLine(writer.ToString());

            ClassicAssert.AreEqual(content, writer.ToString());
        }
    }
}
