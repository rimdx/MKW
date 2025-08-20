namespace MKW.Core.Client.Notify
{
    public class EntryInfo
    {
        public required ActionInfo Action { get; init; }

        public required Guid Id { get; init; }

        public required IReadOnlyList<UserInfo> EncodedForUsers { get; init; }
    }
}
