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
        protected readonly EntryEncoder encoder;

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
            encoder = new EntryEncoder(crypto, accessController);
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
            EntryPayload? payload = OpenPayload();

            if (payload == null)
            {
                throw new Exception("The entry is not encrypted for this user.");
            }

            accessController.AddAccess(userId);
            encoder.EncodeEntry(entry, payload);

            entry.Save();
        }

        public void Dispose()
        {
            accessController.Dispose();
        }
    }
}
