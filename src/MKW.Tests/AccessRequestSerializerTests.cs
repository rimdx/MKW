using MKW.Core;
using MKW.Core.Client;
using MKW.Testing.Client;
using NUnit.Framework.Legacy;

namespace MKW.Tests
{
    public class AccessRequestSerializerTests<T>
    {
        [Test]
        public void SimpleTest()
        {
            using ClientSandBox sbox = new ClientSandBox();
            using ClientSession client = sbox.OpenSession();

            UserAccessRequest req1 = client.CreateUserAccessRequest("abc");

            ReadOnlyMemory<byte> data = UserAccessRequestSerializer.Serialize(req1);
            UserAccessRequest req2 = UserAccessRequestSerializer.Deserialize(data.Span);

            CollectionAssert.AreEqual(req1.Salt.ToArray(), req2.Salt.ToArray());
            CollectionAssert.AreEqual(req1.PublicKey.ToArray(), req2.PublicKey.ToArray());
            CollectionAssert.AreEqual(req1.EncryptedPrivateKey.ToArray(), req2.EncryptedPrivateKey.ToArray());
            CollectionAssert.AreEqual(req1.AdminSignature.ToArray(), req2.AdminSignature.ToArray());
        }
    }
}
