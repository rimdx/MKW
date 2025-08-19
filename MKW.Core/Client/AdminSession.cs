using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class AdminSession : IDisposable
    {
        private readonly IDatabase db;
        private readonly AdminUser admin;

        private readonly AsymmetricTransformer transformer;

        public AdminSession(IDatabase db, AdminUser admin, byte[] privateKey)
        {
            this.db = db;
            this.admin = admin;

            transformer = AsymmetricTransformer.Open(admin.PublicKey, privateKey);
        }

        public void Dispose()
        {
            transformer.Dispose();
        }
    }
}
