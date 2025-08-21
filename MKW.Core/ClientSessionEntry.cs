using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public partial class ClientSession : IDisposable
    {
        public EntryInfo UpdateEntry(Guid id, EntryPayload? payload)
        {
            if (payload == null)
            {
                Database.DeleteEntry(id);

                return new EntryInfo
                {
                    Id = id,
                    Action = ActionInfo.Deleted,
                    EncodedForUsers = []
                };
            }
            else
            {
                using IDatabaseEntry entry = Database.OpenEntry(id,
                                                                DatabaseOpenMode.OpenOrCreate,
                                                                out bool created);

                UserInfo[] users = EnumerateUsersTrust().Where(user => user.Trust == Trust.FullTrust).ToArray();

                EncodeEntry(entry, payload, users);

                return new EntryInfo
                {
                    Id = id,
                    Action = created ? ActionInfo.Added : ActionInfo.Updated,
                    EncodedForUsers = users
                };
            }
        }

        public void EncodeEntry(IDatabaseEntry entry, EntryPayload payload, IEnumerable<UserInfo> users)
        {
            using SymmetricTransformer payloadEncoder = SymmetricTransformer.Create();

            Memory<byte> data = payloadEncoder.Encrypt(payload.Data.Span);

            Dictionary<Guid, ReadOnlyMemory<byte>> keys = new Dictionary<Guid, ReadOnlyMemory<byte>>();

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
    }
}
