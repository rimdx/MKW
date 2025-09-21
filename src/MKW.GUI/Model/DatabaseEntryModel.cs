using MKW.Core.Storage;

namespace MKW.GUI.Model
{
    public class DatabaseEntryModel
    {
        public required EntryId Id { get; init; }
        public required string? Payload { get; init; }
    }
}
