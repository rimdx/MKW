using MKW.Core.Client;
using MKW.Core.Client.Notify;
using MKW.Core.Storage;

namespace MKW.Core.Editor
{
    public class UserEditor
        : IUserSession
        , IEntryController
        , ITrustProvider
        , ITrustController
        , IDisposable
    {
        private readonly IUserSession proxy;
        private readonly TrustEditor trustEditor;

        public UserId Id => proxy.Id;

        public UserEditor(IUserSession proxy)
        {
            this.proxy = proxy;
            trustEditor = new TrustEditor(proxy);
        }

        public void Commit()
        {
            trustEditor.Commit();
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
            foreach (UserInfo user in trustEditor.EnumerateImplicitlyTrustedUsers())
            {
                yield return user;
            }
        }

        public IEnumerable<UserInfo> EnumerateExplicitlyTrustedUsers()
        {
            foreach (UserInfo user in trustEditor.EnumerateExplicitlyTrustedUsers())
            {
                yield return user;
            }
        }

        public Trust GetExplicitTrust(ReadOnlySpan<byte> publicKey)
        {
            return trustEditor.GetExplicitTrust(publicKey);
        }

        public Trust GetImplicitTrust(UserId userId)
        {
            return trustEditor.GetImplicitTrust(userId);
        }

        // ITrustController

        public void UpdateTrust(UserId userId, Trust trust)
        {
            trustEditor.UpdateTrust(userId, trust);
        }
    }
}
