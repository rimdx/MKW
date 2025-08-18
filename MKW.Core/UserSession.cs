using MKW.Core.Storage;

namespace MKW.Core
{
    public class UserSession
    {
        private readonly Database db;
        private readonly User user;
        private readonly byte[] decryptedPrivateKey;

        public UserSession(Database db, User user, byte[] decryptedPrivateKey)
        {
            this.db = db;
            this.user = user;
            this.decryptedPrivateKey = decryptedPrivateKey;
        }
    }
}
