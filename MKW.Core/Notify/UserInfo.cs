using MKW.Core.Storage;

namespace MKW.Core.Client.Notify
{
    public record class UserInfo
    {
        public required UserId Id { get; init; }
        public required ReadOnlyMemory<byte> PublicKey { get; init; }

        public Trust Trust { get; set; } = Trust.Unknown;

        public UserInfo()
        {
        }

        internal static UserInfo FromDatabaseUser(IDatabaseUser user)
        {
            return new UserInfo
            {
                Id = user.Id,
                PublicKey = user.PublicKey
            };
        }

        internal static UserInfo FromDatabaseUser(IDatabaseUser user, Trust trust)
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
