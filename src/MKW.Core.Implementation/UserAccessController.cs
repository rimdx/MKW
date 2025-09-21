using MKW.Core.Notify;
using MKW.Core.Storage;

namespace MKW.Core.Implementation
{
    public class UserAccessController
        : IEntryAccessController
        , IDisposable
    {
        private readonly IUserSession me;

        public UserAccessController(IUserSession me)
        {
            this.me = me;
        }

        public IEnumerable<UserInfo> EnumerateAccess()
        {
            throw new NotImplementedException();
        }

        public void AddAccess(UserId userId)
        {
            EntryId[] ids = [.. EnumerateEntriesToShare(userId)];

            foreach (EntryId id in ids)
            {
                using IEntrySession entry = me.OpenEntry(id);
                entry.AddAccess(userId);
            }

            // TODO: notify the which users are given access to which entries
        }

        private IEnumerable<EntryId> EnumerateEntriesToShare(UserId userId)
        {
            foreach (IEntrySession entry in me.EnumerateEntries())
            {
                if (HasAccess(entry, me.Id) && !HasAccess(entry, userId))
                {
                    // IEntryController.EnumerateEntries always enumerates the entries
                    // as read-only entities, meaning we need to re-open the session an
                    // "editor", by accessing the client API directly.

                    yield return entry.Id;
                }

                // TODO: warn when /me/ doesn't have access to the entry, so
                // unable to share one

                // TODO: warn when the /user/ does already have access to the entry
            }
        }

        private static bool HasAccess(IEntrySession entry, UserId userId)
        {
            foreach (UserInfo access in entry.EnumerateAccess())
            {
                if (access.Id == userId)
                {
                    return true;
                }
            }

            return false;
        }

        public void Dispose()
        {
        }
    }
}
