using MKW.Core.Client.Notify;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public partial class ClientSession : IDisposable
    {
        private EntryController OpenEntryController()
        {
            return new EntryController(this, Database.OpenAdmin(true));
        }

        public IEntrySession OpenEntry(EntryId id)
        {
            using EntryController entryController = OpenEntryController();
            return entryController.OpenEntry(id);
        }

        public IEntrySession CreateEntry(EntryId id)
        {
            using EntryController entryController = OpenEntryController();
            return entryController.CreateEntry(id);
        }

        public IEntrySession CreateEntry()
        {
            using EntryController entryController = OpenEntryController();
            return entryController.CreateEntry();
        }

        public EntryInfo DeleteEntry(EntryId id)
        {
            using EntryController entryController = OpenEntryController();
            return entryController.DeleteEntry(id);
        }

        public EntryInfo UpdateEntry(EntryId id, EntryPayload? payload)
        {
            using EntryController entryController = OpenEntryController();
            return entryController.UpdateEntry(id, payload);
        }
    }
}
