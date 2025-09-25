using MKW.Core;

namespace MKW.GUI.Model
{
    public class UserEditorModel : IDisposable
    {
        private readonly DatabaseUnlockedModel database;
        private readonly UserInfo user;

        private readonly bool initialTrust;
        private bool newTrust;

        public UserId Id => user.Id;

        public ReadOnlySpan<byte> PublicKey => user.PublicKey.Span;

        public bool Trust => newTrust;

        public UserEditorModel(DatabaseUnlockedModel database, UserInfo user)
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
