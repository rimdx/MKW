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
        protected readonly IDatabaseEntry entry;
        protected readonly IEntryAccessController accessController;
        protected readonly EntryEncoder encoder;

        public EntryId Id => entry.Id;

        protected Entry(ClientSession client,
                        ICryptographyProvider crypto,
                        IDatabaseEntry entry,
                        IEntryAccessController accessController)
        {
            this.client = client;
            this.crypto = crypto;
            this.entry = entry;
            this.accessController = accessController;

            encoder = new EntryEncoder(crypto, accessController);
        }

        public static Entry Create(ClientSession client,
                                   ICryptographyProvider crypto,
                                   ITrustProvider trustProvider,
                                   IDatabaseEntry entry)
        {
            AccessController accessController = AccessController.Create(client, trustProvider, entry);

            return new Entry(client,
                             crypto,
                             entry,
                             accessController /* move */);
        }

        public static Entry Open(ClientSession client,
                                 ICryptographyProvider crypto,
                                 IDatabaseEntry entry)
        {
            AccessController accessController = AccessController.Open(client, entry);

            return new Entry(client,
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
