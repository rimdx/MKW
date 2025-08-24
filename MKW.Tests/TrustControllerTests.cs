using MKW.Core.Client;
using MKW.Core.Client.Notify;
using NUnit.Framework.Legacy;

namespace MKW.Tests
{
    public class TrustControllerTests
    {
        [Test]
        public void SimpleTest()
        {
            using SandBox sbox = new SandBox();
            using ClientSession client = sbox.OpenSession();

            using UserSession user = sbox.CreateUser(client, "123", out UserInfo userInfo, Trust.None);
            using AdminSession admin = sbox.OpenAdmin(client);

            using UserTrustController trustController = new UserTrustController(client, user);

            UserInfo adminInfo = client.GetAdminInfo();

            adminInfo.Trust = Trust.FullTrust;
            userInfo.Trust = Trust.None;

            CollectionAssert.AreEqual(
                new UserInfo[]
                {
                    adminInfo,
                    userInfo
                },
                trustController.EnumerateUsersTrust());
        }
    }
}
