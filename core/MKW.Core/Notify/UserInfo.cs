using MKW.Core.Storage;

namespace MKW.Core.Notify
{
    public record class UserInfo
    {
        public required UserId Id { get; init; }
        public required ReadOnlyMemory<byte> PublicKey { get; init; }

        public Trust Trust { get; set; } = Trust.Unknown;

        public UserInfo()
        {
        }

        public static UserInfo FromDatabaseUser(IDatabaseUser user, Trust trust = Trust.Unknown)
        {
            return new UserInfo
            {
                Id = user.Id,
                PublicKey = user.PublicKey,
                Trust = trust
            };
        }
    }
}
