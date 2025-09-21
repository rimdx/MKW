using MKW.Core.Storage;

namespace MKW.Core.Editor
{
    public class UserEditor
        : IUserSession
        , IEntryController
        , ITrustProvider
        , IDisposable
    {
        private readonly IUserSession proxy;

        public UserId Id => proxy.Id;

        public UserEditor(IUserSession proxy)
        {
            this.proxy = proxy;
        }

        public UserMetadata OpenMetadata()
        {
            return proxy.OpenMetadata();
        }

        public void Commit()
        {
        }

        public void Dispose()
        {
            proxy.Dispose();
        }

        // IEntryController

        public IEntrySession CreateEntry()
        {
            return proxy.CreateEntry();
        }

        public IEntrySession CreateEntry(EntryId id)
        {
            return proxy.CreateEntry(id);
        }

        public EntryInfo DeleteEntry(EntryId id)
        {
            return proxy.DeleteEntry(id);
        }

        public IEnumerable<IEntrySession> EnumerateEntries()
        {
            foreach (IEntrySession entry in proxy.EnumerateEntries())
            {
                yield return entry;
            }
        }

        public IEntrySession OpenEntry(EntryId id)
        {
            return proxy.OpenEntry(id);
        }

        public EntryInfo UpdateEntry(EntryId id, EntryPayload? payload)
        {
            return proxy.UpdateEntry(id, payload);
        }

        // ITrustProvider

        public IEnumerable<UserInfo> EnumerateImplicitlyTrustedUsers()
        {
            foreach (UserInfo user in proxy.EnumerateImplicitlyTrustedUsers())
            {
                yield return user;
            }
        }

        public bool VerifyTrust(UserId userId)
        {
            return proxy.VerifyTrust(userId);
        }
    }
}
