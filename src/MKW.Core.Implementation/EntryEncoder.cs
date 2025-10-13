using MKW.Core.Serialization;
using MKW.Cryptography;
using MKW.Storage;

namespace MKW.Core.Implementation
{
    public class EntryEncoder : IDisposable
    {
        private readonly ICryptographyProvider crypto;
        private readonly IDatabase database;
        private readonly IUserSession user;

        public EntryEncoder(ICryptographyProvider crypto,
                            IDatabase database,
                            IUserSession user)
        {
            this.crypto = crypto;
            this.database = database;
            this.user = user;
        }

        public DatabaseEntry EncodeEntry(DatabaseEntry entry, EntryPayload payload)
        {
            using ISymmetricTransformer payloadEncoder = crypto.CreateSymmetricTransformer(
                CommonCryptographyAlgorithms.Aes128Gcm);

            ReadOnlyMemory<byte> serializedPayload = EntryPayloadSerializer.Serialize(payload);
            ReadOnlyMemory<byte> data = payloadEncoder.Encrypt(serializedPayload.Span);

            Dictionary<UserId, ReadOnlyMemory<byte>> keys = [];

            foreach (UserId userId in user.EnumerateTrustedUsers())
            {
                DatabaseUser user = database.OpenUser(userId);

                using IAsymmetricPublicTransformer keyEncoder = crypto.OpenAsymmetricTransformer(
                    user.PublicKey.Payload.Span,
                    CommonCryptographyAlgorithms.Rsa2048);

                Memory<byte> encyptedKey = keyEncoder.Encrypt(payloadEncoder.ExportKey().Span);

                keys.Add(user.Id, encyptedKey);
            }

            return entry with
            {
                Keys = keys,
                Data = data,
                Salt = payloadEncoder.ExportIV(),
            };
        }

        public void Dispose()
        {
        }
    }
}
