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

                IDatabaseUser[] users = Database.EnumerateUsers().ToArray();

                EncodeEntry(entry, payload, users);

                List<UserInfo> encodedForUsers = [];
                foreach (IDatabaseUser user in users)
                {
                    encodedForUsers.Add(UserInfo.FromDatabaseUser(user));
                }

                return new EntryInfo
                {
                    Id = id,
                    Action = created ? ActionInfo.Added : ActionInfo.Updated,
                    EncodedForUsers = encodedForUsers
                };
            }
        }

        public void EncodeEntry(IDatabaseEntry entry, EntryPayload payload, IEnumerable<IDatabaseUser> users)
        {
            using SymmetricTransformer payloadEncoder = SymmetricTransformer.Create();

            Memory<byte> data = payloadEncoder.Encrypt(payload.Data.Span);

            Dictionary<Guid, ReadOnlyMemory<byte>> keys = new Dictionary<Guid, ReadOnlyMemory<byte>>();

            foreach (IDatabaseUser user in Database.EnumerateUsers())
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
