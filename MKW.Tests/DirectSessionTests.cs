using MKW.Core;
using MKW.Core.Storage;
using NUnit.Framework.Legacy;

namespace MKW.Tests
{
    public class DirectSessionTests
    {
        [Test]
        public void SimpleAddUserTest()
        {
            Database db = new Database()
            {
                Users = new List<User>()
            };

            DatabaseSession session = new DatabaseSession(db);

            session.AddUser("whattheheckamidoing");

            ClassicAssert.AreEqual(1, db.Users.Count);
        }
    }
}
