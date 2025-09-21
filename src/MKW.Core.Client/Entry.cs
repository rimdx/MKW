using MKW.Core.Implementation;
using MKW.Core.Notify;
using MKW.Core.Storage;
using MKW.Cryptography;

namespace MKW.Core.Client
{
    public class Entry
        : IEntrySession
        , IDisposable
    {
        protected readonly IDatabase database;
        protected readonly ICryptographyProvider crypto;

        // TODO: dispose
        protected readonly IDatabaseEntry entry;
        protected readonly AccessController accessController;
        protected readonly EntryEncoder encoder;

        public EntryId Id => entry.Id;

        protected Entry(IDatabase database,
                        ICryptographyProvider crypto,
                        IDatabaseEntry entry,
                        AccessController accessController)
        {
            this.database = database;
            this.crypto = crypto;
            this.entry = entry;
            this.accessController = accessController;

            encoder = new EntryEncoder(crypto, accessController);
        }

        public static Entry Create(IDatabase database,
                                   ICryptographyProvider crypto,
                                   ITrustProvider trustProvider,
                                   IDatabaseEntry entry)
        {
            AccessController accessController = AccessController.Create(database, trustProvider, entry);

            return new Entry(database,
                             crypto,
                             entry,
                             accessController /* move */);
        }

        public static Entry Open(IDatabase database,
                                 ICryptographyProvider crypto,
                                 IDatabaseEntry entry)
        {
            AccessController accessController = AccessController.Open(database, entry);

            return new Entry(database,
                             crypto,
                             entry,
                             accessController /* move */);
        }

        public EntryInfo UpdatePayload(EntryPayload payload)
        {
            UserInfo[] users = accessController.EnumerateAccess().ToArray();

            encoder.EncodeEntry(entry, payload);

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

        public IEnumerable<UserInfo> EnumerateAccess()
        {
            foreach (UserInfo user in accessController.EnumerateAccess())
            {
                yield return user;
            }
        }

        public virtual void AddAccess(UserId userId)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            accessController.Dispose();
        }
    }
}
