using MKW.Core.Notify;
using MKW.Core.Storage;

namespace MKW.GUI.Model
{
    public class UserEditorModel : IDisposable
    {
        private readonly DatabaseModel database;
        private readonly UserInfo user;

        private readonly bool initialTrust;
        private bool newTrust;

        public UserId Id => user.Id;

        public ReadOnlySpan<byte> PublicKey => user.PublicKey.Span;

        public bool Trust => newTrust;

        public UserEditorModel(DatabaseModel database, UserInfo user)
        {
            this.database = database;
            this.user = user;

            initialTrust = database.User!.VerifyTrust(user.Id);
            newTrust = initialTrust;
        }

        public void OnApply()
        {
        }

        public void Dispose()
        {
            /* no-op */
        }
    }
}
