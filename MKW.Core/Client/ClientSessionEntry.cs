using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public partial class ClientSession : IDisposable
    {
        public EntryInfo UpdateEntry(Guid id, EntryPayload? entry)
        {
            if (entry == null)
            {
                Database.UpdateEntry(id, null);

                return new EntryInfo
                {
                    Id = id,
                    Action = ActionInfo.Deleted,
                    EncodedForUsers = []
                };
            }
            else
            {
                DatabaseUser[] users = Database.EnumerateUsers().ToArray();

                DatabaseSecretEntry encodedEntry = EncodeEntry(entry, users);

                List<UserInfo> encodedForUsers = [];
                foreach (DatabaseUser user in users)
                {
                    encodedForUsers.Add(UserInfo.FromDatabaseUser(user));
                }

                DatabaseSecretEntry? oldEntry = Database.QueryEntry(id);
                Database.UpdateEntry(id, encodedEntry);

                return new EntryInfo
                {
                    Id = id,
                    Action = oldEntry == null ? ActionInfo.Added : ActionInfo.Updated,
                    EncodedForUsers = encodedForUsers
                };
            }
        }

        public DatabaseSecretEntry EncodeEntry(EntryPayload payload, IEnumerable<DatabaseUser> users)
        {
            using SymmetricTransformer payloadEncoder = SymmetricTransformer.Create();

            Memory<byte> data = payloadEncoder.Encrypt(payload.Data.Span);

            var keys = new Dictionary<Guid, Memory<byte>>();

            foreach (DatabaseUser user in Database.EnumerateUsers())
            {
                using AsymmetricTransformer keyEncoder = AsymmetricTransformer.Open(user.PublicKey.Span);

                Memory<byte> encyptedKey = keyEncoder.Encrypt(payloadEncoder.ExportKey().Span);

                keys.Add(user.Id, encyptedKey);
            }

            DatabaseSecretEntry entry = new DatabaseSecretEntry
            {
                Keys = keys,
                Data = data,
                Salt = payloadEncoder.ExportIV(),
            };

            return entry;
        }
    }
}
