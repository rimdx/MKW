using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class Entry : IDisposable
    {
        protected readonly ClientSession client;
        // TODO: dispose
        protected readonly ITrustProvider trustProvider;
        protected readonly IDatabaseEntry entry;

        public EntryId Id => entry.Id;

        public Entry(ClientSession client, ITrustProvider trustProvider, IDatabaseEntry entry)
        {
            this.client = client;
            this.trustProvider = trustProvider;
            this.entry = entry;
        }

        public EntryInfo UpdatePayload(EntryPayload payload)
        {
            UserInfo[] users =
                trustProvider.EnumerateUsersTrust()
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

        public IEnumerable<UserInfo> EnumerateEncodedForUsers()
        {
            foreach (UserId id in entry.Keys.Keys)
            {
                IDatabaseUser user = client.OpenDatabaseUser(id, true);
                yield return UserInfo.FromDatabaseUser(user);
            }
        }

        public void Dispose()
        {
        }
    }
}
