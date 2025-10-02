using MKW.Core;
using MKW.Core.Client;
using MKW.Core.Serialization;
using MKW.Core.Storage;
using MKW.Testing.Client;
using NUnit.Framework.Legacy;

namespace MKW.Tests
{
    [TestFixture]
    public class UserMetadataSerializerTests
    {
        [Test]
        public void SimpleTest()
        {
            using ClientSandBox sbox = new ClientSandBox();
            using IDatabase db = sbox.OpenDatabase();
            using ClientSession client = sbox.OpenSession(db);

            UserMetadata metadata = new UserMetadata
            {
                UserId = "user-id",
                DisplayName = "display name",
            };

            ReadOnlyMemory<byte> serialized = UserMetadataSerializer.Serialize(metadata);

            Console.WriteLine(Convert.ToBase64String(serialized.ToArray()));

            UserMetadata deserialized = UserMetadataSerializer.Deserialize(serialized.Span);

            ClassicAssert.AreEqual(metadata, deserialized);
        }
    }
}
