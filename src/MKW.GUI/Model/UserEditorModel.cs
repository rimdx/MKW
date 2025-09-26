using MKW.Core;

namespace MKW.GUI.Model
{
    public class UserEditorModel : IDisposable
    {
        private readonly DatabaseUnlockedModel database;
        private readonly UserInfo user;

        public UserId Id => user.Id;

        public ReadOnlySpan<byte> PublicKey => user.PublicKey.Span;

        public UserEditorModel(DatabaseUnlockedModel database, UserInfo user)
        {
            this.database = database;
            this.user = user;
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
