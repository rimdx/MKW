using MKW.Core.Client.Notify;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public partial class ClientSession : IDisposable
    {
        public Entry OpenEntry(Guid id)
        {
            IDatabaseEntry dbEntry = Database.OpenEntry(id, false);
            return new Entry(this, dbEntry);
        }

        public Entry CreateEntry(Guid id)
        {
            IDatabaseEntry dbEntry = Database.CreateEntry(id);
            dbEntry.Save();
            return new Entry(this, dbEntry);
        }

        public Entry CreateEntry() => CreateEntry(Guid.NewGuid());

        public EntryInfo DeleteEntry(Guid id)
        {
            Database.DeleteEntry(id);

            return new EntryInfo
            {
                Id = id,
                Action = ActionInfo.Deleted,
                EncodedForUsers = []
            };
        }

        public Entry EnsureEntry(Guid id, out bool created)
        {
            created = !Database.HasEntry(id);

            if (created)
            {
                return CreateEntry(id);
            }
            else
            {
                return OpenEntry(id);
            }
        }

        public EntryInfo UpdateEntry(Guid id, EntryPayload? payload)
        {
            if (payload == null)
            {
                return DeleteEntry(id);
            }
            else
            {
                using Entry entry = EnsureEntry(id, out bool created);

                EntryInfo notify = entry.UpdatePayload(payload);

                return new EntryInfo
                {
                    Id = notify.Id,
                    EncodedForUsers = notify.EncodedForUsers,
                    Action = created ? ActionInfo.Added : ActionInfo.Updated,
                };
            }
        }
    }
}
