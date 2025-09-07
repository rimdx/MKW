using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public class Entry
        : IEntrySession
        , IEntryAccessController
        , IDisposable
    {
        protected readonly ClientSession client;
        protected readonly ICryptographyProvider crypto;

        // TODO: dispose
        protected readonly ITrustProvider trustProvider;
        protected readonly IDatabaseEntry entry;
        protected readonly IEntryAccessController accessController;

        public EntryId Id => entry.Id;

        public Entry(ClientSession client,
                     ICryptographyProvider crypto,
                     ITrustProvider trustProvider,
                     IDatabaseEntry entry)
        {
            this.client = client;
            this.crypto = crypto;
            this.trustProvider = trustProvider;
            this.entry = entry;

            accessController = new AccessController(client, trustProvider, entry);
        }

        public EntryInfo UpdatePayload(EntryPayload payload)
        {
            UserInfo[] users = accessController.EnumerateAccess().ToArray();

            EncodeEntry(entry, payload, users);

            entry.Save();

            return new EntryInfo
            {
                Id = entry.Id,
                Action = ActionInfo.Updated,
                EncodedForUsers = users
            };
        }

        public virtual EntryPayload? OpenPayload()
        {
            return null;
        }

        internal void EncodeEntry(IDatabaseEntry entry, EntryPayload payload, IEnumerable<UserInfo> users)
        {
            using ISymmetricTransformer payloadEncoder = crypto.CreateSymmetricTransformer();

            Memory<byte> data = payloadEncoder.Encrypt(payload.Data.Span);

            Dictionary<UserId, ReadOnlyMemory<byte>> keys = new Dictionary<UserId, ReadOnlyMemory<byte>>();

            foreach (UserInfo user in users)
            {
                using IAsymmetricPublicTransformer keyEncoder = crypto.OpenAsymmetricTransformer(user.PublicKey.Span);

                Memory<byte> encyptedKey = keyEncoder.Encrypt(payloadEncoder.ExportKey().Span);

                keys.Add(user.Id, encyptedKey);
            }

            entry.Keys = keys;
            entry.Data = data;
            entry.Salt = payloadEncoder.ExportIV();
        }

        public IEnumerable<UserInfo> EnumerateAccess()
        {
            foreach (UserInfo user in accessController.EnumerateAccess())
            {
                yield return user;
            }
        }

        public virtual void AddAccess(UserId userId)
        {
            EntryPayload? payload = OpenPayload();

            if (payload == null)
            {
                throw new Exception("The entry is not encrypted for this user.");
            }

            accessController.AddAccess(userId);
            EncodeEntry(entry, payload, accessController.EnumerateAccess());

            entry.Save();
        }

        public void Dispose()
        {
            accessController.Dispose();
        }
    }
}
