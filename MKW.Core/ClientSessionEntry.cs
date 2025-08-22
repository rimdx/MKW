using MKW.Core.Client.Notify;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public partial class ClientSession : IDisposable
    {
        public EntryInfo UpdateEntry(Guid id, EntryPayload? payload)
        {
            if (payload == null)
            {
                Database.DeleteEntry(id);

                return new EntryInfo
                {
                    Id = id,
                    Action = ActionInfo.Deleted,
                    EncodedForUsers = []
                };
            }
            else
            {
                bool exists = Database.HasEntry(id);

                using IDatabaseEntry dbEntry = exists ? Database.OpenEntry(id, false) : Database.CreateEntry(id);

                using Entry entry = new Entry(this, dbEntry);

                EntryInfo notify = entry.UpdatePayload(payload);

                return new EntryInfo
                {
                    Id = notify.Id,
                    EncodedForUsers = notify.EncodedForUsers,
                    Action = exists ? ActionInfo.Updated : ActionInfo.Added,
                };
            }
        }
    }
}
