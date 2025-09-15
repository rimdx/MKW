using MKW.Core.Notify;
using MKW.Core.Storage;
using MKW.GUI.Images;

namespace MKW.GUI.Model
{
    public class DatabaseUserModel
    {
        private readonly UserInfo user;

        public DatabaseUserModel(UserInfo user)
        {
            this.user = user;
        }

        public UserId Id => user.Id;
        public bool IsAdmin => Id.IsAdmin;
        public string Name => IsAdmin ? "Admin" : "User";
        public object Icon => IsAdmin ? new Admin() : new User();
    }
}
