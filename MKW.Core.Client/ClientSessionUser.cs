using MKW.Core.Client.Notify;
using MKW.Core.Cryptography;
using MKW.Core.Storage;

namespace MKW.Core.Client
{
    public partial class ClientSession : IUserController, IDisposable
    {
        public UserInfo PromoteUser(string password)
        {
            return userController.PromoteUser(password);
        }

        public UserSession OpenUser(UserId id, string password)
        {
            return userController.OpenUser(id, password);
        }

        internal IEnumerable<IDatabaseUser> EnumerateDatabaseUsers()
        {
            yield return Database.OpenAdmin(true);

            foreach (IDatabaseUser user in Database.EnumerateUsers())
            {
                yield return user;
            }
        }

        public IEnumerable<UserInfo> EnumerateUsers()
        {
            foreach (UserInfo user in userController.EnumerateUsers())
            {
                yield return user;
            }
        }

        internal IDatabaseUser OpenDatabaseUser(UserId id, bool readOnly)
        {
            if (id.IsAdmin)
            {
                return Database.OpenAdmin(readOnly);
            }
            else
            {
                return Database.OpenUser(id, readOnly);
            }
        }

        public UserSession OpenUser(string password)
        {
            return userController.OpenUser(password);
        }

        public UserSession OpenUser(IDatabaseUser user, UserCredentials creds)
        {
            return userController.OpenUser(user, creds);
        }

        public UserInfo GetUserInfo(UserId id)
        {
            return userController.GetUserInfo(id);
        }
    }
}
