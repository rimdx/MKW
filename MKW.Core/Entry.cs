using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class Entry : IDisposable
    {
        protected readonly ClientSession client;
        protected readonly IDatabaseEntry entry;

        public EntryId Id => entry.Id;

        public Entry(ClientSession client, IDatabaseEntry entry)
        {
            this.client = client;
            this.entry = entry;
        }

        public EntryInfo UpdatePayload(EntryPayload payload)
        {
            UserInfo[] users =
                client.EnumerateUsersTrust()
                .Where(user => user.Trust switch
                {
                    Trust.None => false,
                    Trust.Unknown => false,
                    Trust.ExplicitTrust => true,
                    Trust.ImplicitTrust => true,
                    Trust.SelfTrust => true,
                }).ToArray();

            EncodeEntry(entry, payload, users);

            entry.Save();

            return new EntryInfo
            {
                Id = entry.Id,
                Action = ActionInfo.Updated,
                EncodedForUsers = users
            };
        }

        internal static void EncodeEntry(IDatabaseEntry entry, EntryPayload payload, IEnumerable<UserInfo> users)
        {
            using SymmetricTransformer payloadEncoder = SymmetricTransformer.Create();

            Memory<byte> data = payloadEncoder.Encrypt(payload.Data.Span);

            Dictionary<UserId, ReadOnlyMemory<byte>> keys = new Dictionary<UserId, ReadOnlyMemory<byte>>();

            foreach (UserInfo user in users)
            {
                using AsymmetricTransformer keyEncoder = AsymmetricTransformer.Open(user.PublicKey.Span);

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
