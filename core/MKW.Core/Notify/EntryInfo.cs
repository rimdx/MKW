using MKW.Core.Storage;

namespace MKW.Core.Notify
{
    public class EntryInfo
    {
        public required ActionInfo Action { get; init; }

        public required EntryId Id { get; init; }

        public required IReadOnlyList<UserInfo> EncodedForUsers { get; init; }
    }
}
