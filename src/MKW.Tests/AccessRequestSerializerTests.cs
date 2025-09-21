using MKW.Core;
using MKW.Core.Client;
using MKW.Core.Client.AccessRequest;
using MKW.Testing.Client;
using NUnit.Framework.Legacy;

namespace MKW.Tests
{
    [TestFixture(typeof(JSONAccessRequestSerializer))]
    public class AccessRequestSerializerTests<T>
    {
        private readonly IAccessRequestSerializer serializer;

        public AccessRequestSerializerTests()
        {
            serializer = (IAccessRequestSerializer)Activator.CreateInstance(typeof(T))!;
        }

        [Test]
        public void SimpleTest()
        {
            using ClientSandBox sbox = new ClientSandBox();
            using ClientSession client = sbox.OpenSession();

            UserAccessRequest req1 = client.CreateUserAccessRequest("abc");

            ReadOnlyMemory<byte> data = serializer.Serialize(req1);
            UserAccessRequest req2 = serializer.Deserialize(data.Span);

            CollectionAssert.AreEqual(req1.Salt.ToArray(), req2.Salt.ToArray());
            CollectionAssert.AreEqual(req1.PublicKey.ToArray(), req2.PublicKey.ToArray());
            CollectionAssert.AreEqual(req1.EncryptedPrivateKey.ToArray(), req2.EncryptedPrivateKey.ToArray());
            CollectionAssert.AreEqual(req1.AdminSignature.ToArray(), req2.AdminSignature.ToArray());
        }
    }
}
