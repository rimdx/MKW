using MKW.Core;
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
        public string Name
        {
            get
            {
                if (Id.IsAdmin)
                {
                    return "Admin";
                }
                else
                {
                    return Formatters.FormatUserName(user.Metadata.UserId, user.Metadata.DisplayName);
                }
            }
        }

        public ImageMoniker Icon => IsAdmin ? ImageMoniker.Admin : ImageMoniker.User;
    }
}
