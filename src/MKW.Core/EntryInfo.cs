using MKW.Core.Storage;

namespace MKW.Core
{
    public class EntryInfo
    {
        public required EntryId Id { get; init; }

        public required IReadOnlyList<UserId> EncodedForUsers { get; init; }
    }
}
