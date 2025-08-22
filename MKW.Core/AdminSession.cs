using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class AdminSession : IDisposable
    {
        private readonly ClientSession client;
        private readonly IDatabaseAdmin admin;

        private readonly AsymmetricTransformer transformer;

        public AdminSession(ClientSession client /* reference */,
                            IDatabaseAdmin admin /* reference */,
                            ReadOnlySpan<byte> privateKey)
        {
            this.client = client;
            this.admin = admin;

            transformer = AsymmetricTransformer.Open(admin.PublicKey.Span, privateKey);
        }

        public void UpdateTrust(Guid userId, Trust trust)
        {
            IDatabaseUser user = client.Database.OpenUser(userId, true);

            ReadOnlyMemory<byte> signature = transformer.Sign(user.PublicKey.Span);

            if (trust == Trust.FullTrust)
            {
                admin.AddTrust(signature);
            }
            else if (trust == Trust.None)
            {
                admin.DeleteTrust(signature);
            }
            else
            {
                throw new ArgumentException("Invalid trust value.", nameof(trust));
            }

            admin.Save();
        }

        public void Dispose()
        {
            transformer.Dispose();
        }
    }
}
