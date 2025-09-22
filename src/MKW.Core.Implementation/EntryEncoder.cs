using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Implementation
{
    public class EntryEncoder : IDisposable
    {
        private readonly ICryptographyProvider crypto;
        private readonly IDatabase database;
        private readonly AccessController accessController;

        public EntryEncoder(ICryptographyProvider crypto,
                            IDatabase database,
                            AccessController accessController)
        {
            this.crypto = crypto;
            this.database = database;
            this.accessController = accessController;
        }

        public void EncodeEntry(IDatabaseEntry entry, EntryPayload payload)
        {
            using ISymmetricTransformer payloadEncoder = crypto.CreateSymmetricTransformer();

            Memory<byte> data = payloadEncoder.Encrypt(payload.Data.Span);

            Dictionary<UserId, ReadOnlyMemory<byte>> keys = [];

            foreach (UserId userId in accessController.EnumerateAccess())
            {
                IDatabaseUser user = database.OpenUser(userId, true);

                using IAsymmetricPublicTransformer keyEncoder = crypto.OpenAsymmetricTransformer(
                    user.PublicKey.Payload.Span);

                Memory<byte> encyptedKey = keyEncoder.Encrypt(payloadEncoder.ExportKey().Span);

                keys.Add(user.Id, encyptedKey);
            }

            entry.Keys = keys;
            entry.Data = data;
            entry.Salt = payloadEncoder.ExportIV();
        }

        public void Dispose()
        {
        }
    }
}
