using MKW.Core.Client;
using MKW.Core.Client.Notify;
using NUnit.Framework.Legacy;

namespace MKW.Core.Editor.Tests
{
    public class UserEditorTests
    {
        [Test]
        public void SimpleTrustEditTest()
        {
            using SandBox sbox = new SandBox();
            using ClientSession client = sbox.OpenSession();

            using UserSession user1 = sbox.CreateUser(client, "user1", out UserInfo userInfo1, false);
            using UserEditor editor = new UserEditor(user1);

            using UserSession user2 = sbox.CreateUser(client, "user2", out UserInfo userInfo2, false);
            ClassicAssert.AreEqual(Trust.None, user1.GetImplicitTrust(user2.Id));
            // ClassicAssert.AreEqual(Trust.None, editor.GetImplicitTrust(user2.Id));

            editor.AddTrust(user2.Id);
            ClassicAssert.AreEqual(Trust.None, user1.GetImplicitTrust(user2.Id));
            // ClassicAssert.AreEqual(Trust.ExplicitTrust, editor.GetImplicitTrust(user2.Id));

            editor.Commit();
            ClassicAssert.AreEqual(Trust.ExplicitTrust, user1.GetImplicitTrust(user2.Id));
            // ClassicAssert.AreEqual(Trust.ExplicitTrust, editor.GetImplicitTrust(user2.Id));

            editor.Commit();
            ClassicAssert.AreEqual(Trust.ExplicitTrust, user1.GetImplicitTrust(user2.Id));
            // ClassicAssert.AreEqual(Trust.ExplicitTrust, editor.GetImplicitTrust(user2.Id));

            editor.RemoveTrust(user2.Id);
            editor.AddTrust(user2.Id);
            ClassicAssert.AreEqual(Trust.ExplicitTrust, user1.GetImplicitTrust(user2.Id));
            // ClassicAssert.AreEqual(Trust.ExplicitTrust, editor.GetImplicitTrust(user2.Id));

            editor.RemoveTrust(user2.Id);
            ClassicAssert.AreEqual(Trust.ExplicitTrust, user1.GetImplicitTrust(user2.Id));
            // ClassicAssert.AreEqual(Trust.None, editor.GetImplicitTrust(user2.Id));

            editor.Commit();
            ClassicAssert.AreEqual(Trust.None, user1.GetImplicitTrust(user2.Id));
            // ClassicAssert.AreEqual(Trust.None, editor.GetImplicitTrust(user2.Id));
        }
    }
}
