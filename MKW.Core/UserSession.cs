using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class UserSession : IDisposable
    {
        private readonly ClientSession client;
        private readonly IDatabaseUser user;

        public Guid Id => user.Id;
        public AsymmetricTransformer Transformer { get; }

        public UserSession(ClientSession client /* reference */,
                           IDatabaseUser user /* reference */,
                           ReadOnlySpan<byte> privateKey)
        {
            this.client = client;
            this.user = user;

            Transformer = AsymmetricTransformer.Open(user.PublicKey.Span, privateKey);
        }

        public UserEntry OpenEntry(Guid id)
        {
            IDatabaseEntry entry = client.Database.OpenEntry(id, true);

            return new UserEntry(client, this, entry);
        }

        public IEnumerable<UserEntry> EnumerateEntries()
        {
            foreach (IDatabaseEntry entry in client.Database.EnumerateEntries())
            {
                yield return new UserEntry(client, this, entry);
            }
        }

        public void Dispose()
        {
            Transformer.Dispose();
        }
    }
}
