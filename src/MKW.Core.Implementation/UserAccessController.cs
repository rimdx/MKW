namespace MKW.Core.Implementation
{
    public class UserAccessController : IDisposable
    {
        private readonly IUserSession me;

        public UserAccessController(IUserSession me)
        {
            this.me = me;
        }

        public void UpdateKeys()
        {
            EntryId[] ids = [.. EnumerateEntriesToShare()];

            foreach (EntryId id in ids)
            {
                using IEntrySession entry = me.OpenEntry(id);
                entry.UpdateKey();
            }
        }

        private IEnumerable<EntryId> EnumerateEntriesToShare()
        {
            foreach (IEntrySession entry in me.EnumerateEntries())
            {
                yield return entry.Id;
            }
        }

        public void Dispose()
        {
        }
    }
}
