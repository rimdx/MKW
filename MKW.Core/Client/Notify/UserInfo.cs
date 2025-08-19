using MKW.Core.Storage;

namespace MKW.Core.Client.Notify
{
    public record class UserInfo
    {
        public required Guid Id { get; init; }
        public required byte[] PublicKey { get; init; }

        public Trust Trust { get; set; } = Trust.Unknown;

        public UserInfo()
        {
        }

        internal static UserInfo FromDatabaseUser(DatabaseUser user)
        {
            return new UserInfo
            {
                Id = user.Id,
                PublicKey = user.PublicKey
            };
        }
    }
}
