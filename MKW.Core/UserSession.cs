using MKW.Core.Storage;

namespace MKW.Core
{
    public class UserSession
    {
        private readonly IDatabaseSession db;
        private readonly User user;
        private readonly byte[] decryptedPrivateKey;

        public UserSession(IDatabaseSession db, User user, byte[] decryptedPrivateKey)
        {
            this.db = db;
            this.user = user;
            this.decryptedPrivateKey = decryptedPrivateKey;
        }
    }
}
