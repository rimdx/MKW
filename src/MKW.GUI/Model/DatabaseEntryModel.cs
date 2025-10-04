using MKW.Core;

namespace MKW.GUI.Model
{
    public class DatabaseEntryModel
    {
        public required EntryId Id { get; init; }
        public required EntryPayload? Payload { get; init; }

        public string? Notes => Payload?.GetProperty(EntryPayloadCommonProperties.Notes);
    }
}
