namespace MKW.Core
{
    public record class UserMetadata
    {
        public required string UserId { get; init; }
        public required string DisplayName { get; init; }
    }
}
