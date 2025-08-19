using MKW.Core.Client;

namespace MKW.Tests
{
    [TestFixture]
    public class AdminTests
    {
        [Test]
        public void AddOpenSimpleTest()
        {
            using var sbox = new SandBox();
            using var db = sbox.OpenDatabase();

            var client = new ClientSession(db);

            var admin = client.PromoteAdmin("adminsecret");

            var adminSession = client.OpenAdmin("adminsecret");
        }
    }
}
