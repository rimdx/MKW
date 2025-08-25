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

            adminInfo.Trust = Trust.ExplicitTrust;

            CollectionAssert.AreEqual(
                new UserInfo[]
                {
                },
                trustController.EnumerateExplicitlyTrustedUsersInfo());

            userInfo.Trust = Trust.SelfTrust;
            adminInfo.Trust = Trust.ImplicitTrust;

            CollectionAssert.AreEqual(
                new UserInfo[]
                {
                    userInfo,
                },
                trustController.EnumerateImplicitlyTrustedUsers().ToArray());
        }

        [Test]
        public void SimpleTrustNetworkTest()
        {
            using SandBox sbox = new SandBox();
            using ClientSession client = sbox.OpenSession();

            //            exp trust    imp trust
            // self    /---- 2 ----------- 4
            //  1 ----  
            //         \---- 3
            //            exp trust

            UserSession user1 = sbox.CreateUser(client, "user1", out UserInfo user1Info, Trust.None);
            UserSession user2 = sbox.CreateUser(client, "user2", out UserInfo user2Info, Trust.None);
            UserSession user3 = sbox.CreateUser(client, "user3", out UserInfo user3Info, Trust.None);
            UserSession user4 = sbox.CreateUser(client, "user4", out UserInfo user4Info, Trust.None);
            UserSession user5 = sbox.CreateUser(client, "user5", out UserInfo user5Info, Trust.None);
            using AdminSession admin = sbox.OpenAdmin(client);

            using UserTrustController trustController1 = new UserTrustController(client, user1);
            using UserTrustController trustController2 = new UserTrustController(client, user2);
            using UserTrustController trustController3 = new UserTrustController(client, user3);
            using UserTrustController trustController4 = new UserTrustController(client, user4);

            trustController1.UpdateTrust(user2Info.Id, Trust.ExplicitTrust);
            trustController1.UpdateTrust(user3Info.Id, Trust.ExplicitTrust);
            trustController2.UpdateTrust(user4Info.Id, Trust.ExplicitTrust);

            UserInfo adminInfo = client.GetAdminInfo();

            user1Info.Trust = Trust.ExplicitTrust;
            user2Info.Trust = Trust.ExplicitTrust;
            user3Info.Trust = Trust.ExplicitTrust;
            user4Info.Trust = Trust.ImplicitTrust;
            user5Info.Trust = Trust.None;

            CollectionAssert.AreEquivalent(
                new UserInfo[]
                {
                    user2Info,
                    user3Info,
                },
                trustController1.EnumerateExplicitlyTrustedUsersInfo());

            user1Info.Trust = Trust.SelfTrust;
            user2Info.Trust = Trust.ExplicitTrust;
            user3Info.Trust = Trust.ExplicitTrust;
            user4Info.Trust = Trust.ImplicitTrust;
            user5Info.Trust = Trust.None;

            CollectionAssert.AreEquivalent(
                new UserInfo[]
                {
                    user1Info,
                    user2Info,
                    user3Info,
                    user4Info,
                },
                trustController1.EnumerateImplicitlyTrustedUsers().ToArray());

        }

        [Test]
        public void TrustNetworkTestWithCommonNode()
        {
            using SandBox sbox = new SandBox();
            using ClientSession client = sbox.OpenSession();

            //            exp trust
            // self    /---- 2 -----\   imp trust
            //  1 ----               -----  4
            //         \---- 3 -----/
            //            exp trust

            UserSession user1 = sbox.CreateUser(client, "user1", out UserInfo user1Info, Trust.None);
            UserSession user2 = sbox.CreateUser(client, "user2", out UserInfo user2Info, Trust.None);
            UserSession user3 = sbox.CreateUser(client, "user3", out UserInfo user3Info, Trust.None);
            UserSession user4 = sbox.CreateUser(client, "user4", out UserInfo user4Info, Trust.None);
            UserSession user5 = sbox.CreateUser(client, "user5", out UserInfo user5Info, Trust.None);
            using AdminSession admin = sbox.OpenAdmin(client);

            using UserTrustController trustController1 = new UserTrustController(client, user1);
            using UserTrustController trustController2 = new UserTrustController(client, user2);
            using UserTrustController trustController3 = new UserTrustController(client, user3);
            using UserTrustController trustController4 = new UserTrustController(client, user4);

            trustController1.UpdateTrust(user2Info.Id, Trust.ExplicitTrust);
            trustController1.UpdateTrust(user3Info.Id, Trust.ExplicitTrust);
            trustController2.UpdateTrust(user4Info.Id, Trust.ExplicitTrust);
            trustController3.UpdateTrust(user4Info.Id, Trust.ExplicitTrust);

            UserInfo adminInfo = client.GetAdminInfo();

            user1Info.Trust = Trust.SelfTrust;
            user2Info.Trust = Trust.ExplicitTrust;
            user3Info.Trust = Trust.ExplicitTrust;
            user4Info.Trust = Trust.ImplicitTrust;
            user5Info.Trust = Trust.None;

            CollectionAssert.AreEquivalent(
                new UserInfo[]
                {
                    user1Info,
                    user2Info,
                    user3Info,
                    user4Info,
                    user4Info, // todo:
                },
                trustController1.EnumerateImplicitlyTrustedUsers().ToArray());
        }
    }
}
