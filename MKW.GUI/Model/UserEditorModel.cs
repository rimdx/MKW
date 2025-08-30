using MKW.Core.Client.Notify;
using MKW.Core.Storage;
using MKW.GUI.Model;

namespace MKW.GUI
{
    public class UserEditorModel : IDisposable
    {
        private readonly DatabaseModel database;
        private readonly UserInfo user;

        private readonly Trust initialTrust;
        private Trust newTrust;

        public UserId Id => user.Id;

        public ReadOnlySpan<byte> PublicKey => user.PublicKey.Span;

        public Trust Trust => newTrust;

        public UserEditorModel(DatabaseModel database, UserInfo user)
        {
            this.database = database;
            this.user = user;

            initialTrust = GetTrust();
            newTrust = initialTrust;
        }

        private Trust GetTrust()
        {
            foreach (UserInfo userTrust in database.User!.EnumerateUsersTrust())
            {
                if (userTrust.Id == user.Id)
                {
                    return userTrust.Trust;
                }
            }

            return Trust.None;
        }

        public void OnVerify()
        {
            newTrust = Trust.ExplicitTrust;
        }

        public void OnApply()
        {
            if (initialTrust != newTrust)
            {
                database.User!.UpdateTrust(user.Id, newTrust);
            }
        }

        public void Dispose()
        {
            /* no-op */
        }
    }
}
