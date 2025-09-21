using MKW.Core.Notify;
using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Implementation
{
    public class EntryEncoder : IDisposable
    {
        private readonly ICryptographyProvider crypto;
        private readonly AccessController accessController;

        public EntryEncoder(ICryptographyProvider crypto,
                            AccessController accessController)
        {
            this.crypto = crypto;
            this.accessController = accessController;
        }

        public void EncodeEntry(IDatabaseEntry entry, EntryPayload payload)
        {
            using ISymmetricTransformer payloadEncoder = crypto.CreateSymmetricTransformer();

            Memory<byte> data = payloadEncoder.Encrypt(payload.Data.Span);

            Dictionary<UserId, ReadOnlyMemory<byte>> keys = [];

            foreach (UserInfo user in accessController.EnumerateAccess())
            {
                using IAsymmetricPublicTransformer keyEncoder = crypto.OpenAsymmetricTransformer(user.PublicKey.Span);

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
