using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class AdminSession
    {
        private readonly IDatabase db;
        private readonly AdminUser admin;
        private readonly byte[] privateKey;

        public AdminSession(IDatabase db, AdminUser admin, byte[] privateKey)
        {
            this.db = db;
            this.admin = admin;
            this.privateKey = privateKey;
        }
    }
}
