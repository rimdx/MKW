using MKW.Core;
using MKW.Core.Client;
using MKW.Core.Notify;
using MKW.Testing.Client;
using NUnit.Framework.Legacy;

namespace MKW.Tests
{
    public class TrustControllerTests
    {
        [Test]
        public void SimpleTest()
        {
            using ClientSandBox sbox = new ClientSandBox();
            using ClientSession client = sbox.OpenSession();

            using IUserSession user = sbox.CreateUser(client, "123", out UserInfo userInfo, false);
            using IUserSession admin = sbox.OpenAdmin(client);

            using UserTrustController trustController = new UserTrustController(client, sbox.Crypto, (UserSession)user);

            UserInfo adminInfo = client.GetAdminInfo();

            adminInfo.Trust = Trust.ExplicitTrust;

            CollectionAssert.AreEqual(
                new UserInfo[]
                {
                },
                trustController.EnumerateExplicitlyTrustedUsers());

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
            using ClientSandBox sbox = new ClientSandBox();
            using ClientSession client = sbox.OpenSession();

            //            exp trust    imp trust
            // self    /---- 2 ----------- 4
            //  1 ----  
            //         \---- 3
            //            exp trust

            IUserSession user1 = sbox.CreateUser(client, "user1", out UserInfo user1Info, false);
            IUserSession user2 = sbox.CreateUser(client, "user2", out UserInfo user2Info, false);
            IUserSession user3 = sbox.CreateUser(client, "user3", out UserInfo user3Info, false);
            IUserSession user4 = sbox.CreateUser(client, "user4", out UserInfo user4Info, false);
            IUserSession user5 = sbox.CreateUser(client, "user5", out UserInfo user5Info, false);
            using IUserSession admin = sbox.OpenAdmin(client);

            using UserTrustController trustController1 = new UserTrustController(client, sbox.Crypto, (UserSession)user1);
            using UserTrustController trustController2 = new UserTrustController(client, sbox.Crypto, (UserSession)user2);
            using UserTrustController trustController3 = new UserTrustController(client, sbox.Crypto, (UserSession)user3);
            using UserTrustController trustController4 = new UserTrustController(client, sbox.Crypto, (UserSession)user4);

            trustController1.AddTrust(user2Info.Id);
            trustController1.AddTrust(user3Info.Id);
            trustController2.AddTrust(user4Info.Id);

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
                trustController1.EnumerateExplicitlyTrustedUsers());

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
            using ClientSandBox sbox = new ClientSandBox();
            using ClientSession client = sbox.OpenSession();

            //            exp trust
            // self    /---- 2 -----\   imp trust
            //  1 ----               -----  4
            //         \---- 3 -----/
            //            exp trust

            IUserSession user1 = sbox.CreateUser(client, "user1", out UserInfo user1Info, false);
            IUserSession user2 = sbox.CreateUser(client, "user2", out UserInfo user2Info, false);
            IUserSession user3 = sbox.CreateUser(client, "user3", out UserInfo user3Info, false);
            IUserSession user4 = sbox.CreateUser(client, "user4", out UserInfo user4Info, false);
            IUserSession user5 = sbox.CreateUser(client, "user5", out UserInfo user5Info, false);
            using IUserSession admin = sbox.OpenAdmin(client);

            using UserTrustController trustController1 = new UserTrustController(client, sbox.Crypto, (UserSession)user1);
            using UserTrustController trustController2 = new UserTrustController(client, sbox.Crypto, (UserSession)user2);
            using UserTrustController trustController3 = new UserTrustController(client, sbox.Crypto, (UserSession)user3);
            using UserTrustController trustController4 = new UserTrustController(client, sbox.Crypto, (UserSession)user4);

            trustController1.AddTrust(user2Info.Id);
            trustController1.AddTrust(user3Info.Id);
            trustController2.AddTrust(user4Info.Id);
            trustController3.AddTrust(user4Info.Id);

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
                },
                trustController1.EnumerateImplicitlyTrustedUsers().ToArray());
        }

        [Test]
        public void TrustNetworkTestLoops()
        {
            using ClientSandBox sbox = new ClientSandBox();
            using ClientSession client = sbox.OpenSession();

            // 1 --> 2
            // 2 --> 1
            // it's a loop!

            IUserSession user1 = sbox.CreateUser(client, "user1", out UserInfo user1Info, false);
            IUserSession user2 = sbox.CreateUser(client, "user2", out UserInfo user2Info, false);
            IUserSession user3 = sbox.CreateUser(client, "user3", out UserInfo user3Info, false);
            using IUserSession admin = sbox.OpenAdmin(client);

            using UserTrustController trustController1 = new UserTrustController(client, sbox.Crypto, (UserSession)user1);
            using UserTrustController trustController2 = new UserTrustController(client, sbox.Crypto, (UserSession)user2);

            trustController1.AddTrust(user2Info.Id);
            trustController2.AddTrust(user1Info.Id);

            UserInfo adminInfo = client.GetAdminInfo();

            user1Info.Trust = Trust.SelfTrust;
            user2Info.Trust = Trust.ExplicitTrust;
            user3Info.Trust = Trust.ExplicitTrust;

            CollectionAssert.AreEquivalent(
                new UserInfo[]
                {
                    user1Info,
                    user2Info,
                },
                trustController1.EnumerateImplicitlyTrustedUsers().ToArray());
        }
    }
}
