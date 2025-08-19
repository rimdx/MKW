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
                db.UpdateEntry(id, null);

                return new EntryInfo
                {
                    Id = id,
                    Action = ActionInfo.Deleted,
                    EncodedForUsers = []
                };
            }
            else
            {
                DatabaseUser[] users = db.EnumerateUsers().ToArray();

                DatabaseSecretEntry encodedEntry = EncodeEntry(entry, users);

                List<UserInfo> encodedForUsers = [];
                foreach (DatabaseUser user in users)
                {
                    encodedForUsers.Add(UserInfo.FromDatabaseUser(user));
                }

                DatabaseSecretEntry? oldEntry = db.QueryEntry(id);
                db.UpdateEntry(id, encodedEntry);

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

            byte[] data = payloadEncoder.Encrypt(payload.Data);

            var keys = new Dictionary<Guid, byte[]>();

            foreach (DatabaseUser user in db.EnumerateUsers())
            {
                using AsymmetricTransformer keyEncoder = AsymmetricTransformer.Open(user.PublicKey);

                byte[] encyptedKey = keyEncoder.Encrypt(payloadEncoder.ExportKey());

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
