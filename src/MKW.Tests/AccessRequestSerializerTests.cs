using MKW.Core;
using MKW.Core.Client;
using MKW.Core.Serialization;
using MKW.Storage;
using MKW.Testing.Client;
using NUnit.Framework.Legacy;

namespace MKW.Tests
{
    public class AccessRequestSerializerTests
    {
        [Test]
        public void SimpleTest()
        {
            using ClientSandBox sbox = new ClientSandBox();
            using IDatabase db = sbox.OpenDatabase(); 
            using ClientSession client = sbox.OpenSession(db);

            UserAccessRequest req1 = client.CreateUserAccessRequest("abc");

            string data = UserAccessRequestSerializer.Serialize(req1);

            Console.Write(data);

            UserAccessRequest req2 = UserAccessRequestSerializer.Deserialize(data);

            CollectionAssert.AreEqual(req1.Salt.ToArray(), req2.Salt.ToArray());
            CollectionAssert.AreEqual(req1.PublicKey.ToArray(), req2.PublicKey.ToArray());
            CollectionAssert.AreEqual(req1.EncryptedPrivateKey.EncryptedPayload.ToArray(),
                                      req2.EncryptedPrivateKey.EncryptedPayload.ToArray());
            CollectionAssert.AreEqual(req1.AdminSignature.ToArray(), req2.AdminSignature.ToArray());
        }
    }
}
